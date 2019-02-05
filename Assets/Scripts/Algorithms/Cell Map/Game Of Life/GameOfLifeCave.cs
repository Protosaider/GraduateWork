using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOfLifeCave  {

    public static EmptyGrid ProcessMap(EmptyGrid map, GameOfLifeSettings settings)
    {
        // Random Generator
        Random.State initialState = Random.state;
        if (settings.useFixedSeed)
        {
            Random.InitState(settings.seed.GetHashCode());
        }
        else
        {
            Random.InitState(Time.time.ToString().GetHashCode());
        }

        // Get map mask
        //bool[,] mask = GameOfLife.ProcessMap(map.width, map.height, settings);
        // Fill with cells
        //for (int z = 0; z < map.height; z++)
        //{
        //    for (int x = 0; x < map.width; x++)
        //    {
        //        if (mask[x, z])
        //        {
        //            map.values[x, z] = Cell.CreateCell(CellType.Wall);
        //        }
        //        else
        //        {
        //            map.values[x, z] = Cell.CreateCell(CellType.Floor);
        //        }
        //    }
        //}

        map.values = GameOfLife.TransformBoolToCell(GameOfLife.ProcessMap(map.width, map.height, settings), CellType.Wall, CellType.Floor);

        Random.state = initialState;
        return map;
    }
}
