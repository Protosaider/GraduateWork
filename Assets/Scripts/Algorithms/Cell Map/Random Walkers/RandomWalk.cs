using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomWalk  {

    private static bool[,] map;

    private static int width, height;

    private static int countToChangePath;
    private static Direction currentDirection;

    private static RandomWalkSettings settings;

    /// TODO: Add null ref checking
    /// 
	public static bool[,] GetCarvedMap(int mapWidth, int mapHeight, RandomWalkSettings walkSettings)
    {
        width = mapWidth;
        height = mapHeight;
        // Create boolean Map
        map = new bool[width, height];
        settings = walkSettings;
        //Initial filling
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, z] = false;
            }
		}
        CarveMap();
        return map;
    }

    private static bool IsOutsideMap(int x, int y)
    {
        return (x < 0 || x >= width || y < 0 || y >= height);
    }

    private static void CarveMap()
    {
        countToChangePath = 0;
        currentDirection = GetDirection();
        Coordinate walker = new Coordinate(Random.Range(0, width), Random.Range(0, height));
        map[walker.x, walker.z] = true;
        int stepsCount = 1;
        int stuckCount = 0;

        //Debugging
        //string filepath = WriteToFileDebugger.GetFilePath("Debug", "RandomWalkDebugMakeStep", ".txt");
        //WriteToFileDebugger.filepath = filepath;
        //WriteToFileDebugger.WriteStringToFile(WriteToFileDebugger.filepath, "", false);

        //filepath = WriteToFileDebugger.GetFilePath("Debug", "RandomWalkDebug", ".txt");
        //WriteToFileDebugger.WriteStringToFile(filepath, "", false);

        if (!(settings.allowMovementDiagonal || settings.allowMovementNSWE))
        {
            settings.allowMovementNSWE = true;
        }

        while (stepsCount < settings.stepsAmount)
        {
            //WriteToFileDebugger.WriteStringToFile(filepath, "stepsCount = " + stepsCount + " stuckCount = " + stuckCount + " walker = " + walker, true);
            MakeStep(ref walker);
            stepsCount++;

            if (map[walker.x, walker.z] == false)
            {
                map[walker.x, walker.z] = true;
            }
            else
            {
                stuckCount += 1;
            }

            if (settings.canLeviFlight && stuckCount > settings.stuckLimit)
            {
                if (settings.connectJumpPointsInLeviFlight)
                {
                    Coordinate prevPoint = walker;
                    walker = new Coordinate(Random.Range(0, width), Random.Range(0, height));
                    BresenhamLine(ref map, prevPoint, walker);
                }
                else
                {
                    walker = new Coordinate(Random.Range(0, width), Random.Range(0, height));
                }
                stuckCount = 0;
            }
        }
    }

    private static void MakeStep(ref Coordinate walker)
    {
        Coordinate startPosition;
        bool isOutside = false;
        //int i = 0;
        do
        {
            //WriteToFileDebugger.WriteStringToFile(WriteToFileDebugger.filepath, "Inside MakeStep countToChangePath = " + countToChangePath + " cycles count = " + i + " dir = " + currentDirection + " is outside" + isOutside, true);

            if (countToChangePath > settings.pathLength || isOutside)
            {
                currentDirection = GetDirection();
                countToChangePath = 0;
            }

            startPosition = new Coordinate(walker.x, walker.z);  //!!! new not necessary here. In C# structs always makes copy when are assigned to another variable.
            startPosition += Coordinate.GetOffset(currentDirection);

            if (IsOutsideMap(startPosition.x, startPosition.z))
            {
                isOutside = true;
                currentDirection = Direction.Nowhere;
            }
            //i++;
        } while (currentDirection == Direction.Nowhere);

        countToChangePath++;
        walker = startPosition;
        //WriteToFileDebugger.WriteStringToFile(WriteToFileDebugger.filepath, "Inside MakeStep Walker " + walker, true);
    }

    private static Direction GetDirection()
    {
        int direction = -1;
        if (settings.allowMovementNSWE && settings.allowMovementDiagonal)
        {
            direction = (Random.value < 0.5) ? Random.Range(0, 4) : Random.Range(6, 10);
        }
        else if (settings.allowMovementNSWE)
        {
            direction = Random.Range(0, 4);
        }
        else
        {
            direction = Random.Range(6, 10);
        }
        return (Direction)direction;
    }

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

    private static void BresenhamLine(ref bool[,] map, Coordinate prevPoint, Coordinate nextPoint)
    {
        bool steep = false;
        if (System.Math.Abs(prevPoint.x - nextPoint.x) < System.Math.Abs(prevPoint.z - nextPoint.z))
        {
            Swap(ref prevPoint.x, ref prevPoint.z);
            Swap(ref nextPoint.x, ref nextPoint.z);
            steep = true;
        }
        if (prevPoint.x > nextPoint.x)
        {
            Swap(ref prevPoint.x, ref nextPoint.x);
            Swap(ref prevPoint.z, ref nextPoint.z);
        }

        int deltaX = nextPoint.x - prevPoint.x;
        int deltaZ = nextPoint.z - prevPoint.z;

        int deltaError = System.Math.Abs(deltaZ) * 2;
        int err = 0;

        int z = prevPoint.z;

        for (int x = prevPoint.x; x <= nextPoint.x; x++)
        {
            if (steep)
            {
                map[z, x] = true;
            }
            else
            {
                map[x, z] = true;
            }

            err += deltaError;
            if (err > deltaX)
            {
                z += System.Math.Sign(deltaZ);
                err -= deltaX * 2;
            }
        }
    }

    private static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
}
