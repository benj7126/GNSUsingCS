using Raylib_cs;
using System;
using System.IO;
using System.IO.Compression;
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
        const string sourceName = "source.elm";

        private Type baseType;
        private Dictionary<string, ConfigAttributes.ConfigAttribute> fieldToAttribute = [];
        private List<ConfigAttributes.ChildElements> childElements;

        public StyleDimension Left = new();
        public StyleDimension Top = new();

        public StyleDimension Width = new();
        public StyleDimension Height = new();

        public ElementSettingsInstance(Element toCreateFrom)
        {
            baseType = toCreateFrom.GetType();

            Left = toCreateFrom.Dimensions.Left.CreateClone();
            Top = toCreateFrom.Dimensions.Top.CreateClone();

            Width = toCreateFrom.Dimensions.Width.CreateClone();
            Height = toCreateFrom.Dimensions.Height.CreateClone();

            foreach (FieldInfo field in baseType.GetFields())
            {
                ConfigAttributes.ConfigAttribute ca = (ConfigAttributes.ConfigAttribute)field.GetCustomAttribute(typeof(ConfigAttributes.ConfigAttribute));
                if (ca is null)
                    continue;

                ca.SetValue(field.GetValue(toCreateFrom), field.Name);

                fieldToAttribute.Add(field.Name, ca);
            }
        }

        public Element CreateElementFrom()
        {
            Element element = (Element)Activator.CreateInstance(baseType);

            element.Dimensions.Left = Left.CreateClone();
            element.Dimensions.Top = Top.CreateClone();

            element.Dimensions.Width = Width.CreateClone();
            element.Dimensions.Height = Height.CreateClone();

            if (element is null)
                throw new Exception("Element settings instance was not created from an Element"); // shouldnt be possible...

            foreach (KeyValuePair<string, ConfigAttributes.ConfigAttribute> keyValuePair in fieldToAttribute)
            {
                baseType.GetField(keyValuePair.Key).SetValue(element, keyValuePair.Value.GetValue());
            }

            return element;
        }

        public void SaveInstance(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                SaveInstance("", archive);
            }
        }

        public void SaveInstance(string path, ZipArchive archive)
        {
            // needs to save baseType and childElements as well
            // should maby indent for the attributes?? - though that might look really weird...
            string saveString = "BaseType: " + baseType + "\n";

            foreach (KeyValuePair<string, ConfigAttributes.ConfigAttribute> keyValuePair in fieldToAttribute)
            {
                saveString += keyValuePair.Key + ": " + keyValuePair.Value.SaveToString(path, archive) + "\n";
            }

            ZipArchiveEntry contentEntry = archive.CreateEntry(path + sourceName);
            using (StreamWriter writer = new StreamWriter(contentEntry.Open()))
            {
                writer.Write(saveString);
            }
        }

        private void LoadInstance(Dictionary<string, string> fields, string path, ZipArchive archive)
        {
            foreach (KeyValuePair<string, ConfigAttributes.ConfigAttribute> keyValuePair in fieldToAttribute)
            {
                if (fields.ContainsKey(keyValuePair.Key))
                {
                    keyValuePair.Value.LoadFromString(fields[keyValuePair.Key], path, archive);
                }
            }
        }

        public static ElementSettingsInstance CreateInstance(string path)
        {
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                return CreateInstance(archive, "");
            }

            return null;
        }
        public static ElementSettingsInstance CreateInstance(ZipArchive archive, string path)
        {
            ElementSettingsInstance esi;

            string fullPath = path + sourceName;

            using (StreamReader reader = new StreamReader(archive.GetEntry(fullPath).Open()))
            {
                string[] source = reader.ReadToEnd().Split("\n");

                Dictionary<string, string> fields = new();

                foreach (string entry in source)
                {
                    int idx = entry.IndexOf(":");

                    if (idx < 0)
                        continue;

                    fields.Add(entry[..idx], entry[(idx + 1)..]);
                }

                if (!fields.ContainsKey("BaseType"))
                    throw new Exception("Missing type");

                Type t = typeof(ElementSettingsInstance).Assembly.GetType(fields["BaseType"]);
                if (t is null)
                    throw new Exception("Type not found");
                fields.Remove("BaseType");

                esi = new ElementSettingsInstance((Element)Activator.CreateInstance(t));

                esi.LoadInstance(fields, path, archive);
            }

            return esi;
        }
    }
}
