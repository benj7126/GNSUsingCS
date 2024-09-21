using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Draw
{
    internal class ElementFirstDraw : IDrawModule
    {
        void IDrawModule.Draw(Element e)
        {
            bool usedScissors = e.UseScissor;
            if (usedScissors)
                e.ScissorModule.EnterScissor(e.Dimensions.X, e.Dimensions.Y, e.Dimensions.W, e.Dimensions.H);

            e.DrawElement();
            e.DrawChildren();

            if (usedScissors)
                ScissorManager.ExitScissor();
        }
    }
}
