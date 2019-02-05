using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomWalkers  {

	public static EmptyGrid ProcessMap(EmptyGrid map, RandomWalkSettings settings)
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
        map.values = RandomWalk.TransformBoolToCell(RandomWalk.GetCarvedMap(map.width, map.height, settings), CellType.Floor, CellType.Wall);
        Random.state = initialState;
        return map;
    }
}
