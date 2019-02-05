using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AldousBroder {

    // Implements Random Walkers
    public static T CreateMaze<T>(T grid) where T : MazeGrid
    {
        // Get random initial cell
        MazeCell currentCell = grid.GetRandomCell();

        int unvisitedCellsCount = grid.size - 1;

        // Random walks until visit all cells. Takes long time 'cause can walk on visited cells => possibility to hang around
        while (unvisitedCellsCount > 0)
        {
            var neighbourCell = currentCell.GetRandomNeighbour();

            if (neighbourCell.Links().Count == 0) //if current cell hasn't yet been linked to any other cell.
            {
                currentCell.Link(neighbourCell);
                unvisitedCellsCount--;
            }

            currentCell = neighbourCell;
        }

        return grid;
    }
}
