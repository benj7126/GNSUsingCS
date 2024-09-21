using GNSUsingCS.Elements.Modules.Recalculate;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal class Label : TextContainer
    {
        public bool ScaleHeightWithFont = false; // should somehow set the height to FontSize pixels, but only during note/element editing.
                                                 // *not every frame...
        public bool UseAbsoluteMinSize = false;
        public Label() : base()
        {
            RecalculateModule = new MinSizeRecalculate(() => {
                Vector2 size = MeasureTextEx(FontManager.GetFont(FontType, FontSize), Text, FontSize, Spacing);

                if (!ScaleHeightWithFont)
                    size.Y = 0;

                if (!UseAbsoluteMinSize)
                    size.X = 0;

                return size;
            });
        }

        internal override void DrawElement()
        {
            DrawTextEx(FontManager.GetFont(FontType, FontSize), Text, new Vector2(Dimensions.X, Dimensions.Y), FontSize, Spacing, Color);
        }

        /*
        internal override void ExtraConstraints(ref Dictionary<Constraint, int> constraints)
        {
            base.ExtraConstraints(ref constraints);

            Vector2 size = MeasureTextEx(FontManager.GetFont(FontType, FontSize), Text, FontSize, Spacing);

            constraints[Constraint.MinW] = Math.Max(constraints[Constraint.MinW], (int)Math.Ceiling(size.X) + (int)(Padding.X / 2));
            constraints[Constraint.MinH] = Math.Max(constraints[Constraint.MinH], (int)Math.Ceiling(size.Y) + (int)(Padding.Y / 2));

            // constraints[Constraint.MinX] = (int)(Padding.X / 2);
            // constraints[Constraint.MinY] = (int)(Padding.Y / 2);
        }
        */
    }
}
