namespace Escape.BL.Models
{
    public class Map
    {
        public Map(int xmin, int xmax, int ymin, int ymax)
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
    }
}