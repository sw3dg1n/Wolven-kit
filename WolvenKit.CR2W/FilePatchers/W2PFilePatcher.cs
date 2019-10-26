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
        private const string CParticleSystemLabel = "CParticleSystem";
        private const string AutoHideDistanceLabel = "autoHideDistance";

        private const float AutoHideDistanceIDDValue = 800;

        public W2PFilePatcher(string filePath, ILocalizedStringSource localizedStringSource) : base(filePath, localizedStringSource)
        {
        }

        public override bool PatchForIncreasedDrawDistance()
        {
            CR2WFile file = ReadCR2WFile();

            if (file == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            foreach (CR2WChunk chunk in file.chunks)
            {
                if (!chunk.Type.Equals(CParticleSystemLabel))
                {
                    continue;
                }

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + CParticleSystemLabel + "' and could thus not be patched.");
                }

                CVector chunkData = (CVector) chunk.data;
                bool autoHideDistanceFound = false;

                foreach (CVariable variable in chunkData.variables)
                {
                    if (variable is CFloat && variable.Name.Equals(AutoHideDistanceLabel))
                    {
                        ((CFloat) variable).SetValue(AutoHideDistanceIDDValue);

                        autoHideDistanceFound = true;
                    }
                }

                if (!autoHideDistanceFound)
                {
                    CFloat autoHideDistanceVariable = new CFloat(file);

                    autoHideDistanceVariable.Type = "Float";
                    autoHideDistanceVariable.Name = AutoHideDistanceLabel;
                    autoHideDistanceVariable.SetValue(AutoHideDistanceIDDValue);

                    chunkData.variables.Add(autoHideDistanceVariable);
                }
            }

            WriteCR2WFile(file);

            return true;
        }
    }
}
