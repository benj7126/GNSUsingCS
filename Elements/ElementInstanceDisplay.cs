using GNSUsingCS.Elements.Modules.Draw;
using GNSUsingCS.Elements.Modules.Recalculate;
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
    internal class ElementInstanceDisplay : Element
    {
        public Box box;
        private Element toDraw;

        public ElementInstanceDisplay(ElementSettingsInstance esi)
        {
            box = new Box();

            box.Dimensions.Width.Set(0, 1f);
            box.Dimensions.Height.Set(0, 1f);

            DrawModule = new ChildrenFirstDraw();

            Children.Add(box);
            toDraw = esi.CreateElementFrom();

            RecalculateModule = new FitChildElementRecalculate(RecalculateModule, toDraw);
        }

        internal override void DrawElement()
        {
            toDraw.DrawElement();
        }

        internal override void RecalculateChildren()
        {
            base.RecalculateChildren();
            toDraw.Recalculate(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H); // its scuffed but idk bro...
        }
    }
}
