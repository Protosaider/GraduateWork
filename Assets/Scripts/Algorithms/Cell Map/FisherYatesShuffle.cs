using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FisherYatesShuffle
{

    //    -- To shuffle an array a of n elements(indices 0..n-1):
    //for i from 0 to n−2 do
    //     j ← random integer such that i ≤ j < n
    //     exchange a[i] and a[j]

    //To initialize an array a of n elements to a randomly shuffled copy of source, both 0 - based:
    //for i from 0 to n − 1 do
    //    j ← random integer such that 0 ≤ j ≤ i
    //    if j ≠ i
    //        a[i] ← a[j]
    //    a[j] ← source[i]

    ///"inside-out" algorithm realisation
    ///

    public static EmptyGrid ProcessMap(EmptyGrid map, FisherYatesShuffleSettings settings)
    {
        List<Coordinate> coords = new List<Coordinate>(map.height * map.width);
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                coords.Add(new Coordinate(x, y));
            }
        }
        Queue<Coordinate> shuffledCoords = new Queue<Coordinate>(Shuffle<Coordinate>(coords.ToArray(), settings.seed));

        int totalObstaclesCount = 0;
        int maxObstaclesCount = (int)(map.width * map.height * settings.obstaclesFillingPercentage);

        //for (int i = 0; i < maxObstaclesCount; i++)
        while (totalObstaclesCount < maxObstaclesCount)
        {
            Coordinate coord = shuffledCoords.Dequeue();
            shuffledCoords.Enqueue(coord);

            if (coord != settings.spawnPoint)
            {
                map.values[coord.x, coord.z].cellType = CellType.Wall;
                map.values[coord.x, coord.z].cellMapColor = Color.red;
                map.values[coord.x, coord.z].isWalkable = false;
                map.values[coord.x, coord.z].cellMapChar = '#';
                totalObstaclesCount++;
            }
            else
            {
                continue;
            }

        }

        return map;
    }

    private static T[] Shuffle<T>(T[] array, int seed)
    {
        Random.State initialState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            T exchange = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = exchange;
        }

        Random.state = initialState;
        return array;
    }
    
}