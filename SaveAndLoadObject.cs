using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal class DataObject // the relevant thing is i can change it with this structure - though if i switch to tags it might be a pain, but oh well.
    {
        protected List<string> dataBits = [];
    }
    internal class SaveObject : DataObject
    {
        public void Write(string data)
        {
            dataBits.Add(data);
        }

        /// <summary>
        /// Just runs ToString on whatever it gets.
        /// </summary>
        /// <param name="data"></param>
        public void Write(object data)
        {
            Write(data.ToString());
        }

        public string GetData()
        {
            string data = "";

            foreach (string bit in dataBits)
            {
                data += bit.Length + " " + bit;
            }

            return data;
        }
    }
    internal class LoadObject(string data) : DataObject
    {
        public string Read()
        {
            int spaceIdx = data.IndexOf(' ');

            if (spaceIdx == -1)
                return "";

            int size = int.Parse(data[..spaceIdx]);

            string dataBit = data[(spaceIdx + 1)..(spaceIdx + size + 1)];

            data = data[(spaceIdx + size + 1)..];

            return dataBit;
        }

        public int ReadInt()
        {
            return int.Parse(Read());
        }

        public float ReadFloat()
        {
            return float.Parse(Read());
        }
    }
}
