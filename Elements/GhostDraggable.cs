using GNSUsingCS.Elements.Modules.Recalculate;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal class GhostDraggable : Element // could also make a ghost scale...
    {
        internal static GhostDraggable Instance = null;
        private bool ChangedCalc = false;
        /* -- switch to modules + should draw on top of layer, not after element.
        internal override void Draw()
        {
            base.Draw();

            if (!_grapped)
                return; // draw ghost if its grabbed

            ChangedCalc = true;
            Recalculate();
            ScissorManager.GodScissor(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);


            DrawElement();
            DrawChildren();

            DrawRectangle(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H, new Color(0, 0, 0, 100));

            ScissorManager.ExitScissor();

            ChangedCalc = false;
            Recalculate();
        }
        */

        private OffsetRecalculate offsetRecalculate;
        public GhostDraggable()
        {
            offsetRecalculate = new OffsetRecalculate(new DefaultRecalculate());
            RecalculateModule = offsetRecalculate;
        }


        private bool _grapped = false;
        /* -- keep the idea while transitioning to modules
        internal override void Recalculate(int x, int y, int w, int h)
        {
            if (ChangedCalc)
                base.Recalculate((int)_offset.X + x, (int)_offset.Y + y, w, h);
            else
                base.Recalculate(x, y, w, h);

            parentSize = [x, y, w, h];
        }
        */

        internal override void UpdateElement()
        {
            if (_grapped && MouseManager.MouseVelocity.Length() > 0.01)
            {
                offsetRecalculate.Offset += MouseManager.MouseVelocity;
                Recalculate();
            }

            if (!_grapped && Instance is null && IsHovered && IsMouseButtonDown(MouseButton.Left))
            {
                Instance = this;
                _grapped = true;
                offsetRecalculate.Offset = new();
            }
            if (!IsMouseButtonDown(MouseButton.Left))
            {
                Instance = null;
                _grapped = false;
            }
        }
    }
}
