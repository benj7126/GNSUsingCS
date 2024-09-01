using GNSUsingCS.Tabs.WorkspaceTab;
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
        public static (List<string>, string) ParseArray(string text)
        {
            if (text == "")
                return ([], "");

            List<string> list = [];

            int idx = 1;
            if (text[0] == '{')
            {
                int depth = 1;

                string curString = "";
                bool inString = false;

                while (depth != 0)
                {
                    if (text[idx] == '{')
                    {
                        depth++;

                        if (depth > 1)
                            curString += text[idx];
                    }
                    else if (text[idx] == '}')
                    {
                        if (depth > 1)
                            curString += text[idx];
                        
                        depth--;
                    }
                    else if (text[idx] == '"')
                    {
                        inString = !inString;
                        curString += text[idx];
                    }
                    else if (text[idx] == ',' && depth == 1 && inString == false)
                    {
                        list.Add(curString);
                        curString = "";
                    }
                    else
                    {
                        curString += text[idx];
                    }

                    idx++;
                }
                list.Add(curString);
            }
            else
            {
                throw new Exception(text + " is presumed not to be an array but was parsed as one either way");
            }

            return (list, text[idx..]);
        }

        public static string SetupArray(List<string> arr)
        {
            if (arr.Count == 0)
                return "";

            return "{" + arr.Aggregate("", (a, s) => a + "," + s)[1..] + "}";
        }

        public static string ParseObject<T>(T obj)
        {
            if (obj == null)
                return "";

            if (typeof(T) == typeof(string))
            {
                return '"' + obj.ToString() + '"';
            }

            return "";
        }
        public static T ParseObjecTo<T>(string obj)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(obj[1..^1], typeof(T));
            }

            return default;
        }

        public static string RelativePath; // gets set in Program.cs

        internal static void SaveTab(Tab tab)
        {
            string path = RelativePath + "\\Tabs\\" + tab.UUID;
            string tempPath = path + "_tmp";

            SaveObject so = new();
            tab.SaveData(ref so);

            string data = tab.GetType().FullName + " " + so.GetData();

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

            tab.UUID = uuid;

            LoadObject lo = new LoadObject(data);
            tab.LoadData(ref lo);

            return tab;
        }
    }
}
