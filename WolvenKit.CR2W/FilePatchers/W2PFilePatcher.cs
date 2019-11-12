using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.FilePatchers;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.BatchProcessors
{
    public sealed class W2AFilePatcher : W2XFilePatcher
    {
        private const string TypeCParticleEmitter = "CParticleEmitter";
        private const string TypeCParticleSystem = "CParticleSystem";

        private const string VariableNameDistance = "distance";
        private const string VariableNameEditorName = "editorName";
        private const string VariableNameLODs = "lods";

        public W2AFilePatcher(ILocalizedStringSource localizedStringSource) : base(localizedStringSource)
        {
        }

        public void PatchForIncreasedDrawDistance(string filePath, W2PSettings w2PSettings)
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

                        PatchAutoHideDistance(variable, w2PSettings.AutoHideDistance);

                        autoHideDistanceFound = true;
                    }
                    else if (IsLODs(variable))
                    {
                        PatchLODs(variable, w2PSettings);
                    }
                }

                if (!autoHideDistanceFound)
                {
                    AddAutoHideDistance(w2PFile, chunkData, w2PSettings.AutoHideDistance);
                }
            }

            if (!cParticleSystemFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + TypeCParticleSystem + "'.");
            }

            WriteCR2WFile(w2PFile);
        }

        private static void PatchLODs(CVariable variableLODs, W2PSettings w2PSettings)
        {
            if (w2PSettings.LOD1 == null || w2PSettings.LODIncrement == null)
            {
                return;
            }

            float valueLODIDD = w2PSettings.LOD1.Value;

            foreach (CVariable lodEntry in ((CArray)variableLODs).array)
            {
                if (lodEntry is CVector && ((CVector)lodEntry).variables != null)
                {
                    foreach (CVariable lodVariable in ((CVector)lodEntry).variables)
                    {
                        if (lodVariable is CFloat && ((CFloat)lodVariable).Name.Equals(VariableNameDistance))
                        {
                            ((CFloat)lodVariable).SetValue(valueLODIDD);

                            valueLODIDD += w2PSettings.LODIncrement.Value;
                        }
                    }
                }
            }
        }

        // TODO remove if it turns out that it is really not required
        //internal static bool W2PFileContainsFireParticleEmitter(string w2PFilePath, ILocalizedStringSource localizedStringSource)
        //{
        //    CR2WFile file;

        //    try
        //    {
        //        // TODO check other read methods and change to this try-catch scheme
        //        file = ReadCR2WFile(w2PFilePath, localizedStringSource);
        //    }
        //    catch (Exception e)
        //    {
        //        // TODO maybe this should be logged, at least for testing purposes but this will 
        //        //throw new System.InvalidOperationException("File '" + w2PFilePath + "' could not be loaded.");
        //        return false;
        //    }

        //    foreach (CR2WChunk chunk in file.chunks)
        //    {
        //        if (!chunk.Type.Equals(TypeCParticleEmitter))
        //        {
        //            continue;
        //        }

        //        if (chunk.data == null || !(chunk.data is CVector))
        //        {
        //            throw new System.InvalidOperationException("File '" + w2PFilePath + "' contains either no or invalid chunk data for type '" + TypeCParticleEmitter + "'.");
        //        }

        //        CVector chunkData = (CVector)chunk.data;

        //        foreach (CVariable variable in chunkData.variables)
        //        {
        //            if (isEditorName(variable) && (((CString)variable).val.Contains(LabelFire) || ((CString)variable).val.Contains(LabelFlame)))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //private static bool isEditorName(CVariable variable)
        //{
        //    return variable is CString && variable.Name.Equals(VariableNameEditorName);
        //}

        private static bool IsLODs(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(VariableNameLODs);
        }
    }
}
