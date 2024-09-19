using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GNSUsingCS.Elements
{
    internal class ElementInstanceDisplay : ReverseDrawElement
    {
        public Box box;
        private Element toDraw;

        public ElementInstanceDisplay(ElementSettingsInstance esi)
        {
            box = new Box();

            box.Dimensions.Width.Set(0, 1f);
            box.Dimensions.Height.Set(0, 1f);

            Children.Add(box);
            toDraw = esi.CreateElementFrom();

            Children.Add(toDraw);
        }

        internal override void Recalculate(int x, int y, int w, int h)
        {
            Dimensions.Width.Set(0, 1f);
            Dimensions.Height.Set(0, 1f);

            base.Recalculate(x, y, w, h);

            Dimensions.Width.Set(toDraw.Dimensions.W, 0f);
            Dimensions.Height.Set(toDraw.Dimensions.H, 0f);
            Dimensions.Left.Set(0, 0.5f);
            Dimensions.Top.Set(0, 0.5f);

            Dimensions.HAlign = 0.5f;
            Dimensions.VAlign = 0.5f;

            base.Recalculate(x, y, w, h); // make this scale to toDraw element
        }

        internal override void Draw()
        {
            base.Draw();
        }
    }
}
