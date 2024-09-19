using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GNSUsingCS.Elements
{
    abstract class ReverseDrawElement : Element // not sure if this is dumb or not...
    {
        internal override void Draw()
        {
            bool usedScissors = UseScissor;
            if (usedScissors)
                ScissorManager.EnterScissor(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);

            DrawChildren();
            DrawElement();

            if (usedScissors)
                ScissorManager.ExitScissor();
        }
    }
}
