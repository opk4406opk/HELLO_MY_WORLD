using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace MapTool.Source
{
    class MapViewer : GameWindow
    {
        private string v;

        public MapViewer(int width, int height, string v) : base(width, height)
        {
            this.v = v;
        }
    }
}
