using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Recalculate
{
    internal class OffsetRecalculate(IRecalculateModule baseRecaluclate) : IRecalculateModule
    {
        public Vector2 Offset = new();
        public bool Active = false;
        void IRecalculateModule.Recalculate(int x, int y, int w, int h, Element e)
        {
            if (Active)
                baseRecaluclate.Recalculate((int)Offset.X + x, (int)Offset.Y + y, w, h, e);
            else
                baseRecaluclate.Recalculate(x, y, w, h, e);

            e.parentSize = [x, y, w, h];
        }
    }
}
