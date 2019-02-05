using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameOfLife  {

    private static bool[,] map;
    private static bool[,] mask;
    private static int width, height;
    /// TODO: Add null ref checking
    /// 
	public static bool[,] ProcessMap(int mapWidth, int mapHeight, GameOfLifeSettings settings)
    {
        width = mapWidth;
        height = mapHeight;
        // Create Mask and Map
        map = new bool[width, height];
        mask = new bool[width, height];

        //Initial filling
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (Random.value < settings.randomFillPercent)
                {
                    map[x, z] = true; //wall
                }
                else
                {
                    map[x, z] = false;
                }

                if (settings.useDoubleLayerGeneration)
                {
                    if (Random.value < settings.randomFillPercent)
                    {
                        mask[x, z] = true; //wall
                    }
                    else
                    {
                        mask[x, z] = false;
                    }
                }
            }
        }

        if (settings.useDoubleLayerGeneration)
        {
            //Smooth map
            for (int i = 0; i < settings.smoothCount; i++)
            {
                SmoothMapDoubleLayer(ref settings);
            }

            //Apply Mask Double Layer
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] && mask[x, y] == true)
                    {
                        map[x, y] = true; //wall
                    }
                    if (map[x, y] && mask[x, y] == false)
                    {
                        map[x, y] = false;
                    }
                }
            }
        }
        else
        {
            //Smooth map
            for (int i = 0; i < settings.smoothCount; i++)
            {
                SmoothMap(ref settings);
            }
        }

        return map;
    }

    private static void SmoothMap(ref GameOfLifeSettings settings)
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                //Count neighbours
                int neighboursCount = GetSurroundingWallsCount(x, z, 1, false);

                if (map[x, z] == true) //if a wall
                {
                    mask[x, z] = SmoothRuleOne(neighboursCount, settings.destroyWallLimit);
                }
                else
                {
                    mask[x, z] = SmoothRuleTwo(neighboursCount, settings.createWallLimit);
                }
            }
        }
        // Copy mask to map
        System.Buffer.BlockCopy(mask, 0, map, 0, sizeof(bool) * width * height);
    }

    private static void SmoothMapDoubleLayer(ref GameOfLifeSettings settings)
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int neighbourWallsCount = GetSurroundingWallsCount(x, z, 1, false);

                if (neighbourWallsCount > 4)
                {
                    map[x, z] = true;
                }

                int neighbourWallsCountMask = GetSurroundingWallsCountMask(x, z, 1, false);

                if (neighbourWallsCountMask > 4)
                {
                    mask[x, z] = true;
                }
            }
        }
    }

    private static bool IsOutsideMap(int x, int y)
    {
        return (x < 0 || x >= width || y < 0 || y >= height);
    }

    private static int GetSurroundingWallsCount(int _x, int _y, int rangeOfNeighbours, bool checkCenter)
    {
        int wallsCount = 0;
        for (int x = _x - rangeOfNeighbours; x <= _x + rangeOfNeighbours; x++)
        {
            for (int y = _y - rangeOfNeighbours; y <= _y + rangeOfNeighbours; y++)
            {
                if (checkCenter && x == _x && y == _y)
                {
                    continue;
                }
                if (!IsOutsideMap(x, y))
                {
                    wallsCount += map[x, y] ? 1 : 0;
                }
                else
                {
                    wallsCount += 1;
                }
            }
        }
        return wallsCount;
    }

    private static int GetSurroundingWallsCountMask(int _x, int _y, int rangeOfNeighbours, bool checkCenter)
    {
        int wallsCount = 0;
        for (int x = _x - rangeOfNeighbours; x <= _x + rangeOfNeighbours; x++)
        {
            for (int y = _y - rangeOfNeighbours; y <= _y + rangeOfNeighbours; y++)
            {
                if (checkCenter && x == _x && y == _y)
                {
                    continue;
                }
                if (!IsOutsideMap(x, y))
                {
                    wallsCount += mask[x, y] ? 1 : 0;
                }
                else
                {
                    wallsCount += 1;
                }
            }
        }
        return wallsCount;
    }

    private static bool SmoothRuleOne(int neighboursCount, int destroyWallLimit)
    {
        if (neighboursCount < destroyWallLimit)
        {
            return false;
        }
        else
        {
            return true; //still a wall, 'cause enough walls around
        }
    }

    private static bool SmoothRuleTwo(int neighboursCount, int createWallLimit)
    {
        if (neighboursCount > createWallLimit) //if an empty
        {
            return true; //can create wall
        }
        else
        {
            return false; //failure
        }
    }

    //private static void SmoothMapClassic()
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            SmoothRuleOneClassic(x, y, GetSurroundingWallsCount(x, y, 1, true), GetSurroundingWallsCount(x, y, 2, true));
    //        }
    //    }
    //    // Copy mask to map
    //    System.Buffer.BlockCopy(mask, 0, map, 0, sizeof(bool) * width * height);
    //}

    //private static bool SmoothRuleOneClassic(int x, int y, int neighboursCount, int neighboursCount2)
    //{
    //    if ((neighboursCount >= 5) || (neighboursCount2 <= 2))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    public static Cell[,] TransformBoolToCell(ref bool[,] map, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
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

    public static Cell[,] TransformBoolToCell(bool[,] map, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
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
}
