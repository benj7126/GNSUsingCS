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
        public Label Label;
        public Box Box;

        public override List<Element> Children => [Label, Box];

        internal Button()
        {
            Label = new Label();
            Label.Dimensions.Width.Set(0, 1);
            Label.Dimensions.Height.Set(0, 1);

            Box = new Box();
            Box.Dimensions.Width.Set(0, 1);
            Box.Dimensions.Height.Set(0, 1);
        }

        protected override void DrawElement()
        {
            Box.Draw();
            Label.Draw();
        }
        internal override void Update()
        {
            // Console.WriteLine("Exists");

            if (IsHovered && IsMouseButtonPressed(MouseButton.Left))
            {
                LuaInterfacer.TryCallMethod("OnPress");

                WorkspaceTab t = new();
                SaveAndLoadManager.SaveTab(t);
                SaveAndLoadManager.LoadTab(t.UUID);
            }
        }
    }
}
