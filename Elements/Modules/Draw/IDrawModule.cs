using GNSUsingCS.Elements.Modules.Recalculate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements.Modules.Draw
{
    internal interface IDrawModule : IModule
    {
        internal abstract void Draw(Element e);
    }
}
