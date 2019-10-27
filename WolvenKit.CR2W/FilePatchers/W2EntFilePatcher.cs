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
        private const string typeCEntityTemplate = "CEntityTemplate";
        private const string typeCFXDefinition = "CFXDefinition";
        private const string typeSharedDataBuffer = "SharedDataBuffer";

        private const string variableNameBuffer = "buffer";
        private const string variableNameCookedEffects = "cookedEffects";
        private const string variableNamePrefixFire = "fire";
        private const string variableNameShowDistance = "showDistance";

        private const float valueShowDistanceIDD = 800;

        public W2EntFilePatcher(string filePath, ILocalizedStringSource localizedStringSource) : base(filePath, localizedStringSource)
        {
        }

        public override bool PatchForIncreasedDrawDistance()
        {
            CR2WFile file = ReadCR2WFile(filePath, localizedStringSource);

            if (file == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            bool cEntityTemplateFound = false;

            foreach (CR2WChunk chunk in file.chunks)
            {
                if (!isCEntityTemplate(chunk))
                {
                    continue;
                }
                else if (cEntityTemplateFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + typeCEntityTemplate + "'.");
                }

                cEntityTemplateFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + typeCEntityTemplate + "' and could thus not be patched.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (isCookedEffects(variable))
                    {
                        patchFireShowDistance(variable);
                    }
                }
            }

            if (!cEntityTemplateFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + typeCEntityTemplate + "' and could thus not be patched.");
            }

            WriteCR2WFile(file, filePath);

            return true;
        }

        private static bool isCEntityTemplate(CR2WChunk chunk)
        {
            return chunk.Type.Equals(typeCEntityTemplate);
        }

        private static bool isCFXDefinition(CR2WChunk chunk)
        {
            return chunk.Type.Equals(typeCFXDefinition);
        }

        private static bool isCookedEffects(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(variableNameCookedEffects);
        }

        private static bool isFire(CVariable variable)
        {
            return variable is CName && ((CName)variable).Value.StartsWith(variableNamePrefixFire);
        }

        private static bool isSharedDataBuffer(CVariable variable)
        {
            return variable is CByteArray && variable.Name.Equals(variableNameBuffer) && variable.Type.Equals(typeSharedDataBuffer);
        }

        private static bool isShowDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(variableNameShowDistance);
        }

        private void patchFireShowDistance(CVariable variableCookedEffects)
        {
            foreach (CVariable cookedEffectsEntry in ((CArray)variableCookedEffects).array)
            {
                if (cookedEffectsEntry is CVector)
                {
                    List<CVariable> cookedEffectsVariables = ((CVector)cookedEffectsEntry).variables;

                    if (cookedEffectsVariables.Count < 2)
                    {
                        throw new System.InvalidOperationException("File '" + filePath + "' contains a '" + variableNameCookedEffects + "' entry with less than 2 variables and could thus not be patched.");
                    }

                    if (isFire(cookedEffectsVariables[0]) && isSharedDataBuffer(cookedEffectsVariables[1]))
                    {
                        CR2WFile sharedDataBufferContent = ReadSharedDataBuffer((CByteArray)cookedEffectsVariables[1], localizedStringSource);

                        if (sharedDataBufferContent == null)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains a shared data buffer which could not be read and could thus not be patched.");
                        }

                        patchFireShowDistanceInSharedDataBuffer(sharedDataBufferContent, (CByteArray)cookedEffectsVariables[1]);
                    }
                }
            }
        }

        private void patchFireShowDistanceInSharedDataBuffer(CR2WFile sharedDataBufferContent, CByteArray sharedDataBufferByteArray)
        {
            bool cFXDefinitionFound = false;

            foreach (CR2WChunk chunk in sharedDataBufferContent.chunks)
            {
                if (!isCFXDefinition(chunk))
                {
                    continue;
                }
                else if (cFXDefinitionFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + typeCFXDefinition + "'.");
                }

                cFXDefinitionFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + typeCFXDefinition + "' and could thus not be patched.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool showDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (isShowDistance(variable))
                    {
                        if (showDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one attribute '" + variableNameShowDistance + "' and could thus not be patched.");
                        }

                        patchShowDistance(variable);

                        showDistanceFound = true;
                    }
                }

                if (!showDistanceFound)
                {
                    addShowDistance(sharedDataBufferContent, chunkData);
                }
            }

            if (!cFXDefinitionFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + typeCFXDefinition + "' and could thus not be patched.");
            }

            WriteSharedDataBuffer(sharedDataBufferContent, sharedDataBufferByteArray);
        }

        private static void patchShowDistance(CVariable variableShowDistance)
        {
            ((CFloat)variableShowDistance).SetValue(valueShowDistanceIDD);
        }

        private static void addShowDistance(CR2WFile file, CVector chunkData)
        {
            CFloat showDistanceVariable = new CFloat(file);

            showDistanceVariable.Type = "Float";
            showDistanceVariable.Name = variableNameShowDistance;
            showDistanceVariable.SetValue(valueShowDistanceIDD);

            chunkData.variables.Add(showDistanceVariable);
        }
    }
}
