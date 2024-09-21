using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.MouseCapture
{
    internal interface IMouseCapture : IModule
    {
        internal abstract bool MouseCaptured(int px, int py, Element e);
    }
}
