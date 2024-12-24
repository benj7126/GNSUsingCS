using GNSUsingCS.Elements;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Tabs
{
    internal class WorkspaceTab : Tab
    {
        public List<Element> Elements = new List<Element>();
        private string code; // code should be based on workspace type, and types should contain code...

        public string Code
        {
            get { return code; }
        }

        public string WorkspaceName = "a workspace";
        public override string Name => WorkspaceName;

        public void SetCode(string code) // should probably have this in element as well
        {
            this.code = code;
            // reload code
        }

        public override void Update()
        {
            Elements.ForEach(e => e.Update());
        }

        public override void Draw(int x, int y, int w, int h)
        {
            base.Draw(x, y, w, h);

            Elements.ForEach(e => e.Draw());
        }

        public override void Resize(int x, int y, int w, int h)
        {
            Elements.ForEach(e => e.Recalculate(x, y, w, h));
        }

        public override void MouseCaptured(int x, int y)
        {
            base.MouseCaptured(x, y);

            for (int i = Elements.Count - 1; i >= 0; i--)
            {
                if (Elements[i].MouseCaptured(x, y))
                {
                    return;
                }
            }
        }

        public void SaveWorkspace()
        {
            using (FileStream stream = new FileStream(SaveAndLoadManager.RelativePath + "\\" + WorkspaceName + ".wksp", FileMode.Create))
            using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create))
            {
                ZipArchiveEntry codeEntry = archive.CreateEntry("code.txt");
                using (StreamWriter writer = new StreamWriter(codeEntry.Open()))
                {
                    writer.Write(code);
                }

                if (Elements.Count > 0)
                {
                    string elements = "";
                    foreach (Element e in Elements)
                    {
                        ElementSettingsInstance esi = new ElementSettingsInstance(e);
                        esi.SaveInstance("Elements\\" + e.ID + "\\", archive);

                        elements += e.ID + '\n';
                    }

                    elements = elements[..^1];

                    ZipArchiveEntry elementIndex = archive.CreateEntry("Elements\\elementIndex.txt");
                    using (StreamWriter writer = new StreamWriter(elementIndex.Open()))
                    {
                        writer.Write(elements);
                    }
                }
            }
        }

        public WorkspaceTab GetWorkspace(string path)
        {
            WorkspaceTab tab = new WorkspaceTab();

            /*
            using (StreamReader outputFile = new StreamReader(path))
            {
                tab.SetCode(outputFile.ReadToEnd());
                tab.WorkspaceName = path.Substring(path.Length - 4, 3);
            }
            */

            return tab;
        }
    }
}
