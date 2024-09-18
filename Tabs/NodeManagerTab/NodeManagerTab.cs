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
    internal class NodeManagerTab : Tab
    {
        public override string Name => "NodeManagerTab";

        ElementLayer elementLayer;

        public NodeManagerTab()
        {
            elementLayer = new([]);
            _layers = [elementLayer];
        }
    }
}
