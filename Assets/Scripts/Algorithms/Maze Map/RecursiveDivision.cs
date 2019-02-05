using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecursiveDivision  {

    public static T CreateMaze<T>(T grid) where T : MazeGrid
    {
        foreach (var item in grid.EachCell())
        {
            foreach (var c in item.Neighbours)
            {
                item.Link(c, false);
            }
        }
        Divide(ref grid, 0, 0, grid.width, grid.height);
        return grid;
    }

    private static void Divide<T>(ref T grid, int x, int z, int width, int height) where T : MazeGrid
    {
        //if (height < 2 || width < 2 || (height < 5 && width < 5 && Random.value < 0.2f))
        if (height < 2 || width < 2)
        {
            return;
        }

        if (height > width)
        {
            DivideHorizontally(ref grid, x, z, width, height);
        }
        else
        {
            DivideVertically(ref grid, x, z, width, height);
        }
    }

    private static void DivideVertically<T>(ref T grid, int x, int z, int width, int height) where T : MazeGrid
    {
        int divideCellIndex = Random.Range(0, width - 1);
        int passageAt = Random.Range(0, height);

        for (int i = 0; i < height; i++)
        {
            if (passageAt != i)
            {
                var cell = grid.GetCell(x + divideCellIndex, z + i);
                cell.Unlink(cell.East);
            }
        }

        Divide(ref grid, x, z, divideCellIndex + 1, height);
        Divide(ref grid, x + divideCellIndex + 1, z, width - divideCellIndex - 1, height);
    }

    private static void DivideHorizontally<T>(ref T grid, int x, int z, int width, int height) where T : MazeGrid
    {
        int divideCellIndex = Random.Range(0, height - 1);
        int passageAt = Random.Range(0, width);

        for (int i = 0; i < width; i++)
        {
            if (passageAt != i)
            {
                var cell = grid.GetCell(x + i, z + divideCellIndex);
                cell.Unlink(cell.North);
            }
        }

        Divide(ref grid, x, z, width, divideCellIndex + 1);
        Divide(ref grid, x, z + divideCellIndex + 1, width, height - divideCellIndex - 1);
    }
}
