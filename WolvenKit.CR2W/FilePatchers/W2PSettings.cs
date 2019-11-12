using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2PSettings
    {
        private readonly string fileName;

        public W2PSettings(string fileName)
        {
            this.fileName = fileName;
        }

        public float AutoHideDistance
        {
            get
            {
                return 1200;
            }

            private set { }
        }

        public float? LOD1
        {
            get
            {
                return null;
            }

            private set { }
        }

        public float? LODIncrement
        {
            get
            {
                return null;
            }

            private set { }
        }
    }
}
