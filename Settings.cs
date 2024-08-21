using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal static class Settings
    {
        public static bool editMode = false;
        public static string defaultFontType = "Vera";
        public static TabSettings TabSettings = new();

        public static float RepeatKeyTime = 0.02f;
        public static float FirstRepeatKeyTime = 0.4f;
        public static float MouseScrollVelocityDropoff = 0.8f;
        public static float mouseScrollSensitivity = 2.4f;

        public static int ScrollOffUp = 2;
        public static int ScrollOffDown = 4;
    }
}

struct TabSettings()
{
    public int fontSize = 24;
    public Vector2 padding = new(3, 6);
    public string fontType = ""; // "" = default

    // something about spacing?
};