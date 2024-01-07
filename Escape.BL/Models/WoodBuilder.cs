using Escape.DL.Repositories;

namespace Escape.BL.Models
{
    public static class WoodBuilder
    {
        public static Wood GetWood(int size, Map map, string path, MongoDBrepository db)
        {
            Random random = new Random();
            List<Tree> trees = GenerateRandomTrees(size, map);

            int woodId = IDgenerator.GetWoodID();

            XYBoundary xYBoundary = new XYBoundary(map.Xmin, map.Xmax, map.Ymin, map.Ymax);

            int x = map.Xmax;
            int y = map.Ymax;

            int nx = x - map.Xmin;
            int ny = y - map.Ymin;

            double deltaX = x / nx;
            double deltaY = y / ny;

            double delta = Math.Min(deltaX, deltaY);

            GridDataSet gridDataSet = new GridDataSet(xYBoundary, delta);
            Grid grid = new Grid(gridDataSet);

            Wood wood = new Wood(woodId, trees, map, path, db, grid);
            return wood;
        }

        public static List<Tree> GenerateRandomTrees(int size, Map map)
        {
            Random random = new Random();
            Dictionary<int, Tree> uniqueTrees = new Dictionary<int, Tree>();

            while (uniqueTrees.Count < size)
            {
                int treeId = IDgenerator.GetTreeID();
                Tree tree = new Tree(
                    treeId,
                    random.Next(map.Xmin, map.Xmax),
                    random.Next(map.Ymin, map.Ymax));

                if (!uniqueTrees.ContainsKey(treeId))
                {
                    uniqueTrees.Add(treeId, tree);
                }
            }
            return uniqueTrees.Values.ToList();
        }
    }
}