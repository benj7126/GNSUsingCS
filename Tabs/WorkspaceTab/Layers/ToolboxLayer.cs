using GNSUsingCS.Elements;
using GNSUsingCS.Elements.Modules.Recalculate;
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

            for (int i = 0; i < 10; i++)
            {
                GhostDraggable draggable = new GhostDraggable();
                draggable.Dimensions.Width.Set(0, 1f);
                draggable.Dimensions.Height.Set(0, 1f);
                list.Children.Add(draggable);

                Label tempLabel = new Label();
                tempLabel.Text = "Test " + i;
                tempLabel.Dimensions.Width.Set(0, 1f);
                tempLabel.FontSize = 42;

                ElementInstanceDisplay eid = new ElementInstanceDisplay(new ElementSettingsInstance(tempLabel));
                draggable.RecalculateModule = new FitChildElementRecalculate(draggable.RecalculateModule, eid);

                draggable.Children.Add(eid);

                int _i = i;
                draggable.actions.Add("Released", (objs) => {
                    Element e = new ElementSettingsInstance(tempLabel).CreateElementFrom();
                    // place in a note
                    e.Dimensions.Left.Set((int)MouseManager.MousePosition.X, 0f);
                    e.Dimensions.Top.Set((int)MouseManager.MousePosition.Y, 0f);

                    layer.Elements.Add(e);
                    ApplicationManager.Instance.CurrentTab.ForceRecalc = true;
                });

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
