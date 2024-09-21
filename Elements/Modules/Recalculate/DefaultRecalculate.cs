using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal class DefaultRecalculate : IRecalculateModule
    {
        void IRecalculateModule.Recalculate(int x, int y, int w, int h, Element e)
        {
            e.Dimensions.Recalculate(x, y, w, h);

            e.RecalculateChildren();

            e.parentSize = [x, y, w, h];
            e.PostRecalculate(x, y, w, h);
        }
    }
}
