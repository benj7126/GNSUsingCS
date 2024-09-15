using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.ConfigAttributes
{
    /*
    
    Bool,
    Int,
    Float,
    Vector2,
    IRange,
    FRange,
    V2Range,
    Color,
    SingleLineString,
    MultiLineString,
    Enum,
    SubElement,

    */

    internal abstract class ConfigAttribute : Attribute
    {
        public abstract object GetValue();
        public abstract void SetValue(object o);

        public abstract string SaveToString();

        public abstract void LoadFromString(string loadstring);
    }

    internal class Bool : ConfigAttribute
    {
        private bool _value;

        public override object GetValue()
        {
            return _value;
        }

        public override void SetValue(object o)
        {
            _value = (bool)o;
        }

        public override string SaveToString()
        {
            return _value ? "true" : "false";
        }

        public override void LoadFromString(string loadstring)
        {
            loadstring = loadstring.ToLower();
            if (loadstring == "true")
                _value = true;
            else if (loadstring == "false")
                _value = false;
            else
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");
        }
    }

    internal class Int : ConfigAttribute
    {
        protected int _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object o)
        {
            _value = (int)o;
        }

        public override string SaveToString()
        {
            return _value.ToString();
        }

        public override void LoadFromString(string loadstring)
        {
            if (!int.TryParse(loadstring, out _value))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");
        }
    }

    internal class Float : ConfigAttribute
    {
        protected float _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object o)
        {
            _value = (float)o;
        }

        public override string SaveToString()
        {
            return _value.ToString();
        }

        public override void LoadFromString(string loadstring)
        {
            if (!float.TryParse(loadstring, out _value))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");
        }
    }

    internal class Vector2 : ConfigAttribute
    {
        protected System.Numerics.Vector2 _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object o)
        {
            _value = (System.Numerics.Vector2)o;
        }

        public override string SaveToString()
        {
            return _value.X.ToString() + ", " + _value.Y.ToString();
        }

        public override void LoadFromString(string loadstring)
        {
            string[] strings = loadstring.Replace(" ", "").Split(",");
            if (strings.Length != 2)
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

            _value = new System.Numerics.Vector2();

            if (float.TryParse(strings[0], out _value.X))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

            if (float.TryParse(strings[1], out _value.Y))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");
        }
    }

    internal class IRange(int min, int max) : Int
    {
        private new int _value = min;
    }

    internal class FRange(float min, float max) : Float
    {
        private new float _value = min;
    }

    internal class V2Range(System.Numerics.Vector2 min, System.Numerics.Vector2 max) : Vector2
    {
        private new System.Numerics.Vector2 _value = min;
    }

    internal class Color : ConfigAttribute
    {
        private Raylib_cs.Color _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object o)
        {
            _value = (Raylib_cs.Color)o;
        }

        public override string SaveToString()
        {
            return _value.R.ToString() + ", " + _value.G.ToString() + ", " + _value.B.ToString() + (_value.A == 255 ? "" : _value.A.ToString());
        }

        public override void LoadFromString(string loadstring)
        {
            string[] strings = loadstring.Replace(" ", "").Split(",");
            if (strings.Length < 3 || strings.Length > 4)
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

            _value = new Raylib_cs.Color();

            if (byte.TryParse(strings[0], out _value.R))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

            if (byte.TryParse(strings[1], out _value.G))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

            if (byte.TryParse(strings[2], out _value.B))
                throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

            if (strings.Length == 3)
                _value.A = 255;
            else
                if (byte.TryParse(strings[3], out _value.A))
                    throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");
        }
    }

    internal abstract class String : ConfigAttribute
    {
        protected string _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object o)
        {
            _value = (string)o;
        }

        public override string SaveToString()
        {
            return _value; // there needs to be some god-like save strategy going on here...
                           // either that or store it in a sepperate file using that zip-like thing lasse mentioned.
        }

        public override void LoadFromString(string loadstring)
        {
            _value = loadstring;
        }
    }

    internal class SingleLineString : String
    {
    }

    internal class MultiLineString : String
    {
    }

    internal class Enum : ConfigAttribute
    {
        private object _value;

        public override object GetValue()
        {
            return _value;
        }
        public override void SetValue(object o)
        {
            _value = o;
        }

        public override string SaveToString()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromString(string loadstring)
        {
            throw new NotImplementedException();
        }
    }

    internal class SubElement : ConfigAttribute
    {
        private ElementSettingsInstance esi;
        public override void SetValue(object o)
        {
            esi = new ElementSettingsInstance((Element)o);
        }

        public override object GetValue()
        {
            return esi.CreateElementFrom();
        }

        public override string SaveToString()
        {
            throw new NotImplementedException();
        }

        public override void LoadFromString(string loadstring)
        {
            throw new NotImplementedException();
        }
    }

    internal class ChildElements
    {
        // type of the child, and ofc its data/stuff
    }
}
