using Raylib_cs;
using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace GNSUsingCS
{
    internal class ElementSettingsInstance
    {
        private Dictionary<string, ConfigAttributes.ConfigAttribute> fieldToAttribute = [];

        public ElementSettingsInstance(Element toCreateFrom)
        {
            foreach (FieldInfo field in toCreateFrom.GetType().GetFields())
            {
                ConfigAttributes.ConfigAttribute ca = (ConfigAttributes.ConfigAttribute)field.GetCustomAttribute(typeof(ConfigAttributes.ConfigAttribute));
                if (ca is null)
                    continue;

                fieldToAttribute.Add(field.Name, ca);
            }
        }

        public void PasteOnto(Element toPasteOnto)
        {

        }

        public void SaveInstance()
        {

        }
        
        public static ElementSettingsInstance LoadInstance()
        {
            return null;
        }
    }
}
