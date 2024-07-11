using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal class ElementLayer(List<Element> _elements) : Layer
    {
        public override void Draw(int x, int y, int w, int h) // i would need a "resize" event instead of this
        {
            _elements.ForEach(e => e.Draw());
        }
        public override void Resize(int x, int y, int w, int h)
        {
            _elements.ForEach(e => e.Recalculate(x, y, w, h));
        }

        public override void Update()
        {
            _elements.ForEach(e => e.Update());
        }

        public override void PreUpdate()
        {
            _elements.ForEach(e => e.PreUpdate());
        }

        public override bool MouseCaptured(int px, int py)
        {
            for (int i = _elements.Count - 1; i >= 0; i--)
                {
                if (_elements[i].Dimensions.ContainsPoint(px, py))
                {
                    _elements[i].MouseCaptured(px, py);
                    return true;
                }
            }

            return false;
        }
    }
}
