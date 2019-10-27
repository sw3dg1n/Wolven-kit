using System;
using System.Collections.Generic;
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
        private const string TypeSharedDataBuffer = "SharedDataBuffer";

        private const string VariableNameBuffer = "buffer";
        private const string VariableNameCookedEffects = "cookedEffects";
        private const string VariableNameParticleSystem = "particleSystem";
        private const string VariableNameShowDistance = "showDistance";

        private const string LabelFire = "fire";
        private const string LabelFlame = "flame";

        private const float ValueShowDistanceIDD = 800;

        public W2EntFilePatcher(string filePath, ILocalizedStringSource localizedStringSource) : base(filePath, localizedStringSource)
        {
        }

        public override bool PatchForIncreasedDrawDistance()
        {
            CR2WFile w2EntFile = ReadW2EntFile(filePath, localizedStringSource);
            List<SharedDataBuffer> sharedDataBuffersForFires = ReadSharedDataBuffersForFires(w2EntFile);

            if (sharedDataBuffersForFires == null || !sharedDataBuffersForFires.Any())
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no shared data buffer.");
            }

            foreach (SharedDataBuffer sharedDataBufferForFire in sharedDataBuffersForFires) {

                PatchFireShowDistance(sharedDataBufferForFire);
                WriteSharedDataBuffer(sharedDataBufferForFire);
            }

            // TODO probably get rid of the 2nd arg
            WriteCR2WFile(w2EntFile, w2EntFile.FileName);

            return true;
        }

        public static CR2WFile ReadW2EntFile(string filePath, ILocalizedStringSource localizedStringSource)
        {
            return ReadCR2WFile(filePath, localizedStringSource);
        }

        public static List<SharedDataBuffer> ReadSharedDataBuffersForFires(CR2WFile w2EntFile)
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

        private void PatchFireShowDistance(SharedDataBuffer sharedDataBufferForFire)
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

        public static bool SharedDataBufferReferencesFireW2PFile(SharedDataBuffer sharedDataBuffer, string filePath)
        {
            foreach (CR2WChunk chunk in sharedDataBuffer.Content.chunks)
            {
                if (!IsCFXTrackItemParticles(chunk))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCFXTrackItemParticles + "'.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsCSoftParticleSystem(variable))
                    {
                        CSoft cSoftParticleSystem = (CSoft)variable;

                        if (cSoftParticleSystem.Handle.Contains(LabelFire) || cSoftParticleSystem.Handle.Contains(LabelFlame))
                        {
                            return true;
                        }

                        // TODO check w2p file
                    }
                }
            }

            return false;
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
            return variable is CSoft && ((CSoft)variable).FileType.Equals(VariableNameParticleSystem);
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
