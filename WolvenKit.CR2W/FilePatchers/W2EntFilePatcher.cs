using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.BatchProcessors;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2EntFilePatcher : W2XFilePatcher
    {
        private const string TypeCEntityTemplate = "CEntityTemplate";
        private const string TypeEngineTransform = "EngineTransform";
        private const string TypeCFXDefinition = "CFXDefinition";
        private const string TypeCFXTrackItemParticles = "CFXTrackItemParticles";
        private const string TypeCParticleSystem = "CParticleSystem";
        private const string TypeCPointLightComponent = "CPointLightComponent";
        private const string TypeSharedDataBuffer = "SharedDataBuffer";

        private const string VariableNameBuffer = "buffer";
        private const string VariableNameCookedEffects = "cookedEffects";
        private const string VariableNameFlatCompiledData = "flatCompiledData";
        private const string VariableNameShowDistance = "showDistance";
        private const string VariableNameTransform = "transform";

        private const float ValueShowDistanceIDD = 1200;

        private const float MinValueTransformYForCandles = 0.01F;

        public W2EntFilePatcher(ILocalizedStringSource localizedStringSource) : base(localizedStringSource)
        {
        }

        public void PatchForIncreasedDrawDistance(string filePath, Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap)
        {
            CR2WFile w2EntFile = ReadW2EntFile(filePath, localizedStringSource);
            (List<CByteArrayContainer> sharedDataBuffersForFires, CByteArrayContainer flatCompiledData) = ReadSharedDataBuffersAndFlatCompiledDataForFires(w2EntFile);

            if (sharedDataBuffersForFires == null || !sharedDataBuffersForFires.Any())
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no shared data buffer.");
            }

            foreach (CByteArrayContainer sharedDataBufferForFire in sharedDataBuffersForFires) {

                PatchFireShowDistance(filePath, sharedDataBufferForFire);
                PatchW2PFilePath(filePath, sharedDataBufferForFire, relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap);

                WriteCByteArrayContainer(sharedDataBufferForFire);
            }

            if(PatchGlowAutoHideDistance(filePath, flatCompiledData))
            {
                WriteCByteArrayContainer(flatCompiledData);
            }

            // TODO probably get rid of the 2nd arg
            WriteCR2WFile(w2EntFile, w2EntFile.FileName);
        }

        internal static CR2WFile ReadW2EntFile(string filePath, ILocalizedStringSource localizedStringSource)
        {
            return ReadCR2WFile(filePath, localizedStringSource);
        }

        internal static (List<CByteArrayContainer> sharedDataBuffersForFires, CByteArrayContainer flatCompiledData) ReadSharedDataBuffersAndFlatCompiledDataForFires(CR2WFile w2EntFile)
        {
            List<CByteArrayContainer> sharedDataBuffersForFires = null;
            CByteArrayContainer flatCompiledData = null;

            string filePath = w2EntFile.FileName;

            if (w2EntFile == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            bool cEntityTemplateFound = false;

            foreach (CR2WChunk chunk in w2EntFile.chunks)
            {
                if (!IsCEntityTemplate(chunk))
                {
                    continue;
                }
                else if (cEntityTemplateFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + TypeCEntityTemplate + "'.");
                }

                cEntityTemplateFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCEntityTemplate + "'.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCookedEffects(variable))
                    {
                        if (sharedDataBuffersForFires != null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one cooked effects variable with shared data buffers.");
                        }

                        sharedDataBuffersForFires = GetSharedDataBuffersForFiresFromCookedEffectsVariable(variable, filePath, w2EntFile.LocalizedStringSource);
                    }
                    else if (IsFlatCompiledData(variable))
                    {
                        if (flatCompiledData != null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one flat compiled data.");
                        }

                        CR2WFile flatCompiledDataContent = ReadCByteArrayContainerContent((CByteArray)variable, w2EntFile.LocalizedStringSource);

                        if (flatCompiledDataContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains flat compiled data which could not be read.");
                        }

                        flatCompiledData = new CByteArrayContainer(flatCompiledDataContent, (CByteArray)variable);
                    }
                }
            }

            return (sharedDataBuffersForFires, flatCompiledData);
        }

        private static List<CByteArrayContainer> GetSharedDataBuffersForFiresFromCookedEffectsVariable(CVariable variableCookedEffects, string filePath, ILocalizedStringSource localizedStringSource)
        {
            List<CByteArrayContainer> sharedDataBuffersForFires = new List<CByteArrayContainer>();

            foreach (CVariable cookedEffectsEntry in ((CArray)variableCookedEffects).array)
            {
                if (cookedEffectsEntry is CVector)
                {
                    List<CVariable> cookedEffectsVariables = ((CVector)cookedEffectsEntry).variables;

                    if (cookedEffectsVariables.Count < 2)
                    {
                        throw new System.InvalidOperationException("File '" + filePath + "' contains a '" + VariableNameCookedEffects + "' entry with less than 2 variables.");
                    }

                    if (IsFire(cookedEffectsVariables[0]) && IsSharedDataBuffer(cookedEffectsVariables[1]))
                    {
                        CR2WFile sharedDataBufferContent = ReadCByteArrayContainerContent((CByteArray)cookedEffectsVariables[1], localizedStringSource);

                        if (sharedDataBufferContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains a shared data buffer which could not be read.");
                        }

                        sharedDataBuffersForFires.Add(new CByteArrayContainer(sharedDataBufferContent, (CByteArray)cookedEffectsVariables[1]));
                    }
                }
            }

            return sharedDataBuffersForFires;
        }

        private static void PatchFireShowDistance(string filePath, CByteArrayContainer sharedDataBufferForFire)
        {
            bool cFXDefinitionFound = false;

            foreach (CR2WChunk chunk in sharedDataBufferForFire.Content.chunks)
            {
                if (!IsCFXDefinition(chunk))
                {
                    continue;
                }
                else if (cFXDefinitionFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + TypeCFXDefinition + "'.");
                }

                cFXDefinitionFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCFXDefinition + "'.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool showDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsShowDistance(variable))
                    {
                        if (showDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one attribute '" + VariableNameShowDistance + "'.");
                        }

                        PatchShowDistance(variable);

                        showDistanceFound = true;
                    }
                }

                if (!showDistanceFound)
                {
                    AddShowDistance(sharedDataBufferForFire.Content, chunkData);
                }
            }

            if (!cFXDefinitionFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + TypeCFXDefinition + "'.");
            }
        }

        private static void PatchShowDistance(CVariable variableShowDistance)
        {
            ((CFloat)variableShowDistance).SetValue(ValueShowDistanceIDD);
        }

        private static void AddShowDistance(CR2WFile file, CVector chunkData)
        {
            CFloat showDistanceVariable = new CFloat(file);

            showDistanceVariable.Type = CVariableTypeFloat;
            showDistanceVariable.Name = VariableNameShowDistance;
            showDistanceVariable.SetValue(ValueShowDistanceIDD);

            chunkData.variables.Add(showDistanceVariable);
        }

        private static void PatchW2PFilePath(string w2EntFilePath, CByteArrayContainer sharedDataBuffer, Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap)
        {
            //bool cFXTrackItemParticlesFound = false;

            foreach (CR2WChunk chunk in sharedDataBuffer.Content.chunks)
            {
                if (!IsCFXTrackItemParticles(chunk))
                {
                    continue;
                }

                //cFXTrackItemParticlesFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCFXTrackItemParticles + "'.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCSoftParticleSystem(variable))
                    {
                        CSoft variableCSoftParticleSystem = (CSoft)variable;

                        string relativeW2PFilePath = variableCSoftParticleSystem.Handle;
                        string relativeRenamedW2PFilePath;

                        if (relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap.TryGetValue(relativeW2PFilePath, out relativeRenamedW2PFilePath))
                        {
                            PatchW2PFilePath(variableCSoftParticleSystem, relativeRenamedW2PFilePath);
                        }
                        else if (!relativeW2PFilePath.EndsWith(W2XFileHandler.FileNameSuffixILOD + W2XFileHandler.FileExtensionW2P))
                        {
                            // TODO this should be tracked but the processing should still finish
                            //throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains a w2p path '" + relativeW2PFilePath + "' which is not yet renamed.");
                        }
                    }
                }
            }

            // TODO maybe enable this check again for some final testing but it should be removed in the release version
            //if (!cFXTrackItemParticlesFound)
            //{
            //    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains no chunk of type '" + TypeCFXTrackItemParticles + "'.");
            //}
        }

        private static void PatchW2PFilePath(CSoft variableCSoftParticleSystem, string relativeRenamedW2PFilePath)
        {
            variableCSoftParticleSystem.Handle = relativeRenamedW2PFilePath;
        }

        private bool PatchGlowAutoHideDistance(string w2EntFilePath, CByteArrayContainer flatCompiledData)
        {
            bool cPointLightComponentFound = false;

            foreach (CR2WChunk chunk in flatCompiledData.Content.chunks)
            {
                if (!IsCPointLightComponent(chunk))
                {
                    continue;
                }

                cPointLightComponentFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCPointLightComponent + "'.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool autoHideDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsAutoHideDistance(variable))
                    {
                        if (autoHideDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains more than one attribute '" + VariableNameAutoHideDistance + "'.");
                        }

                        PatchAutoHideDistance(variable);

                        autoHideDistanceFound = true;
                    }
                    // TODO maybe the campfires should be restricted further, so far only campfire_01.w2ent was observed to have issues
                    else if (IsTransform(variable) && (Path.GetFileName(w2EntFilePath).Contains("candle") || Path.GetFileName(w2EntFilePath).Contains("campfire_")))
                    {
                        CEngineTransform transform = (CEngineTransform)variable;

                        float valueY = transform.y.val;

                        // For some reason changing some coordinates slightly makes the glow render at a bigger distance for a few of the light sources...
                        if (valueY <= 0)
                        {
                            transform.y.SetValue(MinValueTransformYForCandles);
                        }
                    }
                }

                if (!autoHideDistanceFound)
                {
                    AddAutoHideDistance(flatCompiledData.Content, chunkData);
                }
            }

            return cPointLightComponentFound;
        }

        internal static List<string> GetW2PFilePathsForFires(CByteArrayContainer sharedDataBuffer, string w2EntFilePath, string modDirectory, string dlcDirectory, ILocalizedStringSource localizedStringSource)
        {
            List<string> w2PFilePathsForFires = new List<string>();

            foreach (CR2WChunk chunk in sharedDataBuffer.Content.chunks)
            {
                if (!IsCFXTrackItemParticles(chunk))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains either no or invalid chunk data for type '" + TypeCFXTrackItemParticles + "'.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCSoftParticleSystem(variable))
                    {
                        CSoft variableCSoftParticleSystem = (CSoft)variable;

                        string relativeW2PFilePath = variableCSoftParticleSystem.Handle;
                        string initialPath = relativeW2PFilePath.StartsWith(W2XFileHandler.PathDLC) ? dlcDirectory : modDirectory;

                        string absoluteW2PFilePath = initialPath + Path.DirectorySeparatorChar + W2XFileHandler.PathBundle + Path.DirectorySeparatorChar + relativeW2PFilePath;
                        string w2pFileName = relativeW2PFilePath.Substring(relativeW2PFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        if ((w2pFileName.Contains(LabelFire) || w2pFileName.Contains(LabelFlame) || w2pFileName.Contains("_candle") || w2pFileName.Contains("_brazier") || w2pFileName.Contains("torch")
                            || w2pFileName.Contains("chandelier") || (w2pFileName.Contains("coal") && !w2pFileName.Contains("smoke")))
                            && !relativeW2PFilePath.Contains("arson") && !relativeW2PFilePath.Contains("arachas") && !relativeW2PFilePath.Contains("weapons") && !relativeW2PFilePath.Contains("gameplay")
                            && !relativeW2PFilePath.Contains("monsters") && !relativeW2PFilePath.Contains("characters") && !relativeW2PFilePath.Contains("environment") && !relativeW2PFilePath.Contains("work")
                            && !relativeW2PFilePath.Contains("igni"))
                        {
                            w2PFilePathsForFires.Add(absoluteW2PFilePath); 
                        }
                    }
                }
            }

            return w2PFilePathsForFires;
        }

        private static bool IsCEntityTemplate(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCEntityTemplate);
        }

        private static bool IsCFXDefinition(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCFXDefinition);
        }

        private static bool IsCFXTrackItemParticles(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCFXTrackItemParticles);
        }

        private static bool IsCookedEffects(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(VariableNameCookedEffects);
        }

        private static bool IsCSoftParticleSystem(CVariable variable)
        {
            return variable is CSoft && ((CSoft)variable).FileType.Equals(TypeCParticleSystem);
        }

        private static bool IsCPointLightComponent(CR2WChunk chunk)
        {
            return chunk.Type.Equals(TypeCPointLightComponent);
        }

        private static bool IsFire(CVariable variable)
        {
            if (!(variable is CName))
            {
                return false;
            }

            string variableValue = ((CName)variable).Value;

            return variableValue.Contains(LabelFire) || variableValue.Contains("torch") || variableValue.Contains("destroy") || variableValue.Contains("light_on")
                || variableValue.Equals("effects") || variableValue.Equals("active") || variableValue.Equals("candle") || variableValue.Equals("smoke") || variableValue.Equals("burn") || variableValue.Equals("active_effect");
        }

        private static bool IsFlatCompiledData(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(VariableNameFlatCompiledData);
        }

        private static bool IsSharedDataBuffer(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(VariableNameBuffer) && variable.Type.Equals(TypeSharedDataBuffer);
        }

        private static bool IsShowDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameShowDistance);
        }
        private static bool IsTransform(CVariable variable)
        {
            return variable is CEngineTransform && variable.Name.Equals(VariableNameTransform) && variable.Type.Equals(TypeEngineTransform);
        }
    }
}
