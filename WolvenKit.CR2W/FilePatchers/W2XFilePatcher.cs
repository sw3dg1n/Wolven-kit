using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.BatchProcessors
{
    public abstract class W2XFilePatcher
    {
        protected readonly string filePath;
        protected readonly ILocalizedStringSource localizedStringSource;

        public W2XFilePatcher(string filePath, ILocalizedStringSource localizedStringSource)
        {
            this.filePath = filePath;
            this.localizedStringSource = localizedStringSource;
        }

        public abstract bool PatchForIncreasedDrawDistance();

        protected CR2WFile ReadCR2WFile()
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

        protected void WriteCR2WFile(CR2WFile file)
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
    }
}
