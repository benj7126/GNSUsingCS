using GNSUsingCS.Elements;
using KeraLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs.ChoiceTab
{
    internal class ChoiceTab : Tab
    {
        static int count = 0;
        int mcount = 0;
        public override string Name => "ChoiceTab #" + mcount;

        ElementLayer elementLayer;

        public ChoiceTab()
        {
            mcount = count++;

            /*
            TextBox b1 = new TextBox();
            b1.Dimensions.Left.Set(0, 1f/4f);
            b1.Dimensions.Width.Set(0, 0.3f);
            b1.Dimensions.Height.Set(0, 0.3f);

            b1.Dimensions.HAlign = 0.5f;

            b1.UseConstraints = false;

            b1.Dimensions.Top.Set(0, (float)new Random().NextDouble() * 0.4f + 0.3f);

            b1.Wrapping = 0;

            TextBox b2 = new TextBox();
            b2.Dimensions.Left.Set(0, 2f / 4f);
            b2.Dimensions.Width.Set(0, 0.3f);
            b2.Dimensions.Height.Set(0, 0.3f);

            b2.Dimensions.HAlign = 0.5f;

            b2.Dimensions.Top.Set(0, (float)new Random().NextDouble() * 0.4f + 0.3f);

            b2.Wrapping = (Wrapping)1;

            TextBox b3 = new TextBox();
            b3.Dimensions.Left.Set(0, 3f / 4f);
            b3.Dimensions.Width.Set(0, 0.3f);
            b3.Dimensions.Height.Set(0, 0.3f);

            b3.Dimensions.HAlign = 0.5f;

            b3.Dimensions.Top.Set(0, (float)new Random().NextDouble() * 0.4f + 0.3f);

            b3.Wrapping = (Wrapping)2;

            ElementLayer layer = new([b1, b2, b3]);
            _layers = [layer];
            */


            elementLayer = new([]);
            _layers = [elementLayer];
        }

        public void SetTempTestThings()
        {
            LuaInterfacer.EnterNote("");
            LuaInterfacer.EnterTab(UUID);
            TextBox TB = new TextBox();
            LuaInterfacer.EnterElement(0, TB);
            TB.Dimensions.Left.Set(0, 0.5f);
            TB.Dimensions.Width.Set(0, 0.7f);
            TB.Dimensions.Height.Set(0, 0.7f);

            TB.Dimensions.HAlign = 0.5f;
            TB.Dimensions.VAlign = 0.5f;

            TB.Dimensions.Top.Set(0, 0.5f);

            TB.Wrapping = Wrapping.WordWrapping;

            Button B = new Button();
            LuaInterfacer.EnterElement(1, B);
            B.Label.Text = "Counter 0";
            B.Dimensions.Width.Set(400, 0f);
            B.Dimensions.Height.Set(20, 0f);
            B.Label.FontSize = 20;
            B.Code = """
                -- if G.counter == nil then
                --     G.counter = 0
                -- end
                G.counter = G.counter or 0


                function OnPress()
                    G.counter = G.counter + 1

                    this.Code = this.Code .. "+1"
                    this:LoadCode()
                
                    this.Label.Text = "Counter " .. i .. " | " .. G.counter
                end

                i=0
                """;

            B.LoadCode();

            elementLayer.Elements.Add(TB);
            elementLayer.Elements.Add(B);
        }

        public override void SaveData(ref SaveObject so)
        {
            so.Write(elementLayer.Elements.Count.ToString());
            foreach (Element e in elementLayer.Elements)
            {
                e.SaveFromObject(ref so);
            }
        }

        public override void LoadData(ref LoadObject lo)
        {
            int elementCount = int.Parse(lo.Read());

            for (int i = 0; i < elementCount; i++)
            {
                elementLayer.Elements.Add(Element.LoadToObject(ref lo));
            }
        }
    }
}
