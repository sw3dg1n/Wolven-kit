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
        private const string typeCParticleSystem = "CParticleSystem";

        private const string variableNameAutoHideDistance = "autoHideDistance";
        private const string variableNameDistance = "distance";
        private const string variableNameLODs = "lods";

        private const float valueAutoHideDistanceIDD = 800;
        private const float valueLOD1IDD = 80;
        private const float valueIncrementLODIDD = 40;

        public W2PFilePatcher(string filePath, ILocalizedStringSource localizedStringSource) : base(filePath, localizedStringSource)
        {
        }

        public override bool PatchForIncreasedDrawDistance()
        {
            CR2WFile file = ReadCR2WFile(filePath, localizedStringSource);

            if (file == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            bool cParticleSystemFound = false;

            foreach (CR2WChunk chunk in file.chunks)
            {
                if (!chunk.Type.Equals(typeCParticleSystem))
                {
                    continue;
                }
                else if (cParticleSystemFound)
                {
                    // TODO not sure if this is a valid or invalid case
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + typeCParticleSystem + "'.");
                }

                cParticleSystemFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + typeCParticleSystem + "' and could thus not be patched.");
                }

                CVector chunkData = (CVector)chunk.data;
                bool autoHideDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (isAutoHideDistance(variable))
                    {
                        patchAutoHideDistance(variable);

                        autoHideDistanceFound = true;
                    }
                    else if (isLODs(variable))
                    {
                        patchLODs(variable);

                        // TODO not sure if LODs should be added if not present
                    }
                }

                if (!autoHideDistanceFound)
                {
                    addAutoHideDistance(file, chunkData);
                }
            }

            if (!cParticleSystemFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + typeCParticleSystem + "' and could thus not be patched.");
            }

            WriteCR2WFile(file, filePath);

            return true;
        }
        
        private static bool isAutoHideDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(variableNameAutoHideDistance);
        }

        private static bool isLODs(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(variableNameLODs);
        }

        private static void patchAutoHideDistance(CVariable variableAutoHideDistance)
        {
            ((CFloat)variableAutoHideDistance).SetValue(valueAutoHideDistanceIDD);
        }

        private static void patchLODs(CVariable variableLODs)
        {
            float valueLODIDD = valueLOD1IDD;

            foreach (CVariable lodEntry in ((CArray)variableLODs).array)
            {
                if (lodEntry is CVector && ((CVector)lodEntry).variables != null)
                {
                    foreach (CVariable lodVariable in ((CVector)lodEntry).variables)
                    {
                        if (lodVariable is CFloat && ((CFloat)lodVariable).Name.Equals(variableNameDistance))
                        {
                            ((CFloat)lodVariable).SetValue(valueLODIDD);

                            valueLODIDD += valueIncrementLODIDD;
                        }
                    }
                }
            }
        }

        private static void addAutoHideDistance(CR2WFile file, CVector chunkData)
        {
            CFloat autoHideDistanceVariable = new CFloat(file);

            autoHideDistanceVariable.Type = "Float";
            autoHideDistanceVariable.Name = variableNameAutoHideDistance;
            autoHideDistanceVariable.SetValue(valueAutoHideDistanceIDD);

            chunkData.variables.Add(autoHideDistanceVariable);
        }
    }
}
