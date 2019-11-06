using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.BatchProcessors;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2MeshFilePatcher : W2XFilePatcher
    {
        private const string TypeCMesh = "CMesh";

        private const string VariableNameCookedData = "cookedData";
        private const string VariableNameRenderedLODs = "renderedLODs";

        private const float ValueLOD1IDD = 80;
        private const float ValueIncrementLODIDD = 40;

        public W2MeshFilePatcher(ILocalizedStringSource localizedStringSource) : base(localizedStringSource)
        {
        }

        public void PatchForIncreasedDrawDistance(string filePath)
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

                        PatchAutoHideDistance(variable);

                        autoHideDistanceFound = true;
                    }
                    else if (IsCookedData(variable))
                    {
                        PatchLODs((CVector) variable);
                    }
                }

                if (!autoHideDistanceFound)
                {
                    AddAutoHideDistance(w2MeshFile, chunkData);
                }
            }

            if (!cMeshFound)
            {
                throw new System.InvalidOperationException("File '" + filePath + "' contains no chunk of type '" + TypeCMesh + "'.");
            }

            WriteCR2WFile(w2MeshFile);
        }

        private static void PatchLODs(CVector variableCookedData)
        {
            float valueLODIDD = ValueLOD1IDD;

            foreach (CVariable cookedDataEntry in variableCookedData.variables)
            {
                if (isRenderedLODs(cookedDataEntry) && ((CArray)cookedDataEntry).array != null && ((CArray)cookedDataEntry).array.Count > 1)
                {
                    CArray renderedLODs = (CArray)cookedDataEntry;

                    for (int i = 1; i < renderedLODs.array.Count; i++)
                    {
                        ((CFloat)renderedLODs.ElementAt(i)).SetValue(valueLODIDD);

                        valueLODIDD += ValueIncrementLODIDD;
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
            return variable is CArray && variable.Name.Equals(VariableNameRenderedLODs);
        }
    }
}
