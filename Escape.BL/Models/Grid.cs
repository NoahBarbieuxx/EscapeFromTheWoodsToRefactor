using Escape.BL.Models;

public class Grid
{
    private List<Tree>[,] grid;
    private GridDataSet gridDataSet;

    public Grid(GridDataSet gridDataSet)
    {
        this.gridDataSet = gridDataSet;

        int nx = gridDataSet.NX;
        int ny = gridDataSet.NY;

        grid = new List<Tree>[nx, ny];

        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                grid[i, j] = new List<Tree>(gridDataSet.GridData[i][j]);
            }
        }
    }

    public List<Tree> FindNearestTree(Monkey monkey, int n)
    {
        SortedList<double, List<Tree>> nn = new SortedList<double, List<Tree>>();

        (int i, int j) = FindCell(monkey);

        ProcessCell(nn, i, j, monkey, n);

        int ring = 0;

        while (nn.Count < n)
        {
            ring++;
            ProcessRing(i, j, ring, nn, monkey, n);
        }

        int n_rings = 1;
        if (ring > 0) n_rings = (int)Math.Ceiling(Math.Sqrt(2) * ring) - ring;
        for (int extraRings = 1; extraRings <= n_rings; extraRings++)
        {
            ProcessRing(i, j, ring + extraRings, nn, monkey, n);
        }

        return (List<Tree>)ListFromSortedList(nn).Take(n).ToList();
    }

    private List<Tree> ListFromSortedList(SortedList<double, List<Tree>> nn)
    {
        List<Tree> list = new List<Tree>();
        foreach (List<Tree> l in nn.Values)
        {
            foreach (Tree v in l)
            {
                list.Add(v);
            }
        }
        return list;
    }

    public (int, int) FindCell(Monkey monkey)
    {
        if (!gridDataSet.XYBoundary.WithinBounds(monkey.Tree.X, monkey.Tree.Y))
        {
            throw new ArgumentException();

        }

        int i = (int)((monkey.Tree.X - gridDataSet.XYBoundary.Xmin) / gridDataSet.Delta);
        int j = (int)((monkey.Tree.Y - gridDataSet.XYBoundary.Ymin) / gridDataSet.Delta);

        if (i == gridDataSet.NX)
        {
            i--;
        }
        if (j == gridDataSet.NY)
        {
            j--;
        }

        return (i, j);
    }

    public void ProcessCell(SortedList<double, List<Tree>> nn, int i, int j, Monkey monkey, int n)
    {
        foreach (Tree tree in gridDataSet.GridData[i][j])
        {
            double dSquare = Math.Pow(tree.X - monkey.Tree.X, 2) + Math.Pow(tree.Y - monkey.Tree.Y, 2);
            if ((nn.Count < n) || (dSquare < nn.Keys[nn.Count - 1]))
            {
                if (nn.ContainsKey(dSquare))
                {
                    nn[dSquare].Add(tree);
                }
                else
                {
                    nn.Add(dSquare, new List<Tree>() { tree });
                }
            }
        }
    }

    public void ProcessRing(int i, int j, int ring, SortedList<double, List<Tree>> nn, Monkey monkey, int n)
    {
        for (int gx = i - ring; gx <= i + ring; gx++)
        {
            int gy = j - ring;
            if (IsValidCell(gx, gy))
            {
                ProcessCell(nn, gx, gy, monkey, n);
            }

            gy = j + ring;
            if (IsValidCell(gx, gy))
            {
                ProcessCell(nn, gx, gy, monkey, n);
            }
        }
        for (int gy = j - ring; gy <= j + ring; gy++)
        {
            int gx = i - ring;
            if (IsValidCell(gx, gy))
            {
                ProcessCell(nn, gx, gy, monkey, n);
            }

            gx = i + ring;
            if (IsValidCell(gx, gy))
            {
                ProcessCell(nn, gx, gy, monkey, n);
            }
        }
    }

    public bool IsValidCell(int i, int j)
    {
        if ((i < 0) || (i >= gridDataSet.NX))
        {
            return false;
        }
        if ((j < 0) || (j >= gridDataSet.NY))
        {
            return false;
        }

        return true;
    }
}