using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2EntSettings
    {
        private readonly string fileName;

        public W2EntSettings(string fileName)
        {
            this.fileName = fileName;
        }

        public float FireShowDistance
        {
            get
            {
                return 1200;
            }

            private set { }
        }

        public float GlowAutoHideDistance
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
                if (fileName.Contains("braziers_floor"))
                {
                    // TODO This could either be reduced more to avoid the shimmering in the distance or maybe even further extended for higher resolutions, supersampling or better antialiasing e.g. via reshade
                    return 30;
                }
                //else if (fileName.Contains("braziers_wall_chain"))
                //{
                //    // TODO
                //    return 10;
                //}
                else if (fileName.Contains("braziers_wall_round"))
                {
                    return 30;
                }
                else if (fileName.Contains("braziers_wall_wire"))
                {
                    return 80;
                }
                else if (fileName.Contains("chandelier_small"))
                {
                    return 80;
                }
                else if (fileName.Contains("hanging_lamp"))
                {
                    // TODO This could still be increased if the issue with the missing glow mesh cannot be resolved and there is still enough headroom
                    return 30;
                }
                else if (fileName.Contains("shrine_of_ethernal"))
                {
                    return 100;
                }

                return null;
            }

            private set { }
        }
    }
}
