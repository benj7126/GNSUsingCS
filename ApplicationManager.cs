using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GNSUsingCS.Tabs.ChoiceTab;
using GNSUsingCS.Tabs.WorkspaceTab;
using Raylib_cs;

namespace GNSUsingCS
{
    internal class ApplicationManager
    {
        public static ApplicationManager Instance;
        public static Random rand = new();

        private int _curTab = 0;
        private List<Tab> _tabs = [];
        public Tab CurrentTab => _tabs[_curTab];

        public ApplicationManager()
        {
            Instance = this;
        }

        public void ChoiceTab()
        {
            _tabs.Add(new ChoiceTab());
        }

        public void AddWorkspace()
        {
            _tabs.Add(new WorkspaceTab());
        }

        public void AddTab(Tab t)
        {
            _tabs.Add(t);
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

            CurrentTab.Draw(0, y, GetScreenWidth(), GetScreenHeight() - y);

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
                CurrentTab.MouseCaptured(x, y);
            }
        }

        public void Update()
        {
            CurrentTab.Update();
        }

        public void PreUpdate()
        {
            CurrentTab.PreUpdate();
        }
    }
}

