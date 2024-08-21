using GNSUsingCS.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal class ElementLayer : Layer
    {
        public List<Element> Elements;

        public ElementLayer(List<Element> elements)
        {
            Elements = elements;
        }

        public virtual Element GetRightclickPanel() { return new Button(); }

        public override void Draw(int x, int y, int w, int h) // i would need a "resize" event instead of this
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                LuaInterfacer.EnterElement(i, Elements[i]);
                Elements[i].Draw();
            }
        }
        public override void Resize(int x, int y, int w, int h)
        {
            Elements.ForEach(e => e.Recalculate(x, y, w, h));
        }

        public override void Update()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                LuaInterfacer.EnterElement(i, Elements[i]);
                Elements[i].Update();
            }
        }

        public override void PreUpdate()
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                LuaInterfacer.EnterElement(i, Elements[i]);
                Elements[i].PreUpdate();
            }
        }

        public override bool MouseCaptured(int px, int py)
        {
            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                LuaInterfacer.EnterElement(i, Elements[i]);
                if (Elements[i].Dimensions.ContainsPoint(px, py))
                {
                    Elements[i].MouseCaptured(px, py);
                    return true;
                }
            }

            return false;
        }

        public override void Save()
        {
            string save = "";
            save += SaveAndLoadManager.SetupArray(Elements.Select(e => e.Save()).ToList());

            Elements.ForEach(e => e.Load(e.Save()[(e.Save().IndexOf(':')+1)..]));
        }
    }
}
