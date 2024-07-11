using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal class StyleDimension
    {
        public int Pixels;
        public float Precent;
        public int MinPixels = -1;
        public int MaxPixels = -1;
        public void Set(int pixels, float precent)
        {
            Pixels = pixels;
            Precent = precent;
        }
        public void Set(int pixels, float precent, int minPixels, int maxPixels)
        {
            Pixels = pixels;
            Precent = precent;
            MinPixels = minPixels;
            MaxPixels = maxPixels;
        }
        public int GetValue(int containerSize) => Pixels + (int)(Precent * containerSize);
    }
    internal class ElementStyle
    {
        public StyleDimension Left = new();
        public StyleDimension Top = new();

        public StyleDimension Width = new();
        public StyleDimension Height = new();

        public float HAlign;
        public float VAlign;

        public ElementStyle() { }

        public ElementStyle(int lPixels, int lPercent, int tPixels, int tPercent,
        int wPixels, int wPercent, int hPixels, int hPercent, int halign, int valign,
        int x, int y, int w, int h, Element container, List<Element> restraints)
        {
            Left.Set(lPixels, lPercent);
            Top.Set(tPixels, tPercent);

            Width.Set(wPixels, wPercent);
            Height.Set(hPixels, hPercent);

            HAlign = halign;
            VAlign = valign;

            Recalculate(x, y, w, h, container, restraints);
        }

        public int X = 0;
        public int Y = 0;
        public int W = 0;
        public int H = 0;

        public bool ContainsPoint(int px, int py)
        {
            if (px > X && px < X + W && py > Y &&  py < Y + H)
                return true;

            return false;
        }

        public void Recalculate(int x, int y, int w, int h, Element container, List<Element> restraints, bool useConstraints = true)
        {
            if (useConstraints)
            {
                Dictionary<Constraint, int> baseConstraints = container.Constraints();

                for (int i = 0; i < 8; i += 2)
                {
                    baseConstraints[(Constraint)i] = baseConstraints[(Constraint)i] == -1 ? int.MinValue : baseConstraints[(Constraint)i];
                    baseConstraints[(Constraint)(i + 1)] = baseConstraints[(Constraint)(i + 1)] == -1 ? int.MaxValue : baseConstraints[(Constraint)(i + 1)];
                }

                Dictionary<Constraint, int> constraints = restraints.Select(e => e.Constraints()).
                    Aggregate(baseConstraints,
                    (Dictionary<Constraint, int> p, Dictionary<Constraint, int> n) => {

                        p[Constraint.MinW] = n[Constraint.MinW] == -1 ? p[Constraint.MinW] : Math.Max(p[Constraint.MinW], n[Constraint.MinW] + n[Constraint.MinX]);
                        p[Constraint.MaxW] = n[Constraint.MaxW] == -1 ? p[Constraint.MaxW] : Math.Min(p[Constraint.MaxW], n[Constraint.MaxW]);

                        p[Constraint.MinH] = n[Constraint.MinH] == -1 ? p[Constraint.MinH] : Math.Max(p[Constraint.MinH], n[Constraint.MinH] + n[Constraint.MinH]);
                        p[Constraint.MaxH] = n[Constraint.MaxH] == -1 ? p[Constraint.MaxH] : Math.Min(p[Constraint.MaxH], n[Constraint.MaxH]);

                        return p;
                    }
                    );

                for (int i = 0; i < 8; i += 2)
                    if (constraints[(Constraint)i] > constraints[(Constraint)(i + 1)]) // prioritize min constraint
                        constraints[(Constraint)(i + 1)] = constraints[(Constraint)i];

                this.X = x + Left.GetValue(w);
                this.Y = y + Top.GetValue(h);

                this.W = Width.GetValue(w);
                this.H = Height.GetValue(h);

                this.W = Math.Min(Math.Max(this.W, constraints[Constraint.MinW]), constraints[Constraint.MaxW]);
                this.H = Math.Min(Math.Max(this.H, constraints[Constraint.MinH]), constraints[Constraint.MaxH]);

                this.X -= (int)(HAlign * this.W);
                this.Y -= (int)(VAlign * this.H);

                this.X = Math.Min(Math.Max(this.X - x, constraints[Constraint.MinX]), constraints[Constraint.MaxX]) + x;
                this.Y = Math.Min(Math.Max(this.Y - y, constraints[Constraint.MinY]), constraints[Constraint.MaxY]) + y;
            }
            else
            {

                this.X = x + Left.GetValue(w);
                this.Y = y + Top.GetValue(h);

                this.W = Width.GetValue(w);
                this.H = Height.GetValue(h);

                this.X -= (int)(HAlign * this.W);
                this.Y -= (int)(VAlign * this.H);
            }
        }
    }

    internal abstract class Element
    {
        public ElementStyle Dimensions = new ElementStyle();

        public virtual List<Element> Children => [];

        public bool UseConstraints = true;

        public bool IsHovered = false;

        public void Draw()
        {
            ScissorManager.EnterScissor(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);

            DrawElement();

            ScissorManager.ExitScissor();
        }

        protected virtual void DrawElement() { }

        public void RecalculateChildren(int x, int y, int w, int h)
        {
            Children.ForEach(c => c.Recalculate(x, y, w, h));
        }

        public void Recalculate(int x, int y, int w, int h)
        {
            Dimensions.Recalculate(x, y, w, h, this, Children, UseConstraints);
            RecalculateChildren(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);
        }
        public virtual void Update() { }
        public void PreUpdate()
        {
            IsHovered = false;
            Children.ForEach(c => c.PreUpdate());
        }

        // only one node can be hovored so when i make nodes this needs to bo overwritten
        public bool MouseCaptured(int px, int py)
        {
            if (Dimensions.ContainsPoint(px, py))
            {
                IsHovered = true;
                for (int i = 0; i < Children.Count; i++)
                {
                    // TODO: \/
                    /* this or below, unsure yet...
                    if (Children[i].MouseCaptured(px, py))
                    {
                        return true;
                    }
                    */
                    Children[i].MouseCaptured(px, py);
                }

                return true;
            }

            return false;
        }

        public virtual void ExtraConstraints(ref Dictionary<Constraint, int> constraints) { }

        public Dictionary<Constraint, int> Constraints()
        {
            if (!UseConstraints)
            {
                return new Dictionary<Constraint, int>
                {
                    { Constraint.MinX, -1 },
                    { Constraint.MaxX, -1 },
                    { Constraint.MinY, -1 },
                    { Constraint.MaxY, -1 },
                    { Constraint.MinW, -1 },
                    { Constraint.MaxW, -1 },
                    { Constraint.MinH, -1 },
                    { Constraint.MaxH, -1 }
                };
            }

            Dictionary<Constraint, int>  c = new Dictionary<Constraint, int>
            {
                { Constraint.MinX, Dimensions.Left.MinPixels },
                { Constraint.MaxX, Dimensions.Left.MaxPixels },
                { Constraint.MinY, Dimensions.Top.MinPixels },
                { Constraint.MaxY, Dimensions.Top.MaxPixels },
                { Constraint.MinW, Dimensions.Width.MinPixels },
                { Constraint.MaxW, Dimensions.Width.MaxPixels },
                { Constraint.MinH, Dimensions.Height.MinPixels },
                { Constraint.MaxH, Dimensions.Height.MaxPixels }
            };

            ExtraConstraints(ref c);

            return c;
        }
    }
}

public enum Constraint
{
    MinX,
    MaxX,
    MinY,
    MaxY,
    MinW,
    MaxW,
    MinH,
    MaxH
}