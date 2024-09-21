using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal class DefaultScissor : IScissorModule
    {
        void IScissorModule.EnterScissor(int x, int y, int w, int h)
        {
            ScissorManager.EnterScissor(x, y, w, h);
        }
    }
}
