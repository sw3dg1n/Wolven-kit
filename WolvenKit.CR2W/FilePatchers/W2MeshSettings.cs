using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public sealed class W2MeshSettings
    {
        private readonly string fileName;
        private readonly float? defaultAutoHideDistance;
        private readonly float? defaultLOD1;
        private readonly float? defaultLODIncrement;

        public W2MeshSettings(float? defaultAutoHideDistance, float? defaultLOD1, float? defaultLODIncrement)
        {
            this.defaultAutoHideDistance = defaultAutoHideDistance;
            this.defaultLOD1 = defaultLOD1;
            this.defaultLODIncrement = defaultLODIncrement;
        }

        public W2MeshSettings(string fileName) : this(null, null, null)
        {
            this.fileName = fileName;
        }

        public float? AutoHideDistance
        {
            get
            {
                if (defaultAutoHideDistance != null)
                {
                    return defaultAutoHideDistance;
                }
                else if (fileName.Contains("bonfire_large"))
                {
                    return 2300;
                }
                else if (fileName.Contains("lighthouse_fire"))
                {
                    return 1500;
                }

                return 1200;
            }

            private set { }
        }

        public float? LOD1
        {
            get
            {
                if (defaultLOD1 != null)
                {
                    return defaultLOD1;
                }
                else if (fileName.Contains("braziers_wall_chain"))
                {
                    return 80;
                }

                return null;
            }

            private set { }
        }

        public float? LODIncrement
        {
            get
            {
                if (defaultLODIncrement != null)
                {
                    return defaultLODIncrement;
                }
                else if (fileName.Contains("braziers_wall_chain"))
                {
                    return 40;
                }

                return null;
            }

            private set { }
        }
    }
}
