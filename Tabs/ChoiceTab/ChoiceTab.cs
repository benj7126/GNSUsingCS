using GNSUsingCS.Elements;
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
        }
    }
}
