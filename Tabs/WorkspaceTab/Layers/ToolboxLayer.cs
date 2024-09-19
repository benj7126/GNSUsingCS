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
        private Box box = new Box();
        private ElementList list;

        public ToolboxLayer(ElementLayer layer) : base([])
        {
            box.Dimensions.Width.Set(0, 1f);
            box.Dimensions.Height.Set(0, 1f);
            box.Background.A = 100;

            list = new ElementList();
            list.Margin = 10;

            for (int i = 0; i < 1; i++)
            {
                GhostDraggable draggable = new GhostDraggable();
                draggable.Dimensions.Width.Set(0, 1f);
                draggable.Dimensions.Height.Set(0, 1f);
                list.Append(draggable);

                Box obox = new Box();

                obox.Dimensions.Width.Set(0, 1f);
                obox.Dimensions.Height.Set(0, 1f);
                obox.Background = Color.Violet;

                draggable.Children.Add(obox);

                /*
                Button button = new Button();
                int _i = i+1;
                button.actions.Add("OnPress", () => Console.WriteLine("pressed " + _i));
                button.Dimensions.Width.Set(0, 1f);
                button.Dimensions.Height.Set(0, 1f);
                button.Box.Background = Color.Violet;
                button.Label.Text = _i + "'th button";
                list.Append(button);
                */

                /*
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
                */
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
