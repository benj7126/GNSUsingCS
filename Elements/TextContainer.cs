using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal abstract class TextContainer : Element
    {
        public int FontSize = 24;
        public Vector2 Padding = new Vector2(6, 6);
        public string FontType = ""; // "" = default
        public float Spacing = 1f;
        public float LineSpacing = 1f;
        public Color Color = Color.Black;
        public string Text = "";
    }
}
