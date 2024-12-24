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
    public static class LuaInterfacer
    {
        public static Lua L;

        public static void SetupLuaInterfacer()
        {
            L = new();

            L.SetObjectToPath("print", FakePrint);

            L.DoString("""
                local exceptions = { "ObjectID", "ClearObjectCode", "print" }
                local getSetExceptions = { "G" }

                ObjectID = ""

                function ClearElementCode()
                    rawset(_G, ObjectID, {})
                end
                
                _G.G = {}

                setmetatable(_G, {
                    __newindex = function (t, k, v)
                        for exceptionK, v in pairs(getSetExceptions) do
                            if exceptionK == k then
                                return rawset(t, k, v)
                            end
                        end

                        rawset(rawget(t, ObjectID), k, v)
                    end,
                    __index = function (t, k)
                        if k == "this" then
                            return GetCurElement()
                        end
                
                        for exceptionK, v in pairs(exceptions) do
                            if exceptionK == k then
                                return rawrget(t, k)
                            end
                        end
                
                        for exceptionK, v in pairs(getSetExceptions) do
                            if exceptionK == k then
                                return rawrget(t, k)
                            end
                        end
                
                        local element = rawget(t, ObjectID)
                        
                        if (element == nil) then
                            return nil
                        end
                        
                        return rawget(element, k);
                    end,
                })
                """);
        }

        private static FieldInfo reference = typeof(LuaBase).GetField("_Reference", BindingFlags.NonPublic | BindingFlags.Instance);
        private static void FakePrint(params object[] objs)
        { // make custom console - also let you change the console to a textbox - so you can run code within if you want to
            bool start = true;
            foreach (object obj in objs)
            {
                if (!start)
                    Console.Write("\t");

                Console.Write(obj is null ? "nil" : (obj is LuaBase lobj ? lobj.ToString() + "-" + reference.GetValue(lobj) : obj));

                start = false;
            }
            Console.Write("\n");
        }

        public static void SetElementCode(string id, string text)
        {
            L.SetObjectToPath("ObjectID", id);
            L.GetFunction("ClearElementCode").Call();
            L.DoString(text);
        }

        public static void TryCallMethod(string id, string name, params object[] args)
        {
            L.SetObjectToPath("ObjectID", id);
            if (L[name] is not null)
                L.GetFunction(name).Call(args);
        }
    }
}
