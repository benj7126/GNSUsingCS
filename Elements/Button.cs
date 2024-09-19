using GNSUsingCS.ConfigAttributes;
using GNSUsingCS.Tabs.WorkspaceTab;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal class Button : Element
    {
        [ConfigAttributes.SubElement]
        public Label Label;
        [ConfigAttributes.SubElement]
        public Box Box;

        public Button()
        {
            Label = new Label();
            Label.Dimensions.Width.Set(0, 1);
            Label.Dimensions.Height.Set(0, 1);

            Box = new Box();
            Box.Dimensions.Width.Set(0, 1);
            Box.Dimensions.Height.Set(0, 1);

            Children.Add(Box);
            Children.Add(Label);
        }

        internal override void UpdateElement()
        {
            // Console.WriteLine("Exists");

            if (IsHovered && IsMouseButtonPressed(MouseButton.Left))
            {
                ActivateMethod("OnPress");
            }
        }
    }
}
