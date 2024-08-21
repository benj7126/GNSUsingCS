using NLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

        public static Dictionary<Constraint, int> recursiveConstraints(Element container, List<Element> restraints)
        {
            Dictionary<Constraint, int> baseConstraints = container.Constraints();

            Dictionary<Constraint, int> constraints = restraints.Select(e => recursiveConstraints(e, e.Children)).
                Aggregate(baseConstraints,
                (Dictionary<Constraint, int> p, Dictionary<Constraint, int> n) => {

                    p[Constraint.MinW] = n[Constraint.MinW] == -1 ? p[Constraint.MinW] : Math.Max(p[Constraint.MinW], n[Constraint.MinW]);
                    p[Constraint.MaxW] = n[Constraint.MaxW] == -1 ? p[Constraint.MaxW] : Math.Min(p[Constraint.MaxW], n[Constraint.MaxW]);

                    p[Constraint.MinH] = n[Constraint.MinH] == -1 ? p[Constraint.MinH] : Math.Max(p[Constraint.MinH], n[Constraint.MinH]);
                    p[Constraint.MaxH] = n[Constraint.MaxH] == -1 ? p[Constraint.MaxH] : Math.Min(p[Constraint.MaxH], n[Constraint.MaxH]);

                    return p;
                }
                );

            for (int i = 0; i < 4; i += 2) // i < 8
                if (constraints[(Constraint)i] < constraints[(Constraint)(i + 1)]) // prioritize min constraint
                    constraints[(Constraint)(i + 1)] = constraints[(Constraint)i];

            return baseConstraints;
        }

        public void Recalculate(int x, int y, int w, int h, Element container, List<Element> restraints, bool useConstraints = true)
        {
            this.X = x + Left.GetValue(w);
            this.Y = y + Top.GetValue(h);

            this.W = Width.GetValue(w);
            this.H = Height.GetValue(h);

            if (useConstraints)
            {
                Dictionary<Constraint, int> constraints = recursiveConstraints(container, restraints);

                float nW = constraints[Constraint.MinW] == -1 ? this.W : Math.Max(this.W, constraints[Constraint.MinW]);
                this.W = (int)(constraints[Constraint.MaxW] == -1 ? nW : Math.Min(nW, constraints[Constraint.MaxW]));

                float nH = constraints[Constraint.MinH] == -1 ? this.H : Math.Max(this.H, constraints[Constraint.MinH]);
                this.H = (int)(constraints[Constraint.MaxH] == -1 ? nH : Math.Min(nH, constraints[Constraint.MaxH]));

                this.X -= (int)(HAlign * this.W);
                this.Y -= (int)(VAlign * this.H);
            }
            else
            {
                this.X -= (int)(HAlign * this.W);
                this.Y -= (int)(VAlign * this.H);
            }
        }
    }

    internal abstract class Element
    {
        internal static Element GetElementFromString(string element)
        {
            string type = "";
            string code = "";

            bool typeReading = true;

            foreach (char c in element)
            {
                if (c == '\n' && typeReading)
                {
                    typeReading = false;
                    continue;
                }

                if (typeReading)
                {
                    type += c;
                }
                else
                {
                    code += c;
                }
            }

            Type t = typeof(Element).Assembly.GetType(type);
            if (t is null)
            {
                throw new Exception("Type " + t + " not found while loading element:\n" + element);
            }

            Element e = (Element)Activator.CreateInstance(t);
            e.Code = code;

            return e;
        }

        public void LoadCode()
        {
            LuaInterfacer.SetElementCode(Code);
        }

        internal string GetStringFromElement()
        {
            return GetType().FullName + "\n" + Code;
        }

        internal ElementStyle Dimensions = new ElementStyle();

        public string Code = "";

        public virtual List<Element> Children => [];

        public bool UseConstraints = true;

        internal bool IsHovered = false;

        internal virtual bool UseScissor => true;

        internal void Draw()
        {
            if (UseScissor)
                ScissorManager.EnterScissor(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);

            DrawElement();

            if (UseScissor)
                ScissorManager.ExitScissor();
        }

        protected virtual void DrawElement() { }

        internal virtual void PostRecalculate(int x, int y, int w, int h) { }

        internal void RecalculateChildren(int x, int y, int w, int h)
        {
            Children.ForEach(c => c.Recalculate(x, y, w, h));
        }

        int[] parentSize = [0, 0, 0, 0];
        internal void Recalculate(int x, int y, int w, int h)
        {
            parentSize = [x, y, w, h];
            Dimensions.Recalculate(x, y, w, h, this, Children, UseConstraints);
            PostRecalculate(x, y, w, h);
            RecalculateChildren(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);
        }

        public void Recalculate()
        {
            Recalculate(parentSize[0], parentSize[1], parentSize[2], parentSize[3]);
        }

        internal virtual void Update()
        {/*
            Code = """
                if o == "N" then
                    OnPress = function()
                        print(o)
                    end
                end
                
                print(o, "f")
                o = "N"
                """;

            LuaInterfacer.EnterTab(Guid.NewGuid().ToString());
            LuaInterfacer.EnterNote(Guid.NewGuid().ToString());
            LuaInterfacer.EnterElement(0);

            Console.WriteLine("Test");

            LuaInterfacer.DoString(Code);
            LuaInterfacer.DoString(Code);
            LuaInterfacer.TryCallMethod("OnPress");

            LuaInterfacer.EnterElement(1);

            LuaInterfacer.DoString(Code);
            LuaInterfacer.TryCallMethod("OnPress");
            */
        }
        internal void PreUpdate()
        {
            IsHovered = false;
            Children.ForEach(c => c.PreUpdate());
        }

        // only one node can be hovored so when i make nodes this needs to bo overwritten
        internal bool MouseCaptured(int px, int py)
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

        internal virtual void ExtraConstraints(ref Dictionary<Constraint, int> constraints) { }

        internal Dictionary<Constraint, int> Constraints()
        {
            if (!UseConstraints)
            {
                return new Dictionary<Constraint, int>
                {/*
                    { Constraint.MinX, -1 },
                    { Constraint.MaxX, -1 },
                    { Constraint.MinY, -1 },
                    { Constraint.MaxY, -1 },
                    */
                    { Constraint.MinW, -1 },
                    { Constraint.MaxW, -1 },
                    { Constraint.MinH, -1 },
                    { Constraint.MaxH, -1 }
                };
            }

            Dictionary<Constraint, int> c = new Dictionary<Constraint, int>
            {
                { Constraint.MaxW, Dimensions.Width.MaxPixels },
                { Constraint.MinW, Dimensions.Width.MinPixels },
                { Constraint.MaxH, Dimensions.Height.MaxPixels },
                { Constraint.MinH, Dimensions.Height.MinPixels },
            };

            ExtraConstraints(ref c);

            /*
            int nxLeft = Dimensions.Left.Pixels + (int)(Dimensions.Width.Pixels * (Dimensions.HAlign));
            int nxRight = Dimensions.Left.Pixels + (int)(Dimensions.Width.Pixels * (1f - Dimensions.HAlign));
            int nyTop = Dimensions.Top.Pixels + (int)(Dimensions.Height.Pixels * (Dimensions.VAlign));
            int nyBot = Dimensions.Top.Pixels + (int)(Dimensions.Height.Pixels * (1f - Dimensions.VAlign));

            c = new Dictionary<Constraint, int>
            {
                { Constraint.MaxW, nxLeft < c[Constraint.MinW] || c[Constraint.MinW] == -1 ? nxLeft : c[Constraint.MinW] },
                { Constraint.MinW, nxRight > c[Constraint.MaxW] || c[Constraint.MaxW] == -1 ? nxRight : c[Constraint.MaxW] },
                { Constraint.MaxH, nyTop < c[Constraint.MinH] || c[Constraint.MinH] == -1 ? nyTop : c[Constraint.MinH] },
                { Constraint.MinH, nyBot < c[Constraint.Max] || Dimensions.Height.MaxPixels == -1 ? nyBot : Dimensions.Height.MaxPixels },
            };
            */

            return c;
        }

        internal string Save()
        {
            return GetType().Name + ":" + SaveAndLoadManager.SetupArray([SaveValues(), SaveChildValues()]);
        }

        internal string Load(string text)
        {
            (List<string> load, string rest) = SaveAndLoadManager.ParseArray(text);

            LoadValues(load[0]);
            LoadChildValues(load[1]);

            return rest;
        }

        internal virtual string SaveValues() { return ""; }
        internal virtual void LoadValues(string val) { }

        internal string SaveChildValues()
        {
            List<string> save = [];
            foreach (Element c in Children)
            {
                save.Add(SaveAndLoadManager.SetupArray([c.SaveValues(), c.SaveChildValues()]));
            }

            return SaveAndLoadManager.SetupArray(save);
        }

        internal void LoadChildValues(string text)
        {
            (List<string> load, string rest) = SaveAndLoadManager.ParseArray(text);

            foreach ((string saveStr, Element c) in load.Zip(Children))
            {
                (List<string> vals, string error) = SaveAndLoadManager.ParseArray(saveStr);
                if (error != "")
                    throw new Exception(saveStr + " should have just been an array, but had [" + error + "] remaining at the end");

                c.LoadValues(vals[0]);
                c.LoadChildValues(vals[1]);
            }

            /// return rest;
        }

        internal static Element LoadElement()
        {
            return null;
        }
    }
}

public enum Constraint
{
    /*
    MinX,
    MaxX,
    MinY,
    MaxY,
    */
    MinW,
    MaxW,
    MinH,
    MaxH
}