using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenKit.CR2W.FilePatchers
{
    public abstract class W2XSettings
    {
        public float AutoHideDistance { get; private set; }

        public W2XSettings(float autoHideDistance)
        {
            AutoHideDistance = autoHideDistance;
        }
    }
}
