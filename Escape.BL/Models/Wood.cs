using Escape.DL.Models;
using Escape.DL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Escape.BL.Models
{
    public class Wood
    {
        public Wood(int woodID, List<Tree> trees, Map map, string path, MongoDBrepository db, GridDataSet gridDataSet)
        {
            WoodID = woodID;
            Trees = trees;
            Monkeys = new List<Monkey>();
            Map = map;
            _path = path;
            Db = db;
            GridDataSet = gridDataSet;
        }

        public int WoodID { get; set; }
        public List<Tree> Trees { get; set; }
        public List<Monkey> Monkeys { get; set; }
        public Map Map { get; set; }
        public string _path { get; set; }
        public MongoDBrepository Db { get; set; }
        public GridDataSet GridDataSet { get; set; }

        public void PlaceMonkey(string monkeyName, int monkeyID)
        {
            Random random = new Random(0);

            int treeIndex;
            do
            {
                treeIndex = random.Next(0, Trees.Count);
            }
            while (Trees[treeIndex].HasMonkey);

            Monkey monkey = new Monkey(monkeyID, monkeyName, Trees[treeIndex]);
            Monkeys.Add(monkey);

            Trees[treeIndex].HasMonkey = true;
        }

        public void Escape()
        {
            Dictionary<int, List<Tree>> escapeRoutes = new Dictionary<int, List<Tree>>();
            foreach (Monkey monkey in Monkeys)
            {
                List<Tree> escapeRoute = EscapeMonkey(monkey);
                escapeRoutes.Add(monkey.MonkeyID, escapeRoute);
            }

            WriteEscaperoutesToBitmap(escapeRoutes.Values.ToList());
            WriteMonkeyLocationsToLog(escapeRoutes);
            WriteMonkeyLocationsToDB(escapeRoutes);
        }

        public List<Tree> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{WoodID}: Start {WoodID}, {monkey.Name}");

            Dictionary<int, bool> visited = Trees.ToDictionary(x => x.TreeID, x => false);

            List<Tree> route = new List<Tree> { monkey.Tree };
            do
            {
                visited[monkey.Tree.TreeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                double monkeyX = monkey.Tree.X;
                double monkeyY = monkey.Tree.Y;

                foreach (Tree tree in Trees.Where(tree => !visited[tree.TreeID] && !tree.HasMonkey))
                {
                    double distance = Math.Sqrt(Math.Pow(tree.X - monkeyX, 2) + Math.Pow(tree.Y - monkeyY, 2));

                    if (distanceToMonkey.ContainsKey(distance))
                    {
                        distanceToMonkey[distance].Add(tree);
                    }
                    else
                    {
                        distanceToMonkey.Add(distance, new List<Tree>() { tree });
                    }
                }

                double distanceToBorder = new List<double>
                {
                    Map.Ymax - monkey.Tree.Y,
                    Map.Xmax - monkey.Tree.X,
                    monkey.Tree.Y - Map.Ymin,
                    monkey.Tree.X - Map.Xmin
                }.Min();

                if (distanceToMonkey.Count == 0 || distanceToBorder < distanceToMonkey.First().Key)
                {
                    WriteRouteToDB(monkey, route);

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{WoodID}: End {WoodID}, {monkey.Name}");

                    return route;
                }

                Tree nextTree = distanceToMonkey.First().Value.First();
                route.Add(nextTree);
                monkey.Tree = nextTree;
            }
            while (true);
        }

        //public List<Tree> EscapeMonkey(Monkey monkey)
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine($"{WoodID}: Start {WoodID}, {monkey.Name}");

        //    Dictionary<int, bool> visited = Trees.ToDictionary(x => x.TreeID, x => false);

        //    List<Tree> route = new List<Tree> { monkey.Tree };
        //    do
        //    {
        //        visited[monkey.Tree.TreeID] = true;

        //        // Gebruik de FindNearestTree-methode om de dichtstbijzijnde bomen te vinden
        //        List<Tree> nearestTrees = FindNearestTree(monkey.Tree.X, monkey.Tree.Y, 5);

        //        double distanceToBorder = new List<double>
        //        {
        //            Map.Ymax - monkey.Tree.Y,
        //            Map.Xmax - monkey.Tree.X,
        //            monkey.Tree.Y - Map.Ymin,
        //            monkey.Tree.X - Map.Xmin
        //        }.Min();

        //        if (nearestTrees == null || !nearestTrees.Any() || distanceToBorder < CalculateDistance(monkey.Tree, nearestTrees.First())) // Aangepaste voorwaarde, je kunt hier een geschikte drempelwaarde instellen
        //        {
        //            WriteRouteToDB(monkey, route);

        //            Console.ForegroundColor = ConsoleColor.White;
        //            Console.WriteLine($"{WoodID}: End {WoodID}, {monkey.Name}");

        //            return route;
        //        }

        //        Tree nearestTree = nearestTrees.First(); // Kies de eerste boom uit de lijst (je kunt de logica hier aanpassen indien nodig)
        //        route.Add(nearestTree);
        //        monkey.Tree = nearestTree;
        //    }
        //    while (true);
        //}

        //public List<Tree> EscapeMonkey(Monkey monkey)
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine($"{WoodID}: Start {WoodID}, {monkey.Name}");

        //    Dictionary<int, bool> visited = Trees.ToDictionary(x => x.TreeID, x => false);

        //    List<Tree> route = new List<Tree> { monkey.Tree };
        //    do
        //    {
        //        visited[monkey.Tree.TreeID] = true;

        //        int monkeyX = monkey.Tree.X;
        //        int monkeyY = monkey.Tree.Y;

        //        List<Tree> nearestTrees = FindNearestTree(monkeyX, monkeyY, 10);
        //        Tree firstTree = nearestTrees.First();

        //        double distanceToBorder = new List<double>
        //        {
        //            Map.Ymax - monkey.Tree.Y,
        //            Map.Xmax - monkey.Tree.X,
        //            monkey.Tree.Y - Map.Ymin,
        //            monkey.Tree.X - Map.Xmin
        //        }.Min();

        //        if (nearestTrees.Count == 0 || distanceToBorder < CalculateDistance(monkey.Tree, firstTree))
        //        {
        //            WriteRouteToDB(monkey, route);

        //            Console.ForegroundColor = ConsoleColor.White;
        //            Console.WriteLine($"{WoodID}: End {WoodID}, {monkey.Name}");

        //            return route;
        //        }

        //        Tree nextTree = nearestTrees.First();
        //        route.Add(nextTree);
        //        monkey.Tree = nextTree;
        //    }
        //    while (true);
        //}

        private double CalculateDistance(Tree tree1, Tree tree2)
        {
            return Math.Sqrt(Math.Pow(tree1.X - tree2.X, 2) + Math.Pow(tree1.Y - tree2.Y, 2));
        }


        public List<Tree> FindNearestTree(int x, int y, int n)
        {
            SortedList<double, List<Tree>> nn = new SortedList<double, List<Tree>>();

            (int i, int j) = FindCell(x, y);

            ProcessCell(nn, i, j, x, y, n);

            int ring = 0;

            while (nn.Count < n)
            {
                ring++;
                ProcessRing(i, j, ring, nn, x, y, n);
            }

            int n_rings = 1;
            if (ring > 0) n_rings = (int)Math.Ceiling(Math.Sqrt(2) * ring) - ring;
            for (int extraRings = 1; extraRings <= n_rings; extraRings++)
            {
                ProcessRing(i, j, ring + extraRings, nn, x, y, n);
            }

            return (List<Tree>)ListFromSortedList(nn).Take(n).ToList();
        }

        private List<Tree> ListFromSortedList(SortedList<double, List<Tree>> nn)
        {
            List<Tree> list = new List<Tree>();
            foreach (List<Tree> l in nn.Values)
            {
                foreach (Tree v in l) list.Add(v);
            }
            return list;
        }

        private (int, int) FindCell(int x, int y)
        {
            if (!GridDataSet.XYBoundary.WithinBounds(x, y))
                throw new ArgumentException();
            int i = (int)((x - GridDataSet.XYBoundary.Xmin) / GridDataSet.Delta);
            int j = (int)((y - GridDataSet.XYBoundary.Ymin) / GridDataSet.Delta);
            if (i == GridDataSet.NX) i--;
            if (j == GridDataSet.NY) j--;
            return (i, j);
        }

        private void ProcessCell(SortedList<double, List<Tree>> nn, int i, int j, int x, int y, int n)
        {
            foreach (Tree tree in GridDataSet.GridData[i][j])
            {
                double dSquare = Math.Pow(tree.X - x, 2) + Math.Pow(tree.Y - y, 2);
                if ((nn.Count < n) || (dSquare < nn.Keys[nn.Count - 1]))
                {
                    if (nn.ContainsKey(dSquare)) nn[dSquare].Add(tree);
                    else nn.Add(dSquare, new List<Tree>() { tree });
                }
            }
        }

        private void ProcessRing(int i, int j, int ring, SortedList<double, List<Tree>> nn, int x, int y, int n)
        {
            for (int gx = i - ring; gx <= i + ring; gx++)
            {
                int gy = j - ring;
                if (IsValidCell(gx, gy)) ProcessCell(nn, i, j, x, y, n);

                gy = j + ring;
                if (IsValidCell(gx, gy)) ProcessCell(nn, i, j, x, y, n);
            }
            for (int gy = j - ring; gy <= j + ring; gy++)
            {
                int gx = i - ring;
                if (IsValidCell(gx, gy)) ProcessCell(nn, i, j, x, y, n);

                gx = i + ring;
                if (IsValidCell(gx, gy)) ProcessCell(nn, i, j, x, y, n);
            }
        }

        private bool IsValidCell(int i, int j)
        {
            if ((i < 0) || (i >= GridDataSet.NX)) return false;
            if ((j < 0) || (j >= GridDataSet.NY)) return false;
            return true;
        }

        public void WriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{WoodID}: Writing wood in database for WoodID: {WoodID} - Start");

            List<Task> tasks = new List<Task>();
            List<WoodRecords> records = new List<WoodRecords>();
            foreach (Tree tree in Trees)
            {
                WoodRecords dBWoodRecord = new WoodRecords
                {
                    WoodID = WoodID,
                    TreeID = tree.TreeID,
                    X = tree.X,
                    Y = tree.Y
                };
                records.Add(dBWoodRecord);
            }
            Db.WriteWoodRecords(records);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{WoodID}: Writing wood in database for WoodID: {WoodID} - End");
        }

        private void WriteRouteToDB(Monkey monkey, List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{WoodID}: Writing routes in database for WoodID: {WoodID}, Monkey: {monkey.Name} - Start");

            List<MonkeyRecords> records = new List<MonkeyRecords>();
            for (int seqNr = 0; seqNr < route.Count; seqNr++)
            {
                Tree currentTree = route[seqNr];

                MonkeyRecords monkeyRecord = new MonkeyRecords
                {
                    MonkeyID = monkey.MonkeyID,
                    MonkeyName = monkey.Name,
                    WoodID = WoodID,
                    SeqNr = seqNr,
                    TreeID = currentTree.TreeID,
                    X = currentTree.X,
                    Y = currentTree.Y
                };
                records.Add(monkeyRecord);
            }

            Db.WriteMonkeyRecords(records);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{WoodID}: Writing routes in database for WoodID: {WoodID}, Monkey: {monkey.Name} - End");
        }

        public void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            const int drawingFactor = 8;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{WoodID}: Writing bitmap routes for WoodID: {WoodID} - Start");

            Color[] colors = { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };

            Bitmap bitmap = new Bitmap((Map.Xmax - Map.Xmin) * drawingFactor, (Map.Ymax - Map.Ymin) * drawingFactor);
            Graphics graphics = Graphics.FromImage(bitmap);

            int delta = drawingFactor / 2;
            Pen pen = new Pen(Color.Green, 1);

            foreach (Tree tree in Trees)
            {
                graphics.DrawEllipse(pen, tree.X * drawingFactor, tree.Y * drawingFactor, drawingFactor, drawingFactor);
            }

            int colorIndex = 0;
            foreach (List<Tree> route in routes)
            {
                int startX = route[0].X * drawingFactor + delta;
                int startY = route[0].Y * drawingFactor + delta;

                Color color = colors[colorIndex % colors.Length];
                Pen routePen = new Pen(color, 1);

                graphics.DrawEllipse(routePen, startX - delta, startY - delta, drawingFactor, drawingFactor);
                graphics.FillEllipse(new SolidBrush(color), startX - delta, startY - delta, drawingFactor, drawingFactor);

                for (int i = 1; i < route.Count; i++)
                {
                    int endX = route[i].X * drawingFactor + delta;
                    int endY = route[i].Y * drawingFactor + delta;

                    graphics.DrawLine(routePen, startX, startY, endX, endY);
                    startX = endX;
                    startY = endY;
                }
                colorIndex++;
            }
            bitmap.Save(Path.Combine(_path, WoodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{WoodID}: Writing bitmap routes for WoodID: {WoodID} - End");
        }

        public void WriteMonkeyLocationsToLog(Dictionary<int, List<Tree>> escapeRoutes)
        {
            string logFolderPath = Path.Combine("C:\\Users\\barbi\\Documents\\hogent\\sem3\\6 - Solutions\\EscapeFromTheWoodsToRefactor\\Escape.Logs");
            string logFilePath = Path.Combine(logFolderPath, WoodID.ToString() + "_MonkeyLocationsLog.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                int maxLength = escapeRoutes.Max(r => r.Value.Count);
                int j = 0;
                bool continueLoop = true;

                while (continueLoop)
                {

                    foreach (var route in escapeRoutes)
                    {
                        if (j < route.Value.Count)
                        {
                            Tree tree = route.Value[j];
                            Monkey monkey = Monkeys.Single(m => m.MonkeyID == route.Key);

                            monkey.Tree = tree;

                            writer.WriteLine($"{monkey.Name} is in tree {tree.TreeID} at ({tree.X},{tree.Y})");
                        }

                        if (j >= maxLength)
                        {
                            continueLoop = false;
                        }
                    }

                    j++;
                }
            }
        }

        public void WriteMonkeyLocationsToDB(Dictionary<int, List<Tree>> escapeRoutes)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{WoodID}: Writing logs in database for monkeys - Start");

            List<Logs> logs = new List<Logs>();

            foreach (var route in escapeRoutes)
            {
                int monkeyId = route.Key;

                if (Monkeys.Any(m => m.MonkeyID == monkeyId))
                {
                    for (int seqNr = 0; seqNr < route.Value.Count; seqNr++)
                    {
                        Tree currentTree = route.Value[seqNr];

                        int logId = IDgenerator.GetLogID();

                        Logs log = new Logs
                        {
                            LogId = logId,
                            WoodID = WoodID,
                            MonkeyId = monkeyId,
                            Message = $"{Monkeys.First(m => m.MonkeyID == monkeyId).Name} is now in tree {currentTree.TreeID} at ({currentTree.X},{currentTree.Y})"
                        };
                        logs.Add(log);
                    }
                }
                else
                {
                    Console.WriteLine($"Error: Monkey with ID {monkeyId} not found in the Monkeys list.");
                }
            }

            Db.WriteMonkeyLogs(logs);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{WoodID}: Writing logs in database for monkeys - End");
        }

    }
}