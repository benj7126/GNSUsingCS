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
        internal void SaveValues(ref SaveObject so)
        {
            so.Write(Pixels);
            so.Write(Percent);
        }

        internal void LoadValues(ref LoadObject lo)
        {
            Pixels = lo.ReadInt();
            Percent = lo.ReadFloat();
        }
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

        internal void SaveValues(ref SaveObject so)
        {
            Left.SaveValues(ref so);
            Top.SaveValues(ref so);

            Width.SaveValues(ref so);
            Height.SaveValues(ref so);

            so.Write(HAlign);
            so.Write(VAlign);
        }

        internal void LoadValues(ref LoadObject lo)
        {
            Left.LoadValues(ref lo);
            Top.LoadValues(ref lo);

            Width.LoadValues(ref lo);
            Height.LoadValues(ref lo);

            HAlign = lo.ReadFloat();
            VAlign = lo.ReadFloat();
        }
    }

    internal abstract class Element
    {
        public void LoadCode()
        {
            LuaInterfacer.SetElementCode(Code);
        }

        internal void SaveFromObject(ref SaveObject so)
        {
            so.Write(GetType().FullName);
            so.Write(Code);
            Dimensions.SaveValues(ref so);
            SaveValues(ref so);

            foreach (Element c in Children)
            {
                c.SaveFromChild(ref so);
            }
        }

        internal static Element LoadToObject(ref LoadObject lo)
        {
            string type = lo.Read();

            Type t = typeof(Element).Assembly.GetType(type);
            if (t is null)
            {
                throw new Exception("Type " + t + " not found");
            }

            Element e = (Element)Activator.CreateInstance(t);

            e.Code = lo.Read();
            e.Dimensions.LoadValues(ref lo);
            e.LoadValues(ref lo);

            foreach (Element c in e.Children)
            {
                c.LoadToChild(ref lo);
            }

            // e.LoadCode(); -- have to make sure the lua system links right...

            return e;
        }

        internal void SaveFromChild(ref SaveObject so)
        {
            so.Write(Code);
            Dimensions.SaveValues(ref so);
            SaveValues(ref so);

            foreach (Element c in Children)
            {
                c.SaveFromChild(ref so);
            }
        }

        internal void LoadToChild(ref LoadObject lo)
        {
            Code = lo.Read();
            Dimensions.LoadValues(ref lo);
            LoadValues(ref lo);

            foreach (Element c in Children)
            {
                c.LoadToChild(ref lo);
            }
        }

        internal virtual void SaveValues(ref SaveObject so) { }
        internal virtual void LoadValues(ref LoadObject lo) { }

        internal ElementStyle Dimensions = new ElementStyle();

        public string Code = "";
        [ConfigAttributes.Bool]
        public bool SaveLuaState = false; // if non-local variables should be saved.

        [ConfigAttributes.Bool]                         // TODO:
        public bool NeedsSave = false; // for making a *, or another indicator of changed save to the tab containing this element.
                                       // i might want to make it possible to save data from elements without saving everything in the tab
                                       // making it edit the save file seems hard, but making a folder for the tab uuid
                                       // and placing all elements in there seems plausible...

        public virtual List<Element> Children => [];

        internal bool IsHovered = false;

        internal virtual bool UseScissor => true;

        internal void Draw()
        {
            bool usedScissors = UseScissor;
            if (usedScissors)
                ScissorManager.EnterScissor(Dimensions.X, Dimensions.Y, Dimensions.W, Dimensions.H);

            DrawElement();

            if (usedScissors)
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

        /*
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
        */

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