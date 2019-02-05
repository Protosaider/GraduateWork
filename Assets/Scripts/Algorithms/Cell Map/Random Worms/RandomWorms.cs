using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomWorms {

    private static int width, height;

    private static List<Coordinate> cellsCoordinates;
    private static List<RandomWorm> worms;
    private static int maxZ, minZ, minX, maxX;

    private static RandomWormsSettings settings;

    private static bool[,] map;

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, RandomWormsSettings settings)
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

        RandomWorms.settings = settings;
        RandomWorms.worms = new List<RandomWorm>();
        RandomWorms.cellsCoordinates = new List<Coordinate>(settings.cellsToCreateCount)
        {
            new Coordinate(0, 0) // Initial spawn cell
        };

        RandomWorms.maxX = RandomWorms.maxZ = RandomWorms.minX = RandomWorms.minZ = 0;

        RandomWorms.worms.Add(new RandomWorm(settings.chanceToTurnAround, settings.chanceToTurn, settings.allowTurnAround));

        for (int i = 1; i < settings.wormsCount; i++)
        {
            RandomWorms.worms.Add(new RandomWorm(RandomWorms.worms[0].GetCurrentCoord()));
        }
        RandomWorm.SetChanceToDie(settings.initialChanceToDie, settings.increaseDieChanceBy);

        RandomWorm.createRoomEvent += CreateRectangularRoom;

        MoveWorms();

        RandomWorm.createRoomEvent -= CreateRectangularRoom;

        RandomWorm.GetMaxCoord(ref RandomWorms.minX, ref RandomWorms.maxX, ref RandomWorms.minZ, ref RandomWorms.maxZ);
        mapSettings.mapWidth = RandomWorms.width = maxX - minX + 1; //+1 for zero tile
        mapSettings.mapHeight = RandomWorms.height = maxZ - minZ + 1; //+1 for zero tile

        Random.state = initialState;
        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, RandomWormsSettings settings)
    {
        // Process Map
        //RandomWorms.map = new bool[width, height];
        //InitialFillMap();
        //FillMapWithFloor();

        FillMap(ref map.values, CellType.Floor, CellType.Wall);

        return map;
    }

    private static Cell[,] FillMap(ref Cell[,] cellMap, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
    {
        if (trueCell == CellType.Empty && falseCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return null;
        }

        int xOffset = minX; // from -inf to 0
        int zOffset = minZ; // same

        if (trueCell != CellType.Empty)
        {
            foreach (Coordinate coord in cellsCoordinates)
            {
                cellMap[coord.x - xOffset, coord.z - zOffset] = Cell.CreateCell(trueCell);
            }
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (falseCell != CellType.Empty && cellMap[x, z].cellType == CellType.Empty)
                {
                    cellMap[x, z] = Cell.CreateCell(falseCell);
                }
            }
        }
        return cellMap;
    }

    private static void InitialFillMap()
    {
        for (int z = 0; z < width; z++)
        {
            for (int x = 0; x < height; x++)
            {
                RandomWorms.map[x, z] = false;
            }
        }
    }

    //private static void FillMapWithFloor()
    //{
    //    //int xOffset = -minX;
    //    //int zOffset = -minZ;
    //    int xOffset = minX; // from -inf to 0
    //    int zOffset = minZ; // same

    //    foreach (Coordinate coord in cellsCoordinates)
    //    {
    //        //RandomWorms.map[coord.x + xOffset, coord.z + zOffset] = true;
    //        RandomWorms.map[coord.x - xOffset, coord.z - zOffset] = true;  /// example: coord.x = -3;  xOffset = -5;  => x = -3 - (-5)
    //    }
    //}

    private static Cell[,] TransformBoolToCell(ref bool[,] map, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
    {
        if (trueCell == CellType.Empty && falseCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return null;
        }
        Cell[,] cellMap = new Cell[map.GetLength(0), map.GetLength(1)];
        for (int z = 0; z < map.GetLength(1); z++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (trueCell != CellType.Empty && map[x, z])
                {
                    cellMap[x, z] = Cell.CreateCell(trueCell);
                }

                if (falseCell != CellType.Empty && !map[x, z])
                {
                    cellMap[x, z] = Cell.CreateCell(falseCell);
                }
            }
        }
        return cellMap;
    }

    private static Cell[,] TransformBoolToCell(bool[,] map, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
    {
        if (trueCell == CellType.Empty && falseCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return null;
        }
        Cell[,] cellMap = new Cell[map.GetLength(0), map.GetLength(1)];
        for (int z = 0; z < map.GetLength(1); z++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (trueCell != CellType.Empty && map[x, z])
                {
                    cellMap[x, z] = Cell.CreateCell(trueCell);
                }

                if (falseCell != CellType.Empty && !map[x, z])
                {
                    cellMap[x, z] = Cell.CreateCell(falseCell);
                }
            }
        }
        return cellMap;
    }

    private static void MoveWorms()
    {
        while (!RandomWorm.IsEnoughCellsCreated(settings.cellsToCreateCount))
        {
            if (worms.Count == 0)
            {
                worms.Add(new RandomWorm(cellsCoordinates[Random.Range(0, cellsCoordinates.Count)]));
            }

            for (int i = worms.Count - 1; i >= 0; i--)
            {
                //if work is done
                if (RandomWorm.IsEnoughCellsCreated(settings.cellsToCreateCount))
                {
                    break;
                }

                RandomWorm worm = worms[i];
                bool isAlive = worm.Move(); //move

                Coordinate currentCoord = worm.GetCurrentCoord(); //create tile
                if (!cellsCoordinates.Contains(currentCoord))
                {
                    cellsCoordinates.Add(currentCoord);
                    RandomWorm.IncreaseCreatedCellsCount();
                }

                if (!isAlive) //if dead
                {
                    worms.Remove(worm);
                    RandomWorm.DecreaseWormsCount();
                    RandomWorm.SetChanceToDie(settings.initialChanceToDie, settings.increaseDieChanceBy);
                    continue;
                }

                if (Random.value < settings.chanceToCreateWorm)
                {
                    worms.Add(new RandomWorm(worm.GetCurrentCoord()));
                    //worms.Add(new RandomWorm(cellsCoordinates[Random.Range(0, cellsCoordinates.Count)]));
                    //settings.wormsCount++;
                    RandomWorm.SetChanceToDie(settings.initialChanceToDie, settings.increaseDieChanceBy);
                }
            }
        }
    }

    private static void CreateRectangularRoom(Coordinate currentCoord, int width, int height) //universal algorithm for rectangular-shaped room
    {
        Coordinate floorTile = currentCoord;
        List<Coordinate> room = new List<Coordinate>(width * height);
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                floorTile.x += x;
                floorTile.z += z;
                room.Add(floorTile);
                floorTile = currentCoord;
            }
        }

        int xOffset = Random.Range(-(width - 1), 0);  // - 1 to not move away from current coordinate
        int zOffset = Random.Range(-(height - 1), 0); // same here

        for (int i = 0; i < room.Count; i++)
        {
            floorTile = room[i];
            floorTile.x += xOffset;
            floorTile.z += zOffset;

            if (!cellsCoordinates.Contains(floorTile))
            {
                cellsCoordinates.Add(floorTile);
                RandomWorm.IncreaseCreatedCellsCount();
                RandomWorm.UpdateMinMaxMapCoordinates(floorTile.x, floorTile.z);
            }
        }
    }
}
