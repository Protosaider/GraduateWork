using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GrowingTree {

    public static T CreateMaze<T>(T grid, MazeCell startingCell = null, System.Func<List<MazeCell>, MazeCell> func = null) where T : MazeGrid
    {
        if (startingCell == null)
        {
            startingCell = grid.GetRandomCell();
        }
        List<MazeCell> active = new List<MazeCell>() { startingCell };

        if (func == null)
        {
            func = x => x[Random.Range(0, x.Count)];
        }

        while (active.Count > 0)
        {
            var currentCell = func(active);

            var unlinkedNeighbours = currentCell.Neighbours.FindAll((c) => c.Links().Count == 0);

            if (unlinkedNeighbours.Count == 0)
            {
                active.Remove(currentCell);
            }
            else
            {
                var neighbour = unlinkedNeighbours[Random.Range(0, unlinkedNeighbours.Count)];
                currentCell.Link(neighbour);
                active.Add(neighbour);
            }
        }

        return grid;
    }
}
