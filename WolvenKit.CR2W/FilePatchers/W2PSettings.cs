using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2PSettings : W2XSettings
    {
        public float LOD1 { get; private set; }
        public float LODIncrement { get; private set; }

        public W2PSettings(float autoHideDistance, float lod1, float lodIncrement) : base(autoHideDistance)
        {
            LOD1 = lod1;
            LODIncrement = lodIncrement;
        }
    }
}
