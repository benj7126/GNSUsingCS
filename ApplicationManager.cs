using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GNSUsingCS.Tabs.ChoiceTab;
using Raylib_cs;

namespace GNSUsingCS
{
    internal class ApplicationManager
    {
        public static Random rand = new();

        private int _curTab = 0;
        private List<Tab> _tabs = [];

        public void AddWorkspace()
        {
            _tabs.Add(new ChoiceTab());
        }

        private List<int> tabPos = []; 
        public void Draw()
        {
            tabPos.Clear();
            int x = 0;
            for (int i = 0; i < _tabs.Count; i++)
            {
                Tab tab = _tabs[i];
                tab.DrawTab(ref x, i == _curTab);

                tabPos.Add(x);
            }

            int y = (int)Settings.TabSettings.fontSize + (int)Settings.TabSettings.padding.Y * 2;
            DrawLine(0, y, GetScreenWidth(), y, Color.Black);

            BeginScissorMode(0, y, GetScreenWidth(), GetScreenHeight() - y);

            _tabs[_curTab].Draw(0, y, GetScreenWidth(), GetScreenHeight() - y);

            EndScissorMode();
        }

        public void MouseCaptured(int x, int y)
        {
            if (y < Settings.TabSettings.fontSize + Settings.TabSettings.padding.Y * 2)
            {
                if (IsMouseButtonPressed(MouseButton.Left))
                {
                    for (int i = 0; i < tabPos.Count; i++)
                    {
                        int tx = tabPos[i];

                        if (x < tx)
                        {
                            _curTab = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                _tabs[_curTab].MouseCaptured(x, y);
            }
        }

        public void Update()
        {
            _tabs[_curTab].Update();
        }

        public void PreUpdate()
        {
            _tabs[_curTab].PreUpdate();
        }
    }
}

