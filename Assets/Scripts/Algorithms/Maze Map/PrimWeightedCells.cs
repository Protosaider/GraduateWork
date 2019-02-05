using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrimWeightedCells {

    //public static T CreateMaze<T>(T grid, MazeCell startingCell = null) where T : MazeGrid
    //{
    //    if (startingCell == null)
    //    {
    //        startingCell = grid.GetRandomCell();
    //    }

    //    List<MazeCell> active = new List<MazeCell>() { startingCell };

    //    Dictionary<MazeCell, int> costs = new Dictionary<MazeCell, int>();
    //    foreach (var cell in grid.EachCell())
    //    {
    //        costs.Add(cell, Random.Range(0, 100));
    //    }

    //    while (active.Count > 0)
    //    {
    //        active.Sort((x, y) => costs[x].CompareTo(costs[y])); //ascending orders
    //        var currentCell = active[0]; //min value

    //        var unlinkedNeighbours = currentCell.Neighbours.FindAll((c) => c.Links().Count == 0);

    //        if (unlinkedNeighbours.Count == 0)
    //        {
    //            active.Remove(currentCell);
    //        }
    //        else
    //        {
    //            unlinkedNeighbours.Sort((x, y) => costs[x].CompareTo(costs[y]));
    //            var neighbour = unlinkedNeighbours[0]; //min value
    //            currentCell.Link(neighbour);
    //            active.Add(neighbour);
    //        }
    //    }

    //    return grid;
    //}

    public static T CreateMaze<T>(T grid, MazeCell startingCell = null) where T : MazeGrid
    {
        if (startingCell == null)
        {
            startingCell = grid.GetRandomCell();
        }

        foreach (var cell in grid.EachCell())
        {
            cell.weight = Random.Range(0, 100);
        }

        Heap<MazeCell> active = new Heap<MazeCell>(grid.width * grid.height);
        active.Add(startingCell);

        while (active.Count > 0)
        {
            var currentCell = active.Peek(); //min value

            var unlinkedNeighbours = currentCell.Neighbours.FindAll((c) => c.Links().Count == 0);

            if (unlinkedNeighbours.Count == 0)
            {
                active.Remove();
            }
            else
            {
                unlinkedNeighbours.Sort((x, y) => x.weight.CompareTo(y.weight));
                var neighbour = unlinkedNeighbours[0]; //min value
                currentCell.Link(neighbour);
                active.Add(neighbour);
            }
        }

        return grid;
    }
}
