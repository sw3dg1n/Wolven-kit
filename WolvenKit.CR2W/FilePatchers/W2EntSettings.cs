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
                if (fileName.Contains("bonfire_large"))
                {
                    return 4300;
                }
                else if (fileName.Contains("bonfire_medium"))
                {
                    return 2500;
                }
                else if (fileName.Contains("lighthouse_fire"))
                {
                    return 2500;
                }
                else if (fileName.Contains("pile_of_bodies"))
                {
                    return 1500;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_spot"))
                {
                    return 2000;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_wide.w2ent"))
                {
                    return 1500;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_wide_bounce"))
                {
                    return 2500;
                }

                return 1200;
            }

            private set { }
        }

        public float GlowAutoHideDistance
        {
            get
            {
                if (fileName.Contains("bonfire_large"))
                {
                    return 4300;
                }
                else if (fileName.Contains("bonfire_medium"))
                {
                    return 2500;
                }
                else if (fileName.Contains("chandelier_small.w2ent"))
                {
                    return 3000;
                }
                else if (fileName.Contains("lighthouse_fire"))
                {
                    return 4000;
                }
                else if (fileName.Contains("pile_of_bodies"))
                {
                    return 1500;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_spot"))
                {
                    return 3500;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_wide"))
                {
                    return 4000;
                }

                return 1200;
            }

            private set { }
        }

        public float? MinimumGlowTransformY
        {
            get
            {
                if (fileName.Contains("candle_large_group"))
                {
                    return 0.15F;
                }
                else if (fileName.Contains("candle") || fileName.Contains("campfire_") || fileName.Contains("chandelier_small") || fileName.Contains("coal_small_noshadow") || fileName.Contains("braziers_wall_square")
                     || (fileName.Contains("lantern") && !fileName.Contains("dwarf")) || fileName.Contains("torch_hand_long") || fileName.Contains("stand_mw_cooking_big_cauldron") || fileName.Contains("candelabra_tall_three_point.w2ent")
                     || fileName.Contains("chandelier_bright_noshadow") || fileName.Contains("candelabra_tall_three_point_complex"))
                {
                    return 0.01F;
                }
                else if (fileName.Contains("ob_forge_px"))
                {
                    return 0.7F;
                }

                return null;
            }

            private set { }
        }

        public byte? MeshStreamingDistance
        {
            get
            {
                if (fileName.Contains("bonfire_large"))
                {
                    return 255;
                }
                else if (fileName.Contains("bonfire_medium"))
                {
                    return 70;
                }
                else if (fileName.Contains("braziers_floor"))
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
                else if (fileName.Contains("candle") && (!fileName.Contains("holder") || fileName.Contains("small")))
                {
                    // TODO This could either be even further extended for higher resolutions, supersampling or better antialiasing e.g. via reshade
                    return 15;
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
                    return 30;
                }
                else if (fileName.Contains("lighthouse_fire") || fileName.Contains("mh207_lighthouse_brazier"))
                {
                    return 150;
                }
                else if (fileName.Contains("ob_forge_px"))
                {
                    return 30;
                }
                else if (fileName.Contains("pile_of_bodies"))
                {
                    return 70;
                }
                else if (fileName.Contains("shrine_of_ethernal") && !fileName.Contains("small"))
                {
                    return 100;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_small_spot_bounce"))
                {
                    return 30;
                }
                else if (fileName.Contains("stand_mw_cooking_big_cauldron"))
                {
                    return 50;
                }
                else if (fileName.Contains("torch_wall"))
                {
                    return 15;
                }

                return null;
            }

            private set { }
        }

        public bool PatchBrightness
        {
            get
            {
                if (!fileName.Contains("hanging_lamp_complex") && !fileName.Contains("shrine_of_ethernal_fire_bounce"))
                {
                    return true;
                }

                return false;
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
