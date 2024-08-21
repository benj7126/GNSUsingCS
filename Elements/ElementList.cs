using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal class ElementList : AppendableElement
    {
        public List<Element> _children = [];
        public override List<Element> Children => _children;
        internal override bool UseScissor => false;

        private int _width = 0;

        public override void Append(Element e)
        {
            float top = 0;

            if (Children.Count > 0)
            {
                ElementStyle es = Children.Last().Dimensions;

                top = es.Y + es.H;
            }

            e.Dimensions.Top.Set((int)top, 0f);

            Children.Add(e);

            Recalculate();
        }

        protected override void DrawElement()
        {
            foreach (Element e in Children)
            {
                e.Draw();
            }
        }
    }
}
