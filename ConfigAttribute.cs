using System;
using System.Collections.Generic;
using System.IO.Compression;
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
        protected string fieldName = "";
        public abstract object GetValue();
        public void SetValue(object o,  string fieldName)
        {
            this.fieldName = fieldName;
            SetValue(o);
        }
        protected abstract void SetValue(object o);

        public abstract string SaveToString(string path, ZipArchive zipArchive); // (fileName, value) -- if fileName is "", then in origin

        public abstract void LoadFromString(string loadstring, string path, ZipArchive zipArchive);
    }

    internal class Bool : ConfigAttribute
    {
        private bool _value;

        public override object GetValue()
        {
            return _value;
        }

        protected override void SetValue(object o)
        {
            _value = (bool)o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            return _value ? "true" : "false";
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            loadstring = loadstring.Replace(" ", "").ToLower();
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
        protected override void SetValue(object o)
        {
            _value = (int)o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            return _value.ToString();
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
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
        protected override void SetValue(object o)
        {
            _value = (float)o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            return _value.ToString();
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
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
        protected override void SetValue(object o)
        {
            _value = (System.Numerics.Vector2)o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            (char, float)[] colors = { ('X', _value.X), ('Y', _value.Y) };

            string s = "";

            foreach ((char, float) c in colors)
            {
                if (c.Item2 != 0)
                    s += c.Item1 + ": " + c.Item2 + ", ";
            }

            if (s.Length > 1)
            {
                s = s.Substring(0, s.Length - 2);
            }

            return s;
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            string[] strings = loadstring.Replace(" ", "").Split(",");

            _value = new();

            Func<string, float> getValue = (str) => {
                if (float.TryParse(str, out float val))
                    throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

                return val;
            };

            Dictionary<char, Action<string>> actions = new() {
                { 'X', (str) => _value.X = getValue(str) },
                { 'Y', (str) => _value.Y = getValue(str) }
            };

            foreach (string s in strings)
                actions[s[0]](s[1..]);
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
        protected override void SetValue(object o)
        {
            _value = (Raylib_cs.Color)o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            (char, byte)[] colors = { ('R', _value.R), ('G', _value.G), ('B', _value.B), ('A', _value.A) };

            string s = "";

            foreach ((char, byte) c in colors)
            {
                if (c.Item2 != 255)
                    s += c.Item1 + ": " + c.Item2 + ", ";
            }

            if (s.Length > 1)
            {
                s = s.Substring(0, s.Length - 2); // remove ", "
            }

            return s;
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            string[] strings = loadstring.Replace(" ", "").Split(",");

            _value = new Raylib_cs.Color(255, 255, 255, 255);

            Func<string, byte> getValue = (str) => {
                if (byte.TryParse(str, out byte val))
                    throw new Exception("Should probably just be console write error or whatever, but theres a faulty string here");

                return val;
            };

            Dictionary<char, Action<string>> actions = new() {
                { 'R', (str) => _value.R = getValue(str) },
                { 'G', (str) => _value.G = getValue(str) },
                { 'B', (str) => _value.B = getValue(str) },
                { 'A', (str) => _value.A = getValue(str) }
            };

            foreach (string s in strings)
                actions[s[0]](s[1..]);
        }
    }

    internal abstract class String : ConfigAttribute
    {
        protected string _value;

        public override object GetValue()
        {
            return _value;
        }
        protected override void SetValue(object o)
        {
            _value = (string)o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            return '"'+_value+'"';  // there needs to be some god-like save strategy going on here...
                                    // either that or store it in a sepperate file using that zip-like thing lasse mentioned.
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            _value = loadstring;
        }
    }

    internal class SingleLineString : String
    {
    }

    internal class MultiLineString : String
    {
        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            string fileName = path + "Assets/" + Guid.NewGuid().ToString() + ".txt";

            ZipArchiveEntry contentEntry = zipArchive.CreateEntry(fileName);
            using (StreamWriter writer = new StreamWriter(contentEntry.Open()))
            {
                writer.Write(_value);
            }

            return fileName;
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            using (StreamReader reader = new StreamReader(path + zipArchive.GetEntry(loadstring.Replace(" ", "")).Open()))
            {
                _value = reader.ReadToEnd();
            }

        }
    }

    internal class CodeString : MultiLineString
    {
    }

    internal class Enum : ConfigAttribute
    {
        private object _value;

        public override object GetValue()
        {
            return _value;
        }
        protected override void SetValue(object o)
        {
            _value = o;
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            throw new NotImplementedException();
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            throw new NotImplementedException();
        }
    }

    internal class SubElement : ConfigAttribute
    {
        private ElementSettingsInstance esi;
        protected override void SetValue(object o)
        {
            esi = new ElementSettingsInstance((Element)o);
        }

        public override object GetValue()
        {
            return esi.CreateElementFrom();
        }

        public override string SaveToString(string path, ZipArchive zipArchive)
        {
            esi.SaveInstance(fieldName + "\\" + path, zipArchive);

            return fieldName + "\\" + path;
        }

        public override void LoadFromString(string loadstring, string path, ZipArchive zipArchive)
        {
            esi = ElementSettingsInstance.CreateInstance(zipArchive, fieldName + "\\" + path);
        }
    }

    internal class ChildElements
    {
        // type of the child, and ofc its data/stuff
    }
}
