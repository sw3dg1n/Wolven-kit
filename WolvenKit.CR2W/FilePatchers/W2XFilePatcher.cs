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

        protected CR2WFile LoadCR2WFile()
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
    }
}
