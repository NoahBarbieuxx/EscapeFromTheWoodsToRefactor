namespace Escape.BL.Models
{
    public class GridDataSet
    {
        public GridDataSet(XYBoundary xYBoundary, double delta)
        {
            XYBoundary = xYBoundary;
            Delta = delta;
            NX = (int)(xYBoundary.DX / delta) + 1;
            NY = (int)((xYBoundary.DY) / delta) + 1;
            GridData = new List<Tree>[NX][];
            for (int i = 0; i < NX; i++)
            {
                GridData[i] = new List<Tree>[NY];
                for (int j = 0; j < NY; j++)
                {
                    GridData[i][j] = new List<Tree>();
                }
            }
        }

        public GridDataSet(XYBoundary xYBoundary, double delta, List<Tree> gridData) : this(xYBoundary, delta)
        {
            foreach (Tree tree in gridData)
            {
                AddXY(tree);
            }
        }

        public double Delta { get; set; }
        public List<Tree>[][] GridData { get; set; }
        public XYBoundary XYBoundary { get; set; }
        public int NX { get; set; }
        public int NY { get; set; }

        public void AddXY(Tree tree)
        {
            if ((tree.X < XYBoundary.Xmin) || (tree.X > XYBoundary.Xmax) || (tree.Y < XYBoundary.Ymin) || (tree.Y > XYBoundary.Ymax))
            {
                throw new ArgumentException();
            }
                
            int i = (int)((tree.X - XYBoundary.Xmin) / Delta);
            int j = (int)((tree.Y - XYBoundary.Ymin) / Delta);

            if (i == NX)
            {
                i--;
            }
            if (j == NY)
            {
                j--;
            }

            GridData[i][j].Add(tree);
        }
    }
}