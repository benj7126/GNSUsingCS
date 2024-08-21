using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal static class MouseManager
    {
        public static Vector2 MousePosition;
        public static Vector2 MouseVelocity;
        public static Vector2 LastMousePosition;

        public static void Update()
        {
            LastMousePosition = MousePosition;
            MousePosition = GetMousePosition();

            MouseVelocity = MousePosition - LastMousePosition;
        }
    }
}
