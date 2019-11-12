using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2EntSettings : W2XSettings
    {
        private readonly string fileName;
        private readonly string filePath;

        public W2EntSettings(string fileName, string filePath, float glowAutoHideDistance) : base(glowAutoHideDistance)
        {
            this.fileName = fileName;
            this.filePath = filePath;
        }

        public float FireShowDistance
        {
            get
            {
                return 1200;
            }

            private set { }
        }
        public float? MinimumGlowTransformY
        {
            get
            {
                // TODO maybe the campfires should be restricted further, so far only campfire_01.w2ent was observed to have issues
                if (fileName.Contains("candle") || fileName.Contains("campfire_") || fileName.Contains("chandelier_small") || fileName.Contains("coal_small_noshadow"))
                {
                    return 0.01F;
                }

                return null;
            }

            private set { }
        }
        public byte? MeshStreamingDistance
        {
            get
            {
                if (fileName.Contains("shrine_of_ethernal"))
                {
                    return 100;
                }
                else if (fileName.Contains("hanging_lamp"))
                {
                    // TODO this could still be increased if the issue with the missing glow mesh cannot be resolved and there is still enough headroom
                    return 30;
                }

                return null;
            }

            private set { }
        }
    }
}
