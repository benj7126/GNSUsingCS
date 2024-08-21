using GNSUsingCS.Elements;
using KeraLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs.ChoiceTab
{
    internal class ChoiceTab : Tab
    {
        static int count = 0;
        int mcount = 0;
        public override string Name => "ChoiceTab #" + mcount;

        public ChoiceTab()
        {
            mcount = count++;

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
            B.Code = """
                if G.counter == nil then
                    G.counter = 0
                end


                function OnPress()
                    -- if i == 10 then
                    --     this:LoadCode()
                    -- end

                    G.counter = G.counter + 1

                    this.Code = this.Code .. "+1"
                    this:LoadCode()
                
                    this.Label.Text = "Counter " .. i .. " | " .. G.counter
                    this:Recalculate()

                    print(this.Code)

                end

                i=0
                """;

            B.LoadCode();

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


            ElementLayer layer = new([TB, B]);
            _layers = [layer];

            layer.Save();
        }
    }
}
