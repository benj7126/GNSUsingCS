using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    public static class SaveAndLoadManager // i like the Data, Save and Load Objects better, so i will probably just remove this.
    {
        public static string RelativePath; // gets set in Program.cs

        internal static void SaveTab(Tab tab)
        {
            string path = RelativePath + "\\Tabs\\" + tab.ID;
            string tempPath = path + "_tmp";

            // SaveObject so = new();
            // tab.SaveData(ref so);

            string data = tab.GetType().FullName;// + " " + so.GetData();

            if (!Directory.Exists(RelativePath + "\\Tabs")) // should probably make custom file writing functions that do this.
            {
                Directory.CreateDirectory(RelativePath + "\\Tabs");
                return;
            }

            File.WriteAllText(tempPath, data);

            if (!(File.ReadAllText(tempPath) == data))
            {
                // write some error thing
                return;
            }

            // safely written data so rename
            File.Delete(path);
            File.Move(tempPath, path);
        }

        internal static Tab LoadTab(string uuid)
        {
            string path = RelativePath + "\\Tabs\\" + uuid;

            if (!File.Exists(path))
                return null;

            string fullData = File.ReadAllText(path);

            int typeNameIdx = fullData.IndexOf(' ');

            string typeName = fullData[..typeNameIdx];
            string data = fullData[(typeNameIdx+1)..];

            Type t = typeof(Tab).Assembly.GetType(typeName);
            if (t is null)
            {
                // type not found, log error
                return null;
            }

            Tab tab = (Tab)Activator.CreateInstance(t);

            tab.ID = uuid;

            // LoadObject lo = new LoadObject(data);
            // tab.LoadData(ref lo);

            return tab;
        }
    }
}
