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
    }

    internal class Bool : ConfigAttribute
    {
    }

    internal class Int : ConfigAttribute
    {
    }

    internal class Float : ConfigAttribute
    {
    }

    internal class Vector2 : ConfigAttribute
    {
    }

    internal class IRange(int min, int max) : ConfigAttribute
    {
    }

    internal class FRange(float min, float max) : ConfigAttribute
    {
    }

    internal class V2Range(Vector2 min, Vector2 max) : ConfigAttribute
    {
    }

    internal class Color : ConfigAttribute
    {
    }

    internal class SingleLineString : ConfigAttribute
    {
    }

    internal class MultiLineString : ConfigAttribute
    {
    }

    internal class Enum : ConfigAttribute
    {
    }

    internal class SubElement : ConfigAttribute
    {
    }
}
