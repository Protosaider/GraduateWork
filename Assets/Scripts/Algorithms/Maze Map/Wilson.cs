using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wilson {

    public static T CreateMaze<T> (T grid) where T : MazeGrid
    {
        List<MazeCell> unvisitedCells = new List<MazeCell>(grid.EachCell());
        unvisitedCells.Remove(grid.GetRandomCell()); //one is visited

        while (unvisitedCells.Count > 0)
        {
            MazeCell currentCell = unvisitedCells[Random.Range(0, unvisitedCells.Count)];
            List<MazeCell> pathToVisitedCell = new List<MazeCell>();
            pathToVisitedCell.Add(currentCell);

            while (unvisitedCells.Contains(currentCell)) // if current cell are Visited
            {
                currentCell = currentCell.Neighbours[Random.Range(0, currentCell.Neighbours.Count)];

                if (pathToVisitedCell.Contains(currentCell)) //if creates loop - erase
                {
                    int index = pathToVisitedCell.IndexOf(currentCell) + 1;
                    pathToVisitedCell.RemoveRange(index, pathToVisitedCell.Count - index);
                }
                else
                {
                    pathToVisitedCell.Add(currentCell);
                }
            }
            // current cell are visited => mark all cells as visited
            for (int i = 0; i < pathToVisitedCell.Count - 1; i++)
            {
                pathToVisitedCell[i].Link(pathToVisitedCell[i + 1], true);
                unvisitedCells.Remove(pathToVisitedCell[i]);
            }
        }

        return grid;
    }

}
