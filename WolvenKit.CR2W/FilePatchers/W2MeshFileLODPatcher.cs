using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.BatchProcessors;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2MeshFileLODPatcher : W2XFilePatcher
    {
        private const string TypeCMesh = "CMesh";

        private const string VariableNameCookedData = "cookedData";
        private const string VariableNameRenderLODs = "renderLODs";

        private const float autoHideDistance = 2500;

        public W2MeshFileLODPatcher(ILocalizedStringSource localizedStringSource) : base(localizedStringSource)
        {
        }

        public void PatchDrawDistanceAndLOD(string filePath, W2MeshSettings w2MeshSettings)
        {
            CR2WFile w2MeshFile = ReadCR2WFile(filePath, localizedStringSource);

            if (w2MeshFile == null)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' could not be loaded.");
            }

            bool cMeshFound = false;

            foreach (CR2WChunk chunk in w2MeshFile.chunks)
            {
                if (!chunk.Type.Equals(TypeCMesh))
                {
                    continue;
                }
                else if (cMeshFound)
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains more than one chunk of type '" + TypeCMesh + "'.");
                }

                cMeshFound = true;

                if (chunk.data == null || !(chunk.data is CVector))
                {
                    throw new System.InvalidOperationException("File '" + filePath + "' contains either no or invalid chunk data for type '" + TypeCMesh + "'.");
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

                        PatchAutoHideDistance(variable, autoHideDistance);

                        autoHideDistanceFound = true;
                    }
                    else if (IsCookedData(variable))
                    {
                        PatchLODs((CVector)variable, w2MeshSettings);
                    }
                }

                if (!autoHideDistanceFound)
                {
                    AddAutoHideDistance(w2MeshFile, chunkData, autoHideDistance);
                }
            }

            if (!cMeshFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + TypeCMesh + "'.");
            }

            WriteCR2WFile(w2MeshFile);
        }

        private static void PatchLODs(CVector variableCookedData, W2MeshSettings w2MeshSettings)
        {
            float valueLOD = autoHideDistance - 1;

            foreach (CVariable cookedDataEntry in variableCookedData.variables)
            {
                if (isRenderedLODs(cookedDataEntry) && ((CArray)cookedDataEntry).array != null && ((CArray)cookedDataEntry).array.Count > 1)
                {
                    CArray renderedLODs = (CArray)cookedDataEntry;
                    int numberOfLODEntries = renderedLODs.array.Count;

                    ((CFloat)renderedLODs.ElementAt(0)).SetValue(0);

                    for (int i = numberOfLODEntries - 1; i > 0; i--)
                    {
                        ((CFloat)renderedLODs.ElementAt(i)).SetValue(valueLOD);

                        valueLOD--;
                    }

                    break;
                }
            }
        }

        private static bool IsCookedData(CVariable variable)
        {
            return variable is CVector && variable.Name.Equals(VariableNameCookedData);
        }
        private static bool isRenderedLODs(CVariable variable)
        {
            return variable is CArray && variable.Name.Equals(VariableNameRenderLODs);
        }
    }
}
