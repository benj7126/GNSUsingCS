using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal interface IRecalculateModule : IModule
    {
        internal abstract void Recalculate(int x, int y, int w, int h, Element e);
    }
}
