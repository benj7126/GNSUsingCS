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

            L.SetObjectToPath("GetCurElement", GetCurElement);

            L.DoString("""
                local exceptions = { "TabUUID", "NoteUUID", "ElementIndex", "ClearElementCode", "print" }
                local getSetExceptions = { "G" }

                TabUUID = ""
                NoteUUID = ""
                ElementIndex = ""

                function ClearElementCode()
                    rawset(_G, TabUUID .. NoteUUID .. ElementIndex, {})
                end
                
                _G.G = {}

                setmetatable(_G, {
                    __newindex = function (t, k, v)
                        for exceptionK, v in pairs(getSetExceptions) do
                            if exceptionK == k then
                                return rawset(t, k, v)
                            end
                        end

                        rawset(rawget(t, TabUUID .. NoteUUID .. ElementIndex), k, v)
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
                
                        return rawget(rawget(t, TabUUID .. NoteUUID .. ElementIndex), k);
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

        private static Element curElement;
        private static Element GetCurElement()
        {
            return curElement;
        }

        public static void SetElementCode(string text)
        {
            L.GetFunction("ClearElementCode").Call();
            L.DoString(text);
        }

        public static void TryCallMethod(string name, params object[] objs)
        {
            if (L[name] is not null)
                L.GetFunction(name).Call(objs);
        }

        internal static void EnterElement(int idx, Element element)
        {
            curElement = element;
            L.SetObjectToPath("ElementIndex", idx);
        }

        public static void EnterNote(string uuid)
        {
            L.SetObjectToPath("NoteUUID", uuid);
        }

        public static void EnterTab(string uuid)
        {
            L.SetObjectToPath("TabUUID", uuid);
        }
    }
}
