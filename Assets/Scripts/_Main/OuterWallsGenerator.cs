using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OuterWallsGenerator
{

    public static EmptyGrid CreateOuterWalls(EmptyGrid map)
    {
        Cell[,] mapWithWalls = new Cell[map.width + 2, map.height + 2];

        int x = 0, y = 0;

        for (y = 0; y < map.height; y++)
        {
            for (x = 0; x < map.width; x++)
            {
                mapWithWalls[x + 1, y + 1] = map.values[x, y];
            }
        }

        x = 0;
        for (y = 0; y < map.height + 2; y++)
        {
            mapWithWalls[x, y] = new Cell(false, CellType.OuterWall, Color.blue, '@');
            mapWithWalls[map.width + 1, y] = new Cell(false, CellType.OuterWall, Color.blue, '@');
        }

        y = 0;
        for (x = 0; x < map.width + 2; x++)
        {
            mapWithWalls[x, y] = new Cell(false, CellType.OuterWall, Color.blue, '@');
            mapWithWalls[x, map.height + 1] = new Cell(false, CellType.OuterWall, Color.blue, '@');
        }

        map.values = mapWithWalls;
        map.width = map.width + 2;
        map.height = map.height + 2;

        return map;
    }
}
