using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal class FitChildElementRecalculate(IRecalculateModule baseRecaluclate, Element toFit) : IRecalculateModule
    {
        void IRecalculateModule.Recalculate(int x, int y, int w, int h, Element e)
        {
            e.Dimensions.Width.Set(0, 1f);
            e.Dimensions.Height.Set(0, 1f);

            baseRecaluclate.Recalculate(x, y, w, h, e);

            e.Dimensions.Width.Set(toFit.Dimensions.W, 0f);
            e.Dimensions.Height.Set(toFit.Dimensions.H, 0f);
            e.Dimensions.Left.Set(0, 0.5f);
            e.Dimensions.Top.Set(0, 0.5f);

            e.Dimensions.HAlign = 0.5f;
            e.Dimensions.VAlign = 0.5f;

            baseRecaluclate.Recalculate(x, y, w, h, e); // make this scale to toDraw element
        }
    }
}
