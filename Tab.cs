using GNSUsingCS.Tabs;
using Raylib_cs;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace GNSUsingCS
{
    internal abstract class Tab
    {
        public abstract string Name { get; }

        public int[] sizes = new int[4];

        public string ID = Guid.NewGuid().ToString(); // should probably have a thing that check for the loaded UUID's as well
                                                        // and then just return the instance if it already exists...

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

        public virtual void Resize(int x, int y, int w, int h) { }


        public bool ForceRecalc = false;
        public virtual void Draw(int x, int y, int w, int h)
        {
            if (sizes[0] != x || sizes[1] != y || sizes[2] != w || sizes[3] != h || ForceRecalc)
            {
                ForceRecalc = false;
                Resize(x, y, w, h);
            }

            sizes = [x, y, w, h];
        }
        public virtual void PreUpdate() { }

        public virtual void Update() { }
        public virtual void PostUpdate()
        {
            if ((IsKeyDown(KeyboardKey.LeftControl) || IsKeyDown(KeyboardKey.RightControl)) && IsKeyPressed(KeyboardKey.S))
            {
                SaveAndLoadManager.SaveTab(this);
                if (this is WorkspaceTab wt)
                {
                    wt.SaveWorkspace();
                }
            }
        }

        public virtual void MouseCaptured(int x, int y) { }
    }
}