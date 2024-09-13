using Raylib_cs;
using System;
using System.IO;
using System.Text;


namespace GNSUsingCS
{
    internal static class FontManager
    {
        private static readonly Dictionary<string, Font> _fontMap = [];


        public static Font GetFont(string fontName, int size, TextureFilter filter = TextureFilter.Point)
        {
            if (fontName == "")
            {
                fontName = Settings.DefaultFontType;
            }

            string filePath = "Assets/Fonts/" + fontName + ".ttf";
            fontName = $"{fontName} | {filter} | {size}";

            if (!_fontMap.ContainsKey(fontName))
            {
                Font font = LoadFontEx(filePath, size, null, 0); // no clue about these codepoints...
                Console.WriteLine(font);
                SetTextureFilter(font.Texture, filter);

                _fontMap.Add(fontName, font);
            }

            return _fontMap[fontName];
        }
    }
}
