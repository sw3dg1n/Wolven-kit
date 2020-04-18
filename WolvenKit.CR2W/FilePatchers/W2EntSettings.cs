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
                else if (fileName.Contains("burning_drakkar"))
                {
                    return 450;
                }
                else if (fileName.Contains("funeral_pyre_burned"))
                {
                    return 3500;
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
                    return 2200;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_wide.w2ent"))
                {
                    return 1500;
                }
                else if (fileName.Contains("shrine_of_ethernal_fire_altar_wide_bounce"))
                {
                    return 2500;
                }
                else if (fileName.Contains("hanging_lamp") || fileName.Contains("lamp_standing"))
                {
                    return 200;
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
                else if (fileName.Contains("burning_drakkar"))
                {
                    return 450;
                }
                else if (fileName.Contains("chandelier_small.w2ent"))
                {
                    return 3000;
                }
                else if (fileName.Contains("funeral_pyre_burned"))
                {
                    return 3500;
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
                if (fileName.Contains("candle_large_group") || fileName.Contains("candle_small_group_melted_d.w2ent") || fileName.Contains("candle_small_group_melted_d_noshadow"))
                {
                    return 0.15F;
                }
                else if (fileName.Contains("coal_medium"))
                {
                    return 0.17F;
                }
                else if (fileName.Contains("candle") || fileName.Contains("campfire_") || fileName.Contains("chandelier_small") || fileName.Contains("braziers_wall_square")
                     || (fileName.Contains("lantern") && !fileName.Contains("dwarf")) || fileName.Contains("torch_hand_long") || fileName.Contains("stand_mw_cooking_big_cauldron") || fileName.Contains("candelabra_tall_three_point")
                     || fileName.Contains("chandelier_bright_noshadow") || fileName.Contains("braziers_wall_round.w2ent") || fileName.Contains("hanging_lamp.w2ent")
                     || (fileName.Contains("coal_small") && !fileName.Contains("double_spotlight.w2ent") && !fileName.Contains("smaller"))
                     || fileName.Contains("mh308_campfire") || fileName.Contains("mh103_campfire") || fileName.Contains("candelabra.w2ent"))
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
                else if (fileName.Contains("braziers_wall_square"))
                {
                    return 30;
                }
                else if (fileName.Contains("braziers_wall_wire"))
                {
                    return 80;
                }
                else if (fileName.Contains("brick_stove_round"))
                {
                    return 40;
                }
                else if (fileName.Contains("burning_drakkar"))
                {
                    return 75;
                }
                else if (fileName.StartsWith("campfire_"))
                {
                    return 15;
                }
                else if (fileName.Contains("candle") && (!fileName.Contains("holder") || fileName.Contains("small")))
                {
                    // TODO This could be even further extended for higher resolutions, supersampling or better antialiasing e.g. via reshade
                    return 15;
                }
                else if (fileName.Contains("chandelier_small.w2ent"))
                {
                    return 80;
                }
                else if (fileName.Contains("chandelier_small"))
                {
                    return 20;
                }
                else if (fileName.Contains("coal_medium"))
                {
                    return 10;
                }
                else if (fileName.Contains("funeral_pyre_burned"))
                {
                    return 250;
                }
                else if (fileName.Contains("hanging_lamp") || fileName.Contains("lamp_standing"))
                {
                    return 25;
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
                    return 45;
                }
                else if (fileName.Contains("stand_mw_cooking_roasted_pig"))
                {
                    return 45;
                }
                else if (fileName.Contains("torch_hand_long"))
                {
                    return 40;
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
                if (!fileName.Contains("hanging_lamp_complex") && !fileName.Contains("shrine_of_ethernal_fire_bounce") && !fileName.Contains("torch_wall") && !fileName.Contains("candle_small_group_melted_d_complex")
                     && !fileName.Contains("candleholder_small_melted_fake_shadow_complex") && !fileName.Contains("baron_candle_holder"))
                {
                    return true;
                }

                return false;
            }

            private set { }
        }

        public bool PatchGlowRadius
        {
            get
            {
                if (fileName.Contains("candle")
                    && !fileName.Contains("shelf")
                    && (!fileName.Contains("holder") || fileName.Contains("small")))
                {
                    return true;
                }

                return false;
            }

            private set { }
        }
    }
}
