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
                if (fileName.Contains("bonfire_large"))
                {
                    return 4300;
                }
                else if (fileName.Contains("bonfire_medium"))
                {
                    return 2500;
                }
                else if (fileName.Contains("drakkar_fire"))
                {
                    return 450;
                }
                else if (fileName.Contains("fire_funeral_alternative"))
                {
                    return 3500;
                }
                else if (fileName.Contains("h_shrine_eternal_fire_wide_materialswap")) // shrine_of_ethernal_fire_altar_wide_bounce
                {
                    return 2500;
                }
                else if (fileName.Contains("lighthouse_fire"))
                {
                    return 2500;
                }
                else if (fileName.Contains("p_fire_medium_subuv_spot_smaller")) // shrine_of_ethernal_fire_altar_spot
                {
                    return 2200;
                }
                else if (fileName.Contains("pile_corpses_fire"))
                {
                    return 1500;
                }
                else if (fileName.Contains("torch_fx3") || fileName.Contains("torch_fx4") || fileName.Contains("torch_smoke"))
                {
                    return 100;
                }
                else if (fileName.Contains("torch_fx5"))
                {
                    return 200;
                }
                else if (fileName.Contains("torch_local_effect"))
                {
                    return 300;
                }
                else if (fileName.Contains("torch"))
                {
                    return 600;
                }
                else if (fileName.Contains("candle"))
                {
                    return 100;
                }

                return 1200;
            }

            private set { }
        }

        public float? LOD1
        {
            get
            {
                if (fileName.Contains("bonfire_medium"))
                {
                    return 800;
                }
                else if (fileName.Contains("pile_of_bodies"))
                {
                    return 1000;
                }
                else if (fileName.Contains("torch_fx1"))
                {
                    return 50;
                }
                else if (fileName.Contains("shrine_eternal_fire_wide_materialswap"))
                {
                    return 120;
                }
                else if (fileName.Contains("mq1046_fire_house"))
                {
                    return 150;
                }
                else if (fileName.Contains("barons_stable_fire_4"))
                {
                    return 150;
                }
                else if (fileName.StartsWith("h_"))
                {
                    return 40;
                }

                return null;
            }

            private set { }
        }

        public float? LODIncrement
        {
            get
            {
                if (fileName.Contains("bonfire_medium"))
                {
                    return 10;
                }
                else if (fileName.Contains("pile_of_bodies"))
                {
                    return 100;
                }
                else if (fileName.Contains("torch_fx1"))
                {
                    return 10;
                }
                else if (fileName.Contains("shrine_eternal_fire_wide_materialswap"))
                {
                    return 10;
                }
                else if (fileName.StartsWith("h_"))
                {
                    return 10;
                }

                return 10;
            }

            private set { }
        }
    }
}
