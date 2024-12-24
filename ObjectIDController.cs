using GNSUsingCS.Elements;
using GNSUsingCS.Tabs;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GNSUsingCS
{
    internal static class ObjectIDController
    {
        internal static ObjectIDController<WorkspaceTab> Workspace = new("", s => new WorkspaceTab());
        internal static ObjectIDController<Element> Element = new("", s => new Box());

        public static void addElement(Element e) // not intended to be used; just for testing
        {
            Element.Set(e.ID, e);
        }
    }

    internal class ObjectIDController<T>(string loadPath, Func<string, T> loadMethod)
    {
        private Dictionary<string, T> objects = [];

        public T Get(string id)
        {
            if (!objects.ContainsKey(id))
            {
                objects.Add(id, loadMethod(loadPath + "\\" + id));
            }

            return objects[id];
        }
        public void Set(string id, T element)
        {
            objects.Add(id, element);
        }
    }
}
