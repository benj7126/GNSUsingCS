using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal class ElementList : AppendableElement
    {
        internal override bool UseScissor => false;

        public int ConstantHeight = -1; // if -1 then dynamic height
        public int Margin = 0;

        private float scrollability = 0;

        public ElementList()
        {
            Dimensions.Width.Set(0, 1f);
            Dimensions.Height.Set(0, 1f);
            ShareHoverWithParent = true;
        }

        internal override void RecalculateChildren()
        {
            int height = 0;
            
            foreach (Element element in Children)
            {
                int thisHeight = ConstantHeight;

                if (thisHeight == -1)
                {
                    element.Recalculate(0, 0, 0, 0);
                    thisHeight = element.Dimensions.H;

                    if (thisHeight == 0)
                        thisHeight = Dimensions.W; // default to just make it a box if nothing works out.
                }
                
                element.Recalculate(Dimensions.X, Dimensions.Y + height, Dimensions.W, thisHeight);

                height += thisHeight + Margin;
            }

            scrollability = height - Margin;
        }

        public override void Append(Element e)
        {
            Children.Add(e);

            // Recalculate(); -- unsure if this is neccecary yet...
        }

        private float _mouseScrollVel = 0;
        internal override void UpdateElement()
        {
            int pre = Dimensions.Top.Pixels;
            _mouseScrollVel *= Settings.MouseScrollVelocityDropoff;

            if (IsHovered)
            {
                float scrollAmount = GetMouseWheelMoveV().Y * 10f;

                _mouseScrollVel += scrollAmount * Settings.MouseScrollSensitivity;
            }

            Dimensions.Top.Pixels += (int)_mouseScrollVel;

            Dimensions.Top.Pixels = (int)Math.Min(Math.Max(Dimensions.Top.Pixels, -Math.Max(scrollability - Dimensions.H, 0)), 0);

            if (Dimensions.Top.Pixels != pre)
                Recalculate();
        }
    }
}
