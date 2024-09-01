namespace GNSUsingCS
{
    internal class StyleDimension
    {
        public int Pixels;
        public float Percent;

        public void Set(int pixels, float precent)
        {
            Pixels = pixels;
            Percent = precent;
        }

        public int GetValue(int containerSize) => Pixels + (int)(Percent * containerSize);
        public int GetPValueOrNone() => Percent == 0 ? Pixels : -1;
    }
    internal class ExtendedStyleDimension
    {
        public int Pixels;
        public float Percent;

        public int MinPixels = 0;
        public int MaxPixels = 0;

        public bool MinUsed = false;
        public bool MaxUsed = false;

        public void Set(int pixels, float precent)
        {
            Pixels = pixels;
            Percent = precent;
        }

        public int GetValue(int containerSize) => Pixels + (int)(Percent * containerSize);
        public int GetPValueOrNone() => Percent == 0 ? Pixels : -1;
    }

    internal class ElementStyle
    {
        public StyleDimension Left = new();
        public StyleDimension Top = new();

        public ExtendedStyleDimension Width = new();
        public ExtendedStyleDimension Height = new();

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

            Recalculate(x, y, w, h);
        }

        public int X = 0;
        public int Y = 0;
        public int W = 0;
        public int H = 0;

        public void Recalculate(int x, int y, int w, int h)
        {
            this.X = x + Left.GetValue(w);
            this.Y = y + Top.GetValue(h);

            this.W = Width.GetValue(w);
            this.H = Height.GetValue(h);

            this.X -= (int)(HAlign * this.W);
            this.Y -= (int)(VAlign * this.H);
        }

        public bool ContainsPoint(int px, int py)
        {
            if (px > X && px < X + W && py > Y &&  py < Y + H)
                return true;

            return false;
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
        public bool SaveLuaState = false;

        public virtual List<Element> Children => [];

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

        internal void RecalculateChildren()
        {
            Children.ForEach(c => c.Recalculate(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H));
        }

        int[] parentSize = [0, 0, 0, 0];
        internal void Recalculate(int x, int y, int w, int h)
        {
            Dimensions.X = -1;
            Dimensions.Y = -1;
            Dimensions.W = -1;
            Dimensions.H = -1;

            Dimensions.Recalculate(x, y, w, h);

            RecalculateChildren();
            
            parentSize = [x, y, w, h];
            PostRecalculate(x, y, w, h);
        }

        public void Recalculate()
        {
            Recalculate(parentSize[0], parentSize[1], parentSize[2], parentSize[3]);
        }

        internal virtual void Update() {}

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