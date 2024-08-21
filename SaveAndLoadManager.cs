using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    public static class SaveAndLoadManager
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
    }
}
