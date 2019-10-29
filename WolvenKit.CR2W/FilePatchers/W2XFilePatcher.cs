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

        protected const string LabelFire = "fire";
        protected const string LabelFlame = "flame";

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

        protected static void WriteCR2WFile(CR2WFile file, string filePath)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    file.Write(writer);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.WriteTo(fs);
                    }
                }
            }
        }

        protected static CR2WFile ReadSharedDataBufferContent(CByteArray sharedDataBufferByteArray, ILocalizedStringSource localizedStringSource)
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

        protected static void WriteSharedDataBuffer(SharedDataBuffer sharedDataBuffer)
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
    }
}
