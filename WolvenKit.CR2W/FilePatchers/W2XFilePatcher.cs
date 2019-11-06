using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.FilePatchers;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.BatchProcessors
{
    public abstract class W2XFilePatcher
    {
        protected const string CVariableTypeFloat = "Float";

        protected const string VariableNameAutoHideDistance = "autoHideDistance";

        protected const string LabelFire = "fire";
        protected const string LabelFlame = "flame";

        protected const float ValueAutoHideDistanceIDD = 1200;

        protected readonly ILocalizedStringSource localizedStringSource;

        public W2XFilePatcher(ILocalizedStringSource localizedStringSource)
        {
            this.localizedStringSource = localizedStringSource;
        }

        protected static CR2WFile ReadCR2WFile(string filePath, ILocalizedStringSource localizedStringSource)
        {
            CR2WFile file = null;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = new CR2WFile(reader)
                    {
                        FileName = filePath,
                        LocalizedStringSource = localizedStringSource
                    };
                }
            }

            return file;
        }

        protected static void WriteCR2WFile(CR2WFile file)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    file.Write(writer);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var fs = new FileStream(file.FileName, FileMode.Create, FileAccess.Write))
                    {
                        stream.WriteTo(fs);
                    }
                }
            }
        }

        protected static CR2WFile ReadCByteArrayContainerContent(CByteArray sharedDataBufferByteArray, ILocalizedStringSource localizedStringSource)
        {
            CR2WFile file = null;

            using (var stream = new MemoryStream(sharedDataBufferByteArray.Bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = new CR2WFile(reader)
                    {
                        FileName = sharedDataBufferByteArray.cr2w.FileName + ":" + sharedDataBufferByteArray.FullName,
                        LocalizedStringSource = localizedStringSource
                    };
                }
            }

            return file;
        }

        protected static void WriteCByteArrayContainer(CByteArrayContainer sharedDataBuffer)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    sharedDataBuffer.Content.Write(writer);
                }

                sharedDataBuffer.ByteArray.SetValue(stream.ToArray());
            }
        }

        protected static bool IsAutoHideDistance(CVariable variable)
        {
            return variable is CFloat && variable.Name.Equals(VariableNameAutoHideDistance);
        }

        protected static void PatchAutoHideDistance(CVariable variableAutoHideDistance)
        {
            ((CFloat)variableAutoHideDistance).SetValue(ValueAutoHideDistanceIDD);
        }

        protected static void AddAutoHideDistance(CR2WFile file, CVector chunkData)
        {
            CFloat autoHideDistanceVariable = new CFloat(file);

            autoHideDistanceVariable.Type = CVariableTypeFloat;
            autoHideDistanceVariable.Name = VariableNameAutoHideDistance;
            autoHideDistanceVariable.SetValue(ValueAutoHideDistanceIDD);

            chunkData.variables.Add(autoHideDistanceVariable);
        }
    }
}
