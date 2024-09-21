using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GNSUsingCS.Elements
{
    internal class Box : Element
    {
        public Color Background = Color.Orange;
        public Color Border = Color.Black;
        internal override void DrawElement()
        {
            IsHovered = false;
            DrawRectangle(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H, IsHovered ? Color.Black : Background);
            DrawRectangleLines(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H, Border);
        }

        /* might be useless so comment out for now
        public int Width = -1;
        public int Height = -1;

        public override Dictionary<Constraint, int> Constraints()
        {
            Dictionary<Constraint, int> constraints = base.Constraints();

            if (Width != -1)
                constraints[Constraint.MinW] = constraints[Constraint.MaxW] = Width;
            
            if (Height != -1)
                constraints[Constraint.MinH] = constraints[Constraint.MaxH] = Height;

            return constraints;
        }
        */
    }
}
