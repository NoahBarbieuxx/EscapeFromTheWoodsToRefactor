using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.BL.Models
{
    public class Tree
    {
        public Tree(int treeID, int x, int y)
        {
            TreeID = treeID;
            X = x;
            Y = y;
            HasMonkey = false;
        }

        public int TreeID { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool HasMonkey { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Tree tree &&
                   X == tree.X && Y == tree.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        public override string ToString()
        {
            return $"{TreeID}, {X}, {Y}";
        }
    }
}
