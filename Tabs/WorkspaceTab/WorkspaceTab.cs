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
        private DraggableNodeLayer layer;
        private DropdownLayer ddl;

        public override string Name => "WorkspaceTab";
        public WorkspaceTab()
        {
            DraggableNodeLayer layer = new();
            // DropdownLayer ddl = new(layer);
            ToolboxLayer tl = new(layer);

            _layers = [tl, layer];
        }
    }
}
