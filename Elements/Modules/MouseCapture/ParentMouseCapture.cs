using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.MouseCapture
{
    internal class ParentMouseCapture : IMouseCapture
    {
        bool IMouseCapture.MouseCaptured(int px, int py, Element e)
        {
            e.IsHovered = true;
            for (int i = 0; i < e.Children.Count; i++)
            {
                // TODO: \/ - maby module for either type, or a submodule for it?
                /* this or below, unsure yet...
                if (Children[i].MouseCaptured(px, py))
                {
                    return true;
                }
                */
                e.Children[i].MouseCaptured(px, py);
            }

            return true;
        }
    }
}
