using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal class MinSizeRecalculate(Func<Vector2> minSize) : IRecalculateModule
    {
        void IRecalculateModule.Recalculate(int x, int y, int w, int h, Element e)
        {
            e.Dimensions.Recalculate(x, y, w, h);

            Vector2 minElmSize = minSize();
            if (e.Dimensions.W < minElmSize.X || e.Dimensions.H < minElmSize.Y)
            {
                if (e.Dimensions.W < minElmSize.X)
                    e.Dimensions.Width.Pixels += (int)minElmSize.X - e.Dimensions.W;
                if (e.Dimensions.H < minElmSize.Y)
                    e.Dimensions.Height.Pixels += (int)minElmSize.Y - e.Dimensions.H;

                e.Dimensions.Recalculate(x, y, w, h);
            }

            e.RecalculateChildren();

            e.parentSize = [x, y, w, h];
            e.PostRecalculate(x, y, w, h);
        }
    }
}
