using GNSUsingCS.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs.WorkspaceTab.Layers
{
    internal class DraggableNodeLayer : ElementLayer
    {
        public DraggableNodeLayer() : base([]) { }

        public override Element GetRightclickPanel()
        {
            ElementList list = new ElementList();
            for (int i = 0; i < 10; i++)
            {
                Button b = new Button();

                b.Dimensions.Width.Set(120, 0f);
                b.Dimensions.Height.Set(24, 0f);

                b.Box.Background = new Raylib_cs.Color(i * 10, i * 10, i * 10, 255);

                b.Label.Text = "A-s d: " + i;

                list.Children.Add(b);
            }

            return list;
        }
    }
}