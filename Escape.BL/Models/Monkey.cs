﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escape.BL.Models
{
    public class Monkey
    {
        public Monkey(int monkeyID, string name, Tree tree)
        {
            MonkeyID = monkeyID;
            Name = name;
            Tree = tree;
        }

        public int MonkeyID { get; set; }
        public string Name { get; set; }
        public Tree Tree { get; set; }
    }
}
