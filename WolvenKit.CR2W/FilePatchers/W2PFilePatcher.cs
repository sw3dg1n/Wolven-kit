using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.BatchProcessors
{
    public sealed class W2PFilePatcher : W2XFilePatcher
    {
        private const string TypeCParticleEmitter = "CParticleEmitter";
        private const string TypeCParticleSystem = "CParticleSystem";

        private const string VariableNameAutoHideDistance = "autoHideDistance";
        private const string VariableNameDistance = "distance";
        private const string VariableNameEditorName = "editorName";
        private const string VariableNameLODs = "lods";

        private const float ValueAutoHideDistanceIDD = 800;
        private const float ValueLOD1IDD = 80;
        private const float ValueIncrementLODIDD = 40;

        public W2PFilePatcher(string filePath, ILocalizedStringSource localizedStringSource) : base(filePath, localizedStringSource)
        {
        }

        public override bool PatchForIncreasedDrawDistance()
        {
            CR2WFile w2PFile = ReadCR2WFile(filePath, localizedStringSource);

            if (w2PFile == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            bool cParticleSystemFound = false;

            foreach (CR2WChunk chunk in w2PFile.chunks)
            {
                if (!chunk.Type.Equals(TypeCParticleSystem))
                {
                    continue;
                }
                else if (cParticleSystemFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + TypeCParticleSystem + "'.");
                }

                cParticleSystemFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCParticleSystem + "'.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool autoHideDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (IsAutoHideDistance(variable))
                    {
                        if (autoHideDistanceFound)
                        {
                            throw new System.InvalidOperationException("File '" + filePath + "' contains more than one attribute '" + VariableNameAutoHideDistance + "'.");
                        }

                        PatchAutoHideDistance(variable);

                        autoHideDistanceFound = true;
                    }
                    else if (IsLODs(variable))
                    {
                        PatchLODs(variable);

                        // TODO not sure if LODs should be added if not present
                    }
                }

                if (!autoHideDistanceFound)
                {
                    AddAutoHideDistance(w2PFile, chunkData);
                }
            }

            if (!cParticleSystemFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + TypeCParticleSystem + "'.");
            }

            WriteCR2WFile(w2PFile, filePath);

            return true;
        }

        private static void PatchAutoHideDistance(CVariable variableAutoHideDistance)
        {
            ((CFloat)variableAutoHideDistance).SetValue(ValueAutoHideDistanceIDD);
        }

        private static void PatchLODs(CVariable variableLODs)
        {
            float valueLODIDD = ValueLOD1IDD;

            foreach (CVariable lodEntry in ((CArray)variableLODs).array)
            {
                if (lodEntry is CVector && ((CVector)lodEntry).variables != null)
                {
                    foreach (CVariable lodVariable in ((CVector)lodEntry).variables)
                    {
                        if (lodVariable is CFloat && ((CFloat)lodVariable).Name.Equals(VariableNameDistance))
                        {
                            ((CFloat)lodVariable).SetValue(valueLODIDD);

                            valueLODIDD += ValueIncrementLODIDD;
                        }
                    }
                }
            }
        }

        private static void AddAutoHideDistance(CR2WFile file, CVector chunkData)
        {
            CFloat autoHideDistanceVariable = new CFloat(file);

            autoHideDistanceVariable.Type = CVariableTypeFloat;
            autoHideDistanceVariable.Name = VariableNameAutoHideDistance;
            autoHideDistanceVariable.SetValue(ValueAutoHideDistanceIDD);

            chunkData.variables.Add(autoHideDistanceVariable);
        }

        internal static bool W2PFileContainsFireParticleEmitter(string w2PFilePath, ILocalizedStringSource localizedStringSource)
        {
            CR2WFile file;

            try
            {
                // TODO check other read methods and change to this try-catch scheme
                file = ReadCR2WFile(w2PFilePath, localizedStringSource);
            }
            catch (Exception e)
            {
                // TODO maybe this should be logged, at least for testing purposes but this will 
                //throw new System.InvalidOperationException("File '" + w2PFilePath + "' could not be loaded.");
                return false;
            }

            foreach (CR2WChunk chunk in file.chunks)
            {
                if (!chunk.Type.Equals(TypeCParticleEmitter))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + w2PFilePath + "' contains either no or invalid chunk data for type '" + TypeCParticleEmitter + "'.");
                }

                CVector chunkData = (CVector)chunk.data;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (isEditorName(variable) && (((CString)variable).val.Contains(LabelFire) || ((CString)variable).val.Contains(LabelFlame)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsAutoHideDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameAutoHideDistance);
        }

        private static bool isEditorName(CVariable variable)
        {
            return variable is CString && variable.Name.Equals(VariableNameEditorName);
        }

        private static bool IsLODs(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(VariableNameLODs);
        }
    }
}
