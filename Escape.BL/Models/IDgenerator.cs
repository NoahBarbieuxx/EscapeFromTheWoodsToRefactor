﻿namespace Escape.BL.Models
{
    public static class IDgenerator
    {
        private static int treeID = 0;
        private static int woodID = 0;
        private static int monkeyID = 0;
        private static int logId = 0;

        public static int GetTreeID()
        {
            return treeID++;
        }
        public static int GetMonkeyID()
        {
            return monkeyID++;
        }
        public static int GetWoodID()
        {
            return woodID++;
        }

        public static int GetLogID()
        {
            return logId++;
        }
    }
}