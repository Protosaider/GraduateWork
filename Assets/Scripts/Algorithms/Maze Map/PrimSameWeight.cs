using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrimSameWeight {

    public static T CreateMaze<T>(T grid, MazeCell startingCell = null) where T : MazeGrid
    {
        if (startingCell == null)
        {
            startingCell = grid.GetRandomCell();
        }
        List<MazeCell> active = new List<MazeCell>() { startingCell };

        while (active.Count > 0)
        {
            var currentCell = active[Random.Range(0, active.Count)];
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
