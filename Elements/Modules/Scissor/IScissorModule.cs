using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal interface IScissorModule : IModule
    {
        internal abstract void EnterScissor(int x, int y, int w, int h);
    }
}
