using Escape.DL.Models;
using Escape.DL.Repositories;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace Escape.BL.Models
{
    public class Wood
    {
        public Wood(int woodID, List<Tree> trees, Map map, string path, MongoDBrepository db, Grid grid)
        {
            WoodID = woodID;
            Trees = trees;
            Monkeys = new List<Monkey>();
            Map = map;
            _path = path;
            Db = db;
            Grid = grid;
        }

        public int WoodID { get; set; }
        public List<Tree> Trees { get; set; }
        public List<Monkey> Monkeys { get; set; }
        public Map Map { get; set; }
        public string _path { get; set; }
        public MongoDBrepository Db { get; set; }
        public Grid Grid { get; set; }

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

        public async Task EscapeAsync()
        {
            Dictionary<Monkey, List<Tree>> escapeRoutes = new Dictionary<Monkey, List<Tree>>();
            List<Task<List<Tree>>> escapeTasks = new List<Task<List<Tree>>>();

            foreach (Monkey monkey in Monkeys)
            {
                escapeTasks.Add(EscapeMonkeyAsync(monkey));
                //escapeTasks.Add(EscapeMonkeyAsync(monkey, Grid));
            }

            await Task.WhenAll(escapeTasks);

            for (int i = 0; i < Monkeys.Count; i++)
            {
                escapeRoutes.Add(Monkeys[i], escapeTasks[i].Result);
            }

            await WriteEscaperoutesToBitmapAsync(escapeRoutes.Values.ToList());
            await WriteMonkeyLocationsToLogAsync(escapeRoutes);
            await WriteMonkeyLocationsToDBAsync(escapeRoutes);
        }

        public async Task<List<Tree>> EscapeMonkeyAsync(Monkey monkey)
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
                    await WriteRouteToDBAsync(monkey, route);

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

        //public async Task<List<Tree>> EscapeMonkeyAsync(Monkey monkey, Grid grid)
        //{
        //    Console.ForegroundColor = ConsoleColor.White;
        //    Console.WriteLine($"{WoodID}: Start {WoodID}, {monkey.Name}");

        //    Dictionary<int, bool> visited = Trees.ToDictionary(x => x.TreeID, x => false);

        //    List<Tree> route = new List<Tree> { monkey.Tree };
        //    do
        //    {
        //        visited[monkey.Tree.TreeID] = true;
        //        List<Tree> neighbors = grid.FindNearestTree(monkey, 10)
        //                            .Where(tree => !visited[tree.TreeID] && !tree.HasMonkey)
        //                            .ToList();

        //        double distanceToBorder = new List<double>
        //        {
        //            Map.Ymax - monkey.Tree.Y,
        //            Map.Xmax - monkey.Tree.X,
        //            monkey.Tree.Y - Map.Ymin,
        //            monkey.Tree.X - Map.Xmin
        //        }.Min();

        //        if (neighbors.Count == 0 || distanceToBorder < CalculateDistance(neighbors.First(), monkey.Tree))
        //        {
        //            await WriteRouteToDBAsync(monkey, route);

        //            Console.ForegroundColor = ConsoleColor.White;
        //            Console.WriteLine($"{WoodID}: End {WoodID}, {monkey.Name}");

        //            return route;
        //        }

        //        Tree nextTree = neighbors.First();
        //        route.Add(nextTree);
        //        monkey.Tree = nextTree;
        //    }
        //    while (true);
        //}
        //private double CalculateDistance(Tree tree1, Tree tree2)
        //{
        //    return Math.Sqrt(Math.Pow(tree1.X - tree2.X, 2) + Math.Pow(tree1.Y - tree2.Y, 2));
        //}

        public async Task WriteWoodToDBAsync()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{WoodID}: Writing wood in database for WoodID: {WoodID} - Start");

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
            await Db.WriteWoodRecords(records);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{WoodID}: Writing wood in database for WoodID: {WoodID} - End");
        }

        private async Task WriteRouteToDBAsync(Monkey monkey, List<Tree> route)
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

            await Db.WriteMonkeyRecords(records);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{WoodID}: Writing routes in database for WoodID: {WoodID}, Monkey: {monkey.Name} - End");
        }

        public async Task WriteEscaperoutesToBitmapAsync(List<List<Tree>> routes)
        {
            const int drawingFactor = 8;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{WoodID}: Writing bitmap routes for WoodID: {WoodID} - Start");

            Color[] colors = { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Bitmap bitmap = new Bitmap((Map.Xmax - Map.Xmin) * drawingFactor, (Map.Ymax - Map.Ymin) * drawingFactor))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
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
                    }
                    bitmap.Save(memoryStream, ImageFormat.Jpeg);
                }
                await Task.Run(() =>
                {
                    string filePath = Path.Combine(_path, WoodID.ToString() + "_escapeRoutes.jpg");
                    File.WriteAllBytes(filePath, memoryStream.ToArray());
                });
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{WoodID}: Writing bitmap routes for WoodID: {WoodID} - End");
        }

        public async Task WriteMonkeyLocationsToLogAsync(Dictionary<Monkey, List<Tree>> escapeRoutes)
        {
            string logFolderPath = Path.Combine("C:\\Users\\barbi\\Documents\\hogent\\sem3\\6 - Solutions\\EscapeFromTheWoodsToRefactor\\Escape.Logs");
            string logFilePath = Path.Combine(logFolderPath, WoodID.ToString() + "_MonkeyLocationsLog.txt");

            Dictionary<Monkey, List<Tree>> sortedEscapeRoutes = escapeRoutes
                .OrderBy(entry => entry.Key.Name)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                int maxLength = sortedEscapeRoutes.Max(r => r.Value.Count);
                int j = 0;
                bool continueLoop = true;

                while (continueLoop)
                {
                    foreach (var route in sortedEscapeRoutes)
                    {

                        if (j < route.Value.Count)
                        {
                            Tree tree = route.Value[j];
                            Monkey monkey = route.Key;

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

        public async Task WriteMonkeyLocationsToDBAsync(Dictionary<Monkey, List<Tree>> escapeRoutes)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{WoodID}: Writing logs in database for monkeys - Start");

            List<Logs> logs = new List<Logs>();

            foreach (var route in escapeRoutes)
            {
                int monkeyId = route.Key.MonkeyID;


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

            await Db.WriteMonkeyLogs(logs);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{WoodID}: Writing logs in database for monkeys - End");
        }
    }
}