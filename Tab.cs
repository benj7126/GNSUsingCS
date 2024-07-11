using Raylib_cs;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace GNSUsingCS
{
    internal abstract class Tab
    {
        protected List<Layer> _layers = [];

        public abstract string Name { get; }

        public int[] sizes = new int[4];

        public void DrawTab(ref int x, bool selected)
        {
            string fontType = Settings.TabSettings.fontType;
            int fontSize = Settings.TabSettings.fontSize;
            Font font = FontManager.GetFont(fontType, fontSize);
            Vector2 TextSize = MeasureTextEx(font, Name, fontSize, 1f);

            DrawRectangle(x, 0, (int)(TextSize.X + Settings.TabSettings.padding.X * 2), (int)(TextSize.Y + Settings.TabSettings.padding.Y * 2), selected ? Color.Gray : Color.White);
            DrawRectangleLines(x, 0, (int)(TextSize.X + Settings.TabSettings.padding.X * 2), (int)(TextSize.Y + Settings.TabSettings.padding.Y * 2), Color.Black);

            x += (int)Settings.TabSettings.padding.X;

            DrawTextEx(font, Name, new Vector2(x, Settings.TabSettings.padding.Y), fontSize, 1f, Color.Black);

            x += (int)Settings.TabSettings.padding.X + (int)TextSize.X;
        }

        public void Resize(int x, int y, int w, int h) { }

        public void Draw(int x, int y, int w, int h)
        {
            if (sizes[0] != x || sizes[1] != y || sizes[2] != w || sizes[3] != h)
            {
                _layers.ForEach(l => l.Resize(x, y, w, h));
            }

            sizes = [x, y, w, h];

            _layers.ForEach(l => l.Draw(x, y, w, h));
        }

        public void Update()
        {
            _layers.ForEach(l => l.Update());
        }
        public void PreUpdate()
        {
            _layers.ForEach(l => l.PreUpdate());
        }

        internal void MouseCaptured(int x, int y)
        {
            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                if (_layers[i].MouseCaptured(x, y))
                {
                    return;
                }
            }
        }
    }
}