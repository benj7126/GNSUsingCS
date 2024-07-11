using Raylib_cs;

namespace GNSUsingCS
{
    internal static class FontManager
    {
        private static readonly Dictionary<string, Font> _fontMap = [];


        public static Font GetFont(string fontName, int size, TextureFilter filter = TextureFilter.Point)
        {
            if (fontName == "")
            {
                fontName = Settings.defaultFontType;
            }

            string filePath = "assets/Fonts/" + fontName + ".ttf";
            fontName = $"{fontName} | {filter} | {size}";

            if (!_fontMap.ContainsKey(fontName))
            {
                Font font = LoadFontEx(filePath, size, null, 0); // no clue about these codepoints...
                SetTextureFilter(font.Texture, filter);

                _fontMap.Add(fontName, font);
            }

            return _fontMap[fontName];
        }
    }
}
