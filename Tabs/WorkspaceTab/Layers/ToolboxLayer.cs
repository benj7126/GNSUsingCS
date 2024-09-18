using GNSUsingCS.Elements;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs.WorkspaceTab.Layers
{
    internal class ToolboxLayer : ElementLayer
    {
        // should be images of the elements
        List<Button> buttons = new List<Button>(); // REALLY need to 
        private Box box = new Box();
        private ElementList list;

        public ToolboxLayer(ElementLayer layer) : base([])
        {
            box.Dimensions.Width.Set(0, 1f);
            box.Dimensions.Height.Set(0, 1f);
            box.Background.A = 100;

            list = new ElementList();
            list.Margin = 10;

            for (int i = 0; i < 10; i++)
            {
                Box obox = new Box();

                obox.Dimensions.Width.Set(0, 1f);
                obox.Dimensions.Height.Set(0, 1f);
                obox.Background = Color.Violet;

                if (i % 2 == 0)
                {
                    obox.Dimensions.Height.Set(10, 0f);
                }
                if (i % 3 == 0)
                {
                    obox.Dimensions.Width.Set(10, 0f);
                }
                if (i % 4 == 0)
                {
                    obox.Dimensions.Width.Set(1000, 0f);
                }

                list.Append(obox);
            }

            box.Children.Add(list);
            Elements.Add(box);
        }

        public override void Resize(int x, int y, int w, int h)
        {
            box.Recalculate(x, y, 200, h);
        }
    }
}
