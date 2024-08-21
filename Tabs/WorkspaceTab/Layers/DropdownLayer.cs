using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs.WorkspaceTab.Layers
{
    internal class DropdownLayer(ElementLayer layer) : Layer
    {

        public static (ElementLayer, Element)? Selection;


        public override void Draw(int x, int y, int w, int h)
        {
            if (!Selection.HasValue)
                return;

            Selection.Value.Item2.Draw();
        }

        public override void Update()
        {
            if (IsMouseButtonPressed(MouseButton.Right))
            {
                Selection = (layer, layer.GetRightclickPanel());
                Selection.Value.Item2.Recalculate((int)MouseManager.MousePosition.X, (int)MouseManager.MousePosition.Y, 0, 0);
            }
        }
    }
}
