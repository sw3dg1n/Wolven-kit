using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenKit.CR2W.Types;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class SharedDataBuffer
    {
        public CR2WFile Content { get; private set; }
        public CByteArray ByteArray { get; private set; }

        public SharedDataBuffer(CR2WFile content, CByteArray byteArray)
        {
            Content = content;
            ByteArray = byteArray;
        }
    }
}
