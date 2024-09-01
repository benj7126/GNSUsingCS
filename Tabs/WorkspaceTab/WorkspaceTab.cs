using GNSUsingCS.Elements;
using GNSUsingCS.Tabs.WorkspaceTab.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs.WorkspaceTab
{
    internal class WorkspaceTab : Tab
    {
        public override string Name => "WorkspaceTab";
        public WorkspaceTab()
        {

            DraggableNodeLayer layer = new();
            DropdownLayer ddl = new(layer);
            _layers = [ddl, layer];
        }
    }
}
