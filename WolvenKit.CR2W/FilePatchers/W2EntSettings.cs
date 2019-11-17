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
                if (fileName.Contains("candle") || fileName.Contains("campfire_") || fileName.Contains("chandelier_small") || fileName.Contains("coal_small_noshadow") || fileName.Contains("braziers_wall_square")
                     || (fileName.Contains("lantern") && !fileName.Contains("dwarf"))/*|| fileName.Contains("hanging_lamp") || fileName.Contains("lighthouse_fire")*/)
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
                else if (fileName.Contains("braziers_wall_chain"))
                {
                    return 50;
                }
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
                    // TODO This could still be increased if the issue with the missing "glow" mesh cannot be resolved and there is still enough headroom
                    return 30;
                }
                else if (fileName.Contains("lantern") && !fileName.Contains("dwarf"))
                {
                    return 20;
                }
                // TODO tweak further
                //else if (fileName.Contains("candle") && (!fileName.Contains("holder") || fileName.Contains("small")) && !fileName.Contains("shelf"))
                //{
                //    return 50;
                //}
                else if (fileName.Contains("shrine_of_ethernal") && !fileName.Contains("small"))
                {
                    return 100;
                }

                return null;
            }

            private set { }
        }

        public bool UnifyGlowRadius
        {
            get
            {
                if (fileName.Contains("candle") && (!fileName.Contains("holder") || fileName.Contains("small")) && !fileName.Contains("shelf"))
                {
                    return true;
                }

                return false;
            }

            private set { }
        }
    }
}
