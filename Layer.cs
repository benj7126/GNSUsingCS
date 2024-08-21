using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS
{
    internal abstract class Layer
    {
        public virtual void Draw(int x, int y, int w, int h) { }
        public virtual void Resize(int x, int y, int w, int h) { }
        public virtual void Update() { }
        public virtual void PreUpdate() { }
        public virtual void Save() { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if the click was captured in this layer</returns>
        public virtual bool MouseCaptured(int x, int y) { return false; }
    }
}
