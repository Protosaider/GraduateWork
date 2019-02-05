using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGrid {

    public int width, height, size;

    public MazeCell[,] values;

    public MazeGrid() { }

    public MazeGrid(int columns, int rows)
    {
        width = columns;
        height = rows;
        size = rows * columns;

        //Debug.Log("Width = " + width + "; Height = " + height + "; Size = " + size);

        PrepareGrid();
        ConfigureCells();
    }

    public virtual void PrepareGrid()
    {
        values = new MazeCell[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                values[j, i] = new MazeCell(j, i);
                //Debug.Log("Cell.gridX = " + j + "; Cell.gridY = " + i);
            }
        }
        //Debug.Log("WidthCells = " + values.GetLength(0) + "; HeightCells = " + values.GetLength(1));
    }

    public virtual void ConfigureCells()
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (z - 1 >= 0)
                {
                    values[x, z].South = values[x, z - 1];
                }

                if (z + 1 < height)
                {
                    values[x, z].North = values[x, z + 1];
                }

                if (x - 1 >= 0)
                {
                    values[x, z].West = values[x - 1, z];
                }

                if (x + 1 < width)
                {
                    values[x, z].East = values[x + 1, z];
                }
            }
        }
    }

    //accessor
    public MazeCell GetCell(int x, int z)
    {
        if (IsOutside(x, z))
        {
            Debug.Log("GetCell: Out of bounds.");
        }
        return values[x, z];
    }
    
    public bool IsOutside(int x, int z)
    {
        return (z < 0 || x < 0 || x >= width || z >= height);
    }

    public virtual MazeCell GetRandomCell()
    {
        return GetCell(Random.Range(0, width), Random.Range(0, height));
    }

    public IEnumerable<MazeCell[]> EachRow()
    {
        for (int z = height - 1; z >= 0; z--)
        {
            MazeCell[] cellRow = new MazeCell[width];
            for (int x = 0; x < width; x++)
            {
                cellRow[x] = values[x, z];
            }
            yield return cellRow;
        }
    }

    public IEnumerable<MazeCell[]> EachRow(Direction vertical, Direction horizontal)
    {
        if (vertical == Direction.North)
        {
            if (horizontal == Direction.East)
            {
                for (int z = 0; z < height; z++)
                {
                    MazeCell[] cellRow = new MazeCell[width];
                    for (int x = 0; x < width; x++)
                    {
                        cellRow[x] = values[x, z];
                    }
                    yield return cellRow;
                }
            }
            else
            {
                for (int z = 0; z < height; z++)
                {
                    MazeCell[] cellRow = new MazeCell[width];
                    for (int x = width - 1; x >= 0; x--)
                    {
                        cellRow[width - 1 - x] = values[x, z];
                    }
                    yield return cellRow;
                }
            }
        }
        else
        {
            if (horizontal == Direction.East)
            {
                for (int z = height - 1; z >= 0; z--)
                {
                    MazeCell[] cellRow = new MazeCell[width];
                    for (int x = 0; x < width; x++)
                    {
                        cellRow[x] = values[x, z];
                    }
                    yield return cellRow;
                }
            }
            else
            {
                for (int z = height - 1; z >= 0; z--)
                {
                    MazeCell[] cellRow = new MazeCell[width];
                    for (int x = width - 1; x >= 0; x--)
                    {
                        cellRow[width - 1 - x] = values[x, z];
                    }
                    yield return cellRow;
                }
            }
        }
    }

    public IEnumerable<MazeCell> EachCell()
    {
        foreach (var cell in values)
        {
            yield return cell;
        }
    }

    // содержимое клетки
    public virtual string ContentsOfCell(MazeCell cell)
    {
        return "   ";
    }

    public override string ToString()
    {
        string Output = "+";
        for (int i = 0; i < this.width; i++)
        {
            Output += "---+";
        }
        Output += "\n";

        foreach (var cellRow in EachRow())
        {
            string top = "|";
            string bottom = "+";

            foreach (var cell in cellRow)
            {
                string body = ContentsOfCell(cell);
                string eastBoundary = (cell.IsLinked(cell.East) ? " " : "|");

                top += body + eastBoundary;

                string southBoundary = (cell.IsLinked(cell.South) ? "   " : "---");
                string corner = "+";

                bottom += southBoundary + corner;
            }

            Output += top + "\n";
            Output += bottom + "\n";
        }

        return Output;
    }
    public List<MazeCell> GetDeadends()
    {
        List<MazeCell> list = new List<MazeCell>();
        foreach (MazeCell cell in this.EachCell())
        {
            if (cell.Links().Count == 1)
            {
                list.Add(cell);
            }
        }
        return list;
    }

    public Cell[,] ConvertToCellMap(CellType mazeFloor, CellType mazeWall)
    {
        int mapWidth = this.width * 2 - 1;
        int mapHeight = this.height * 2 - 1;

        Cell[,] cellMap = new Cell[mapWidth, mapHeight];

        for (int z = 0; z < this.height; z++)
        {
            for (int x = 0; x < this.width; x++)
            {
                cellMap[x * 2, z * 2] = Cell.CreateCell(mazeFloor);
                //cellMap[x * 2, z * 2] = Cell.CreateCell(CellType.OuterWall);
                if (values[x, z] == null)
                {
                    Debug.Log("x " + x + " y " + z + " values[x, z] " + values[x, z]);
                    cellMap[x * 2, z * 2] = Cell.CreateCell(CellType.OuterWall);
                    for (int i = 0; i < 4; i++)
                    {
                        Coordinate coord = Coordinate.GetOffset(i);
                        if (!IsOutside(x + coord.x, z + coord.z))
                        {
                            //cellMap[x * 2 + coord.x, z * 2 + coord.z] = values[x, z].GetNeighbourAsCell((Direction)i, ref mazeFloor, ref mazeWall);
                            if (cellMap[x * 2 + coord.x, z * 2 + coord.z] == null)
                            {
                                cellMap[x * 2 + coord.x, z * 2 + coord.z] = Cell.CreateCell(CellType.Door);
                            }
                        }
                    }
                    continue;
                }
                for (int i = 0; i < 4; i++)
                {
                    Coordinate coord = Coordinate.GetOffset(i);
                    if (!IsOutside(x + coord.x, z + coord.z))
                    {
                        //Debug.Log("x " + x + " y " + z + " values[x, z] " + values[x, z]);
                        if (values[x + coord.x, z + coord.z] == null)
                        {
                            cellMap[x * 2 + coord.x, z * 2 + coord.z] = Cell.CreateCell(mazeWall);
                            continue;
                        }
                        cellMap[x * 2 + coord.x, z * 2 + coord.z] = values[x, z].GetNeighbourAsCell((Direction)i, ref mazeFloor, ref mazeWall);
                    }                 
                }
            }
        }
        for (int z = 0; z < this.height - 1; z++)
        {
            for (int x = 0; x < this.width - 1; x++)
            {
                cellMap[1 + x * 2, 1 + z * 2] = Cell.CreateCell(mazeWall);
                //cellMap[1 + x * 2, 1 + z * 2] = Cell.CreateCell(CellType.OuterWall);
            }
        }
        return cellMap;
    }

    public EmptyGrid ConvertToEmptyGrid(CellType mazeFloor, CellType mazeWall)
    {
        int mapWidth = this.width * 2 - 1;
        int mapHeight = this.height * 2 - 1;

        Cell[,] cellMap = this.ConvertToCellMap(mazeFloor, mazeWall);

        return new EmptyGrid(cellMap, mapWidth, mapHeight);
    }

    public void BraidMaze(float chanceToCullDeadend)
    {
        List<MazeCell> deadends = GetDeadends();
        deadends = Shuffle(deadends, (int)(Random.value * int.MaxValue));
        foreach (MazeCell deadendCell in deadends)
        {
            if (deadendCell.Links().Count == 1 || Random.value < chanceToCullDeadend)
            {
                List<MazeCell> neighbours = deadendCell.Neighbours.FindAll(c => !deadendCell.IsLinked(c));
                List<MazeCell> neighboursThatAreDeadends = neighbours.FindAll(c => c.Links().Count == 1);

                MazeCell chosenNeighbour = neighboursThatAreDeadends.Count != 0 ? neighboursThatAreDeadends[Random.Range(0, neighboursThatAreDeadends.Count)] : neighbours[Random.Range(0, neighbours.Count)];
                deadendCell.Link(chosenNeighbour);
            }
        }
    }

    private static List<T> Shuffle<T>(List<T> list, int seed)
    {
        Random.State initialState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T exchange = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = exchange;
        }

        Random.state = initialState;
        return list;
    }
}
