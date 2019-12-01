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
                else if (fileName.Contains("lighthouse_fire"))
                {
                    return 3000;
                }
                else if (fileName.Contains("pile_of_bodies"))
                {
                    return 1200;
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

                return null;
            }

            private set { }
        }
    }
}
