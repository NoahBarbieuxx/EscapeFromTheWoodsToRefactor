using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.BL.Models
{
    public class XYBoundary
    {
        public XYBoundary(int xmin, int xmax, int ymin, int ymax)
        {
            Xmin = xmin;
            Xmax = xmax;
            Ymin = ymin;
            Ymax = ymax;
        }

        public int Xmin { get; set; }
        public int Xmax { get; set; }
        public int Ymin { get; set; }
        public int Ymax { get; set; }
        public int DX { get => Xmax - Xmin; }
        public int DY { get => Ymax - Ymin; }

        public bool WithinBounds(double x, double y)
        {
            if ((x < Xmin) || (x > Xmax) || (y < Ymin) || (y > Ymax))
            {
                return false;
            }
            return true;
        }
    }
}