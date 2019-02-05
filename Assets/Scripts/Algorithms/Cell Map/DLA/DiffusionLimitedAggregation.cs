using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffusionLimitedAggregation : MonoBehaviour {

    private static List<Coordinate> northWall;
    private static List<Coordinate> southWall;
    private static List<Coordinate> eastWall;
    private static List<Coordinate> westWall;
    private static Coordinate center;

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, DiffusionLimitedAggregationSettings settings)
    {
        if (mapSettings.mapWidth < 10)
        {
            mapSettings.mapWidth = 10;
        }
        if (mapSettings.mapHeight < 10)
        {
            mapSettings.mapHeight = 10;
        }
        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, DiffusionLimitedAggregationSettings settings)
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

        if (!(settings.allowMovementDiagonal || settings.allowMovementNSWE))
        {
            settings.allowMovementNSWE = true;
        }

        if (!(settings.allowStickDiagonal || settings.allowStickNSWE))
        {
            settings.allowStickNSWE = true;
        }

        if (settings.usePredefinedSpawnPoints)
        {
            if (settings.isAggregatorAtCenter)
            {
                settings.spawnWalkersAlongNorthWall =
                settings.spawnWalkersAlongSouthWall =
                settings.spawnWalkersAlongEastWall =
                settings.spawnWalkersAlongWestWall = 
                settings.spawnAggregatorsAtCenter = true;

                settings.spawnWalkersAtCenter =
                settings.spawnAggregatorsAlongNorthWall =
                settings.spawnAggregatorsAlongSouthWall =
                settings.spawnAggregatorsAlongEastWall =
                settings.spawnAggregatorsAlongWestWall = false;
            }
            else
            {
                settings.spawnWalkersAlongNorthWall =
                settings.spawnWalkersAlongSouthWall =
                settings.spawnWalkersAlongEastWall =
                settings.spawnWalkersAlongWestWall =
                settings.spawnAggregatorsAtCenter = false;

                settings.spawnWalkersAtCenter =
                settings.spawnAggregatorsAlongNorthWall =
                settings.spawnAggregatorsAlongSouthWall =
                settings.spawnAggregatorsAlongEastWall =
                settings.spawnAggregatorsAlongWestWall = true;
            }
        }
        else
        {
            if (!(settings.spawnWalkersAlongNorthWall ||
                  settings.spawnWalkersAlongSouthWall ||
                  settings.spawnWalkersAlongEastWall ||
                  settings.spawnWalkersAlongWestWall ||
                  settings.spawnWalkersAtCenter))
            {
                settings.spawnWalkersAlongNorthWall =
                settings.spawnWalkersAlongSouthWall =
                settings.spawnWalkersAlongEastWall =
                settings.spawnWalkersAlongWestWall = true;
            }

            if (!(settings.spawnAggregatorsAlongNorthWall ||
                  settings.spawnAggregatorsAlongSouthWall ||
                  settings.spawnAggregatorsAlongEastWall ||
                  settings.spawnAggregatorsAlongWestWall ||
                  settings.spawnAggregatorsAtCenter))
            {
                settings.spawnAggregatorsAtCenter = true;
            }

            if (settings.spawnWalkersAlongNorthWall && settings.spawnAggregatorsAlongNorthWall)
            {
                settings.spawnAggregatorsAlongNorthWall = false;
            }
            if (settings.spawnWalkersAlongSouthWall && settings.spawnAggregatorsAlongSouthWall)
            {
                settings.spawnAggregatorsAlongSouthWall = false;
            }
            if (settings.spawnWalkersAlongEastWall && settings.spawnAggregatorsAlongEastWall)
            {
                settings.spawnAggregatorsAlongEastWall = false;
            }
            if (settings.spawnWalkersAlongWestWall && settings.spawnAggregatorsAlongWestWall)
            {
                settings.spawnAggregatorsAlongWestWall = false;
            }
            if (settings.spawnWalkersAtCenter && settings.spawnAggregatorsAtCenter)
            {
                settings.spawnWalkersAtCenter = false;
            }
        }

        if (settings.cellsToCreateCount >= map.width * map.height)
        {
            settings.cellsToCreateCount = Mathf.FloorToInt(map.width * map.height * 0.5f);
        }
        if (settings.aliveWalkersAmount < 1)
        {
            settings.aliveWalkersAmount = 1;
        }
        if (settings.maxStepsAmount < 5)
        {
            settings.maxStepsAmount = 5;
        }
        if (settings.maxStuckCount < 5)
        {
            settings.maxStuckCount = 5;
        }
        if (settings.overallStuckLimit < 10)
        {
            settings.overallStuckLimit = 10;
        }

        // Processing
        center = new Coordinate(Mathf.FloorToInt((map.width - 1) * 0.5f), Mathf.FloorToInt((map.height - 1) * 0.5f));
        northWall = null;
        southWall = null;
        eastWall = null;
        westWall = null;

        if (settings.spawnWalkersAlongNorthWall || settings.spawnAggregatorsAlongNorthWall)
        {
            northWall = new List<Coordinate>(map.width);
            for (int x = 0; x < map.width; x++)
            {
                northWall.Add(new Coordinate(x, map.height - 1));
            }
        }
        if (settings.spawnWalkersAlongSouthWall || settings.spawnAggregatorsAlongSouthWall)
        {
            southWall = new List<Coordinate>(map.width);
            for (int x = 0; x < map.width; x++)
            {
                southWall.Add(new Coordinate(x, 0));
            }
        }
        if (settings.spawnWalkersAlongEastWall || settings.spawnAggregatorsAlongEastWall)
        {
            eastWall = new List<Coordinate>(map.height);
            for (int z = 0; z < map.height; z++)
            {
                eastWall.Add(new Coordinate(map.width - 1, z));
            }
        }
        if (settings.spawnWalkersAlongWestWall || settings.spawnAggregatorsAlongWestWall)
        {
            westWall = new List<Coordinate>(map.height);
            for (int z = 0; z < map.height; z++)
            {
                westWall.Add(new Coordinate(0, z));
            }
        }

        // Fill map
        CellType trueCell = CellType.Wall;
        CellType falseCell = CellType.Empty;

        // карту заполним сразу trueCell-ами
        if (settings.spawnAggregatorsAlongNorthWall)
        {
            for (int i = 0; i < northWall.Count; i++)
            {
                map.values[northWall[i].x, northWall[i].z] = Cell.CreateCell(trueCell);
            }
        }
        if (settings.spawnAggregatorsAlongSouthWall)
        {
            for (int i = 0; i < southWall.Count; i++)
            {
                map.values[southWall[i].x, southWall[i].z] = Cell.CreateCell(trueCell);
            }
        }       
        if (settings.spawnAggregatorsAlongEastWall)
        {
            for (int i = 0; i < eastWall.Count; i++)
            {
                map.values[eastWall[i].x, eastWall[i].z] = Cell.CreateCell(trueCell);
            }
        }
        if (settings.spawnAggregatorsAlongWestWall)
        {
            for (int i = 0; i < westWall.Count; i++)
            {
                map.values[westWall[i].x, westWall[i].z] = Cell.CreateCell(trueCell);
            }
        }                              
        if (settings.spawnAggregatorsAtCenter)
        {
            map.values[center.x, center.z] = Cell.CreateCell(trueCell);
        }

        trueCell = CellType.Wall;
        falseCell = CellType.Empty;

        // fill working map mask
        // from cell to bool
        DifLimAgrWalker.SetBoolMapFromCellMap(ref map.values, trueCell, falseCell);

        //DifLimAgrWalker.Initialize(settings.maxStepsAmount, settings.maxStuckCount, settings.chanceToStick, settings.allowMovementDiagonal, settings.allowStickDiagonal, settings.allowMovementNSWE, settings.allowStickNSWE, null);
        DifLimAgrWalker.Initialize(settings, null);

        List<DifLimAgrWalker> walkers = new List<DifLimAgrWalker>(settings.aliveWalkersAmount);

        int spawnPoints = 0;
        spawnPoints += settings.spawnWalkersAlongNorthWall ? northWall.Count : 0;
        spawnPoints += settings.spawnWalkersAlongSouthWall ? southWall.Count : 0;
        spawnPoints += settings.spawnWalkersAlongEastWall ? eastWall.Count : 0;
        spawnPoints += settings.spawnWalkersAlongWestWall ? westWall.Count : 0;
        spawnPoints += settings.spawnWalkersAtCenter ? 1 : 0;

        float spawnChance = 1.0f / spawnPoints;

        for (int i = 0; i < settings.aliveWalkersAmount; i++)
        {
            walkers.Add(DifLimAgrWalker.SpawnWalker(RandomSpawnPoint(ChooseSpawnPool(ref settings, ref spawnChance))));
        }

        // OverallStuck - критерий останова
        while (!DifLimAgrWalker.IsEnoughCellsCreated() && !DifLimAgrWalker.IsOvergoStuckLimit())
        {
            for (int i = 0; i < settings.aliveWalkersAmount; i++)
            {
                bool isTimeToRespawn = walkers[i].Walk();

                if (isTimeToRespawn)
                {
                    if (walkers[i].IsStuck())
                    {
                        if (DifLimAgrWalker.IsOvergoStuckLimit())
                        {
                            break;
                        }
                    }
                    walkers[i].RespawnWalker(RandomSpawnPoint(ChooseSpawnPool(ref settings, ref spawnChance)));
                }
            }
        }

        DifLimAgrWalker.Clear();

        trueCell = CellType.Wall;
        falseCell = CellType.Floor;

        map.values = DifLimAgrWalker.GetBoolMapAsCellMap(trueCell, falseCell);

        Random.state = initialState;

        return map;
    }

    private static Coordinate RandomSpawnPoint(int spawnPointsPool)
    {
        switch (spawnPointsPool)
        {
            case 0:
                return northWall[Random.Range(0, northWall.Count)];
            case 1:
                return southWall[Random.Range(0, southWall.Count)];
            case 2:
                return eastWall[Random.Range(0, eastWall.Count)];
            case 3:
                return westWall[Random.Range(0, westWall.Count)];
            case 4:
                return center;
            default:
                Debug.LogError("Error in random spawn point choosing. Wrong spawn point pool number.");
                break;
        }
        Debug.LogError("Error in random spawn point choosing.");
        return new Coordinate(0, 0);
    }

    private static int ChooseSpawnPool(ref DiffusionLimitedAggregationSettings settings, ref float spawnChance)
    {
        do
        {
            if (settings.spawnWalkersAlongNorthWall)
            {
                if (Random.value < spawnChance * northWall.Count)
                {
                    return 0;
                }
            }
            if (settings.spawnWalkersAlongSouthWall)
            {
                if (Random.value < spawnChance * southWall.Count)
                {
                    return 1;
                }
            }
            if (settings.spawnWalkersAlongEastWall)
            {
                if (Random.value < spawnChance * eastWall.Count)
                {
                    return 2;
                }
            }
            if (settings.spawnWalkersAlongWestWall)
            {
                if (Random.value < spawnChance * westWall.Count)
                {
                    return 3;
                }
            }
            if (settings.spawnWalkersAtCenter)
            {
                if (Random.value < spawnChance)
                {
                    return 4;
                }
            }
        } while (true);       
    }
}
