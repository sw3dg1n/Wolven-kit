using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2EntSettings : W2XSettings
    {
        public float FireShowDistance { get; private set; }
        public float MinimumGlowTransformY { get; private set; }
        public byte MinimumMeshStreamingDistance { get; private set; }

        public W2EntSettings(float autoHideDistance, float fireShowDistance, float minimumGlowTransformY, byte minimumMeshStreamingDistance) : base(autoHideDistance)
        {
            FireShowDistance = fireShowDistance;
            MinimumGlowTransformY = minimumGlowTransformY;
            MinimumMeshStreamingDistance = minimumMeshStreamingDistance;
        }
    }
}
