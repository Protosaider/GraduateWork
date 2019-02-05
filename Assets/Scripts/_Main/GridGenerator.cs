using UnityEngine;

public static class GridGenerator
{
    public static EmptyGrid GenerateEmptyGrid(TileMapSettings settings)
    {
        int width, height;
        if (settings.isSquareShape)
        {
            width = height = settings.mapWidth;
        }
        else
        {
            width = settings.mapWidth;
            height = settings.mapHeight;
        }

        Cell[,] cellMap = new Cell[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                cellMap[x, y] = new Cell(true, CellType.Empty, Color.clear, ' ');
            }
        }

        return new EmptyGrid(cellMap, width, height);
    }
}

public enum GridType
{
    Square,
    PointyHex,
}

[System.Serializable]
public class EmptyGrid
{
    [SerializeField]
    public int width, height;
    [SerializeField]
    public Cell[,] values;
    [SerializeField]
    public GridType type;

    public EmptyGrid(Cell[,] cellMap, int width, int height, GridType type = GridType.Square, bool fillWholeMapWithEmptyCells = false)
    {      
        this.width = width;
        this.height = height;
        this.type = type;
        this.values = cellMap;

        if (fillWholeMapWithEmptyCells)
        {
            FillWholeGrid(CellType.Empty);
        }
    }


    public EmptyGrid FillWholeGrid(CellType fillingCellType)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                values[x, y] = Cell.CreateCell(fillingCellType);
            }
        }

        return this;
    }

    public Cell GetNeighbour(int x, int z, Direction direction, GridType mode = GridType.Square)
    {
        int y = 0;
        Coordinate offsetToCheck;

        if (mode == GridType.Square)
        {
            offsetToCheck = Coordinate.GetOffset(direction);
        }
        else
        {
            offsetToCheck = Coordinate.GetOffset(direction) + Coordinate.GetPointyHexOffset(direction, z % 2 == 0);
        }

        Coordinate neighbourCoord = new Coordinate(x + offsetToCheck.x, y + offsetToCheck.y, z + offsetToCheck.z);

        if ((neighbourCoord.x < 0 || neighbourCoord.x >= width) || (neighbourCoord.y != 0) || (neighbourCoord.z < 0 || neighbourCoord.z >= height))
        {
            return new Cell(false, CellType.Empty, Color.clear, ' ');
        }

        return values[neighbourCoord.x, neighbourCoord.z];
    }

    public override string ToString()
    {
        string toString = "";
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                toString += values[x, z].cellMapChar;
            }
            toString += "\n";
        }
        return toString;
    }
}

[System.Serializable]
public class EmptyGridWrapJSON
{
    public int width, height;
    public Cell[] values;
    public GridType type;

    public EmptyGridWrapJSON(EmptyGrid grid)
    {
        this.width = grid.width;
        this.height = grid.height;

        // Convert to flat array
        this.values = new Cell[grid.width * grid.height];
        int i = 0;
        for (int z = 0; z < grid.height; z++)
        {
            for (int x = 0; x < grid.width; x++)
            {
                this.values[i] = grid.values[x, z];
                i++;
            }
        }

        this.type = grid.type;
    }

    public void Wrap(EmptyGrid grid)
    {
        this.width = grid.width;
        this.height = grid.height;

        this.values = new Cell[grid.width * grid.height];
        int i = 0;
        for (int z = 0; z < grid.height; z++)
        {
            for (int x = 0; x < grid.width; x++)
            {
                this.values[i] = grid.values[x, z];
                i++;
            }
        }
    }

    public EmptyGrid Unwrap()
    {
        Cell[,] cellMap = new Cell[width, height];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // (x + z * width) => it works because both (x and z) start iterating from 0
                cellMap[x, z] = values[x + z * width];
            }
        }
        return new EmptyGrid(cellMap, width, height);
    }
}


//if (mode == GetNeighbourMode.XZ)
//{
//    //get XZ
//}
//else if (mode == GetNeighbourMode.XZD)
//{
//    // get XZ and diagonal
//}
//else if (mode == GetNeighbourMode.XZY)
//{
//    // get XYZ
//}
//else if (mode == GetNeighbourMode.XZYD)
//{
//    // get XYZ and diagonal
//}
//return 0;
//public enum GetNeighbourMode
//{
//    XZ = 0,
//    XZD,
//    XZY,
//    XZYD,
//}

