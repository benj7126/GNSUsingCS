using Raylib_cs;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;


namespace GNSUsingCS
{
    // should be able to tell an object that its of ths instance and only save the values that differ from this.
    // would make some things a lot easier, i'd say...
    internal class ElementSettingsInstance
    {
        private Type baseType;
        private Dictionary<string, ConfigAttributes.ConfigAttribute> fieldToAttribute = [];
        private List<ConfigAttributes.ChildElements> childElements;

        public ElementSettingsInstance(Element toCreateFrom)
        {
            baseType = toCreateFrom.GetType();
            foreach (FieldInfo field in baseType.GetFields())
            {
                ConfigAttributes.ConfigAttribute ca = (ConfigAttributes.ConfigAttribute)field.GetCustomAttribute(typeof(ConfigAttributes.ConfigAttribute));
                if (ca is null)
                    continue;

                ca.SetValue(field.GetValue(toCreateFrom));

                fieldToAttribute.Add(field.Name, ca);
            }
        }

        public Element CreateElementFrom()
        {
            Element element = (Element)Activator.CreateInstance(baseType);

            if (element is null)
                throw new Exception("Element settings instance was not created from an Element"); // shouldnt be possible...

            foreach (KeyValuePair<string, ConfigAttributes.ConfigAttribute> keyValuePair in fieldToAttribute)
            {
                baseType.GetField(keyValuePair.Key).SetValue(element, keyValuePair.Value.GetValue());
            }

            return element;
        }

        public string SaveInstance()
        {
            // needs to save baseType and childElements as well
            // should maby indent for the attributes?? - though that might look really weird...
            string s = "";

            foreach (KeyValuePair<string, ConfigAttributes.ConfigAttribute> keyValuePair in fieldToAttribute)
            {
                s += keyValuePair.Key + ": " + keyValuePair.Value.SaveToString() + "\n";
            }

            return s;
        }
        
        public static ElementSettingsInstance LoadInstance()
        {
            return null;
        }
    }
}
