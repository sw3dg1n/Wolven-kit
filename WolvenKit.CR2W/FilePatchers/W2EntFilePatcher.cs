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
        private const string TypeCFXDefinition = "CFXDefinition";
        private const string TypeCFXTrackItemParticles = "CFXTrackItemParticles";
        private const string TypeCParticleSystem = "CParticleSystem";
        private const string TypeSharedDataBuffer = "SharedDataBuffer";

        private const string VariableNameBuffer = "buffer";
        private const string VariableNameCookedEffects = "cookedEffects";
        private const string VariableNameShowDistance = "showDistance";
        
        private const string PathBundle = "Bundle";
        private const string PathDLC = "dlc";

        private const float ValueShowDistanceIDD = 800;

        public W2EntFilePatcher(ILocalizedStringSource localizedStringSource) : base(localizedStringSource)
        {
        }

        public void PatchForIncreasedDrawDistance(string filePath, Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap)
        {
            CR2WFile w2EntFile = ReadW2EntFile(filePath, localizedStringSource);
            List<SharedDataBuffer> sharedDataBuffersForFires = ReadSharedDataBuffersForFires(w2EntFile);

            if (sharedDataBuffersForFires == null || !sharedDataBuffersForFires.Any())
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no shared data buffer.");
            }

            foreach (SharedDataBuffer sharedDataBufferForFire in sharedDataBuffersForFires) {

                PatchFireShowDistance(filePath, sharedDataBufferForFire);
                PatchW2PFilePath(filePath, sharedDataBufferForFire, relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap);

                WriteSharedDataBuffer(sharedDataBufferForFire);
            }

            // TODO probably get rid of the 2nd arg
            WriteCR2WFile(w2EntFile, w2EntFile.FileName);
        }

        internal static CR2WFile ReadW2EntFile(string filePath, ILocalizedStringSource localizedStringSource)
        {
            return ReadCR2WFile(filePath, localizedStringSource);
        }

        internal static List<SharedDataBuffer> ReadSharedDataBuffersForFires(CR2WFile w2EntFile)
        {
            List<SharedDataBuffer> sharedDataBuffersForFires = null;

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
                }
            }

            return sharedDataBuffersForFires;
        }

        private static List<SharedDataBuffer> GetSharedDataBuffersForFiresFromCookedEffectsVariable(CVariable variableCookedEffects, string filePath, ILocalizedStringSource localizedStringSource)
        {
            List<SharedDataBuffer> sharedDataBuffersForFires = new List<SharedDataBuffer>();

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
                        CR2WFile sharedDataBufferContent = ReadSharedDataBufferContent((CByteArray)cookedEffectsVariables[1], localizedStringSource);

                        if (sharedDataBufferContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains a shared data buffer which could not be read.");
                        }

                        sharedDataBuffersForFires.Add(new SharedDataBuffer(sharedDataBufferContent, (CByteArray)cookedEffectsVariables[1]));
                    }
                }
            }

            return sharedDataBuffersForFires;
        }

        private static void PatchFireShowDistance(string filePath, SharedDataBuffer sharedDataBufferForFire)
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

        private static void PatchW2PFilePath(string w2EntFilePath, SharedDataBuffer sharedDataBuffer, Dictionary<string, string> relativeOriginalW2PFilePathToRelativeRenamedW2PFilePathMap)
        {
            bool cFXTrackItemParticlesFound = false;

            foreach (CR2WChunk chunk in sharedDataBuffer.Content.chunks)
            {
                if (!IsCFXTrackItemParticles(chunk))
                {
                    continue;
                }

                cFXTrackItemParticlesFound = true;

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

            if (!cFXTrackItemParticlesFound)
            {
                throw new System.InvalidOperationException("File '" + w2EntFilePath + "' contains no chunk of type '" + TypeCFXTrackItemParticles + "'.");
            }
        }

        private static void PatchW2PFilePath(CSoft variableCSoftParticleSystem, string relativeRenamedW2PFilePath)
        {
            variableCSoftParticleSystem.Handle = relativeRenamedW2PFilePath;
        }

        internal static List<string> GetW2PFilePathsForFires(SharedDataBuffer sharedDataBuffer, string w2EntFilePath, string modDirectory, string dlcDirectory, ILocalizedStringSource localizedStringSource)
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
                        string initialPath = relativeW2PFilePath.StartsWith(PathDLC) ? dlcDirectory : modDirectory;

                        string absoluteW2PFilePath = initialPath + Path.DirectorySeparatorChar + PathBundle + Path.DirectorySeparatorChar + relativeW2PFilePath;
                        string w2pFileName = relativeW2PFilePath.Substring(relativeW2PFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        if ((w2pFileName.Contains(LabelFire) || w2pFileName.Contains(LabelFlame) || w2pFileName.Contains("_candle") || w2pFileName.Contains("_brazier"))
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

        private static bool IsFire(CVariable variable)
        {
            return variable is CName && ((CName)variable).Value.Contains(LabelFire);
        }

        private static bool IsSharedDataBuffer(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(VariableNameBuffer) && variable.Type.Equals(TypeSharedDataBuffer);
        }

        private static bool IsShowDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameShowDistance);
        }
    }
}
