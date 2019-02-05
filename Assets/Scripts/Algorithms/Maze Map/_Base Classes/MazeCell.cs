using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : IHeapItem<MazeCell> {

    public int weight
    {
        get;
        set;
    }

    public int X
    {
        get;
        private set;
    }

    public int Z
    {
        get;
        private set;
    }

    //neighbours
    public MazeCell North
    {
        get;
        set;
    }
    public MazeCell West
    {
        get;
        set;
    }
    public MazeCell South
    {
        get;
        set;
    }
    public MazeCell East
    {
        get;
        set;
    }

    public List<MazeCell> Neighbours
    {
        get
        {
            var list = new List<MazeCell>();
            if (North != null)
            {
                list.Add(North);
            }
            if (West != null)
            {
                list.Add(West);
            }
            if (South != null)
            {
                list.Add(South);
            }
            if (East != null)
            {
                list.Add(East);
            }
            return list;
        }
    }

    private Dictionary<MazeCell, bool> links;

    public MazeCell(int gridX, int gridZ)
    {
        this.X = gridX;
        this.Z = gridZ;
        weight = 1;

        links = new Dictionary<MazeCell, bool>();
    }

    public MazeCell Link(MazeCell cell, bool isBidirectional = true)
    {
        links[cell] = true;
        if (isBidirectional)
        {
            cell.Link(this, false);
        }
        return this;
    }

    public MazeCell Unlink(MazeCell cell, bool isBidirectional = true)
    {
        links.Remove(cell);
        if (isBidirectional)
        {
            cell.Unlink(this, false);
        }
        return this;
    }

    public List<MazeCell> Links()
    {
        return new List<MazeCell>(links.Keys);
    }

    public bool IsLinked(MazeCell cell)
    {
        if (cell == null) //don't forget!
        {
            return false;
        }
        return links.ContainsKey(cell);
    }

    public Cell GetNeighbourAsCell(Direction dir, ref CellType mazeFloor, ref CellType mazeWall)
    {
        switch (dir)
        {
            case Direction.North:
                return IsLinked(North) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            case Direction.East:
                return IsLinked(East) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            case Direction.South:
                return IsLinked(South) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            case Direction.West:
                return IsLinked(West) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            default:
                Debug.LogError("Can't find neighbour of maze cell");
                return Cell.CreateCell(CellType.Empty);
        }
    }

    public MazeCell GetRandomNeighbour()
    {
        return Neighbours[Random.Range(0, Neighbours.Count)];
    }

    // Wave algo - fill all nearby cells with distance from first cell to the last cell on grid
    public virtual Distances FindDistanceForAllReachableLinkedCells()
    {
        Distances distances = new Distances(this);
        List<MazeCell> frontier = new List<MazeCell>() { this };

        // Will expands until last connected cell found
        while (frontier.Count > 0)
        {
            var newFrontier = new List<MazeCell>();

            foreach (var frontierCell in frontier)
            {
                foreach (var linkedCell in frontierCell.Links())
                {
                    if (distances.ContainsKey(linkedCell)) // if cell is visited then skip
                    {
                        continue;
                    }
                    distances.Add(linkedCell, distances[frontierCell] + 1);
                    newFrontier.Add(linkedCell);
                }
            }

            frontier = newFrontier;
        }

        return distances;
    }

    private int heapIndex;

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(MazeCell other)
    {
        int compare = this.weight.CompareTo(other.weight);
        return -compare; //return -1 if other node is lower (this node is higher)
    }
}
