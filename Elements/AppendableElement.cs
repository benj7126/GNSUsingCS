using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal abstract class AppendableElement : Element
    {
        public virtual void Append(Element e)
        {
            Children.Add(e);
        }
    }
}
