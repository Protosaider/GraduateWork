using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifLimAgrWalker  {

    //private static bool allowMovementDiagonal;
    //private static bool allowMovementNSWE;

    //private static bool allowStickDiagonal;
    //private static bool allowStickNSWE;
    //private static float chanceToStick;

    //private static int maxStepsAmount;
    //private static int maxStuckCount;

    private static DiffusionLimitedAggregationSettings settings;

    private static int overallStuckCount;
    private static int cellsCreated;
    private static bool[,] map = null;
    private static int width, height;

    private static bool isInitialized = false;
    private static bool hasSetMap = false;

    private Coordinate currentCoord;
    private Direction currentDirection;
    private int stepsCount;
    private int stuckCount;

    private DifLimAgrWalker(Coordinate spawnPoint)
    {
        stepsCount = 0;
        stuckCount = 0;
        currentCoord = spawnPoint;
    }

    public static DifLimAgrWalker SpawnWalker(Coordinate spawnPoint)
    {
        if (isInitialized)
        {
            return new DifLimAgrWalker(spawnPoint);
        }
        else
        {
            Debug.LogError("Variables are not initialized!");
            return null;
        }
    }

    public static void Initialize(DiffusionLimitedAggregationSettings settings, bool[,] map)
    //public static void Initialize(int maxStepsAmount, int maxStuckCount, float chanceToStick, bool allowDiagonal, bool stickDiagonal, bool allowNSWE, bool stickNSWE, bool[,] map)
    {
        if (map == null && !hasSetMap)
        {
            Debug.LogError("Map is set to null.");
        }

        if (!isInitialized)
        {
            if (!hasSetMap)
            {
                DifLimAgrWalker.width = map.GetLength(0);
                DifLimAgrWalker.height = map.GetLength(1);
                System.Buffer.BlockCopy(map, 0, DifLimAgrWalker.map, 0, sizeof(bool) * DifLimAgrWalker.width * DifLimAgrWalker.height);
            }

            //DifLimAgrWalker.allowMovementDiagonal = allowDiagonal;
            //DifLimAgrWalker.allowStickDiagonal = stickDiagonal;
            //DifLimAgrWalker.allowMovementNSWE = allowNSWE;
            //DifLimAgrWalker.allowStickNSWE = stickNSWE;

            //DifLimAgrWalker.chanceToStick = chanceToStick;

            //DifLimAgrWalker.maxStepsAmount = maxStepsAmount;
            //DifLimAgrWalker.maxStuckCount = maxStuckCount;
            DifLimAgrWalker.settings = settings;

            DifLimAgrWalker.cellsCreated = 0;
            DifLimAgrWalker.overallStuckCount = 0;

            DifLimAgrWalker.isInitialized = true;
        }
        else
        {
            Debug.LogError("Static variables are already initialized.");
        }

        //string debug = "";

        //for (int z = 0; z < height; z++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        if (DifLimAgrWalker.map[x, z])
        //        {
        //            debug += "T";
        //        }
        //        else
        //        {
        //            debug += "F";
        //        }
        //    }
        //    debug += "\n";
        //}

        //Debug.Log(debug);
        //Debug.Log("width " + width);
        //Debug.Log("height " + height);
    }

    public static void Clear()
    {
        isInitialized = hasSetMap = false;
    }

    public void RespawnWalker(Coordinate spawnPoint)
    {
        stepsCount = 0;
        stuckCount = 0;
        currentCoord = spawnPoint;

        //Debug.Log("Respawned. cellsCreated " + cellsCreated + " overallStuckCount " + overallStuckCount);
    }

    public bool Walk()
    {
        if (stepsCount >= settings.maxStepsAmount)
        //if (stepsCount >= DifLimAgrWalker.maxStepsAmount)
        {
            return true;
        }

        bool isStuck = MakeStep();
        if (isStuck)
        {
            overallStuckCount++;
            return true;
        }
        stepsCount++;

        if (CanStick())
        {
            //Debug.Log("It's stick");
            //if (Random.value < chanceToStick)
            if (Random.value < settings.chanceToStick)
            {
                //Debug.Log("Walker has been sticked.");
                cellsCreated++;
                map[currentCoord.x, currentCoord.z] = true;
                return true;
            }
        }

        return false;
    }

    public static bool IsEnoughCellsCreated()
    {
        return cellsCreated >= settings.cellsToCreateCount;
    }

    public static bool IsOvergoStuckLimit()
    {
        return overallStuckCount >= settings.overallStuckLimit;
    }

    public bool IsStuck()
    {
        //return stuckCount >= DifLimAgrWalker.maxStuckCount;
        return stuckCount >= settings.maxStuckCount;
    }

    private bool MakeStep()
    {
        Coordinate startPosition;
        do
        {
            currentDirection = GetDirection();
            //startPosition = new Coordinate(currentCoord.x, currentCoord.z);
            startPosition = currentCoord;
            startPosition += Coordinate.GetOffset(currentDirection);
            //Debug.Log("Stuck = " + stuckCount + "startPos" + startPosition);
            if (IsOutsideMap(startPosition))
            {
                currentDirection = Direction.Nowhere;
                stuckCount += 1;
            }
            else
            {
                if (map[startPosition.x, startPosition.z] == true)
                {
                    //Debug.Log("must never happend, but happend here" + startPosition + " " + currentCoord);
                    //Debug.Log("curPos" + currentCoord + " startPos" + startPosition);
                    stuckCount += 1;
                    currentDirection = Direction.Nowhere;
                }
            }

            if (IsStuck())
            {
                return true;
            }
        } while (currentDirection == Direction.Nowhere);
        stuckCount = 0;
        //Debug.Log("curPos" + currentCoord + " startPos" + startPosition);
        currentCoord = startPosition;
        return false;
    }

    private bool IsOutsideMap(int x, int y)
    {
        return (x < 0 || x >= DifLimAgrWalker.width || y < 0 || y >= DifLimAgrWalker.height);
    }

    private bool IsOutsideMap(Coordinate coordinate)
    {
        return (coordinate.x < 0 || coordinate.x >= DifLimAgrWalker.width || coordinate.z < 0 || coordinate.z >= DifLimAgrWalker.height);
    }

    private bool CanStick()
    {
        Coordinate coord = currentCoord;
        //Coordinate coord = new Coordinate(currentCoord.x, currentCoord.z);
        if (settings.allowStickNSWE)
        //if (allowStickNSWE)
        {
            for (int i = 0; i < 4; i++)
            {
                coord += Coordinate.GetOffset((Direction)i);
                //Debug.Log("stick coord " + coord);
                if (!IsOutsideMap(coord))
                {
                    if (map[coord.x, coord.z])
                    {
                        return true;
                    }
                }
                coord = currentCoord;
                //coord = new Coordinate(currentCoord.x, currentCoord.z);
            }
        }
        if (settings.allowStickDiagonal)
        //if (allowStickDiagonal)
        {
            for (int i = 6; i < 10; i++)
            {
                coord += Coordinate.GetOffset((Direction)i);
                if (!IsOutsideMap(coord))
                {
                    if (map[coord.x, coord.z])
                    {
                        return true;
                    }
                }
                coord = currentCoord;
                //coord = new Coordinate(currentCoord.x, currentCoord.z);
            }
        }

        return false;
    }

    private Direction GetDirection()
    {
        int direction = -1;
        if (settings.allowMovementNSWE && settings.allowMovementDiagonal)
        //if (DifLimAgrWalker.allowMovementNSWE && DifLimAgrWalker.allowMovementDiagonal)
        {
            direction = (Random.value < 0.5) ? Random.Range(0, 4) : Random.Range(6, 10);
        }
        //else if (DifLimAgrWalker.allowMovementNSWE)
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

    public static bool[,] TransformCellToBool(ref Cell[,] map, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
    {
        if (trueCell == CellType.Empty && falseCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return null;
        }
        bool[,] boolMap = new bool[map.GetLength(0), map.GetLength(1)];
        for (int z = 0; z < map.GetLength(1); z++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, z].cellType == trueCell)
                {
                    boolMap[x, z] = true;
                }

                if (map[x, z].cellType == falseCell)
                {
                    boolMap[x, z] = false;
                }
            }
        }
        return boolMap;
    }

    public static bool SetBoolMapFromCellMap(ref Cell[,] cellMap, CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
    {
        if (trueCell == CellType.Empty && falseCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return false;
        }

        DifLimAgrWalker.map = new bool[cellMap.GetLength(0), cellMap.GetLength(1)];

        for (int z = 0; z < cellMap.GetLength(1); z++)
        {
            for (int x = 0; x < cellMap.GetLength(0); x++)
            {
                if (cellMap[x, z].cellType == trueCell)
                {
                    DifLimAgrWalker.map[x, z] = true;
                }

                if (cellMap[x, z].cellType == falseCell)
                {
                    DifLimAgrWalker.map[x, z] = false;
                }
            }
        }

        DifLimAgrWalker.width = cellMap.GetLength(0);
        DifLimAgrWalker.height = cellMap.GetLength(1);
        hasSetMap = true;
        return true;
    }

    public static Cell[,] GetBoolMapAsCellMap(CellType trueCell = CellType.Empty, CellType falseCell = CellType.Empty)
    {
        if (trueCell == CellType.Empty && falseCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return null;
        }
        Cell[,] cellMap = new Cell[width, height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, z])
                {
                    cellMap[x, z] = Cell.CreateCell(trueCell);
                }

                if (!map[x, z])
                {
                    cellMap[x, z] = Cell.CreateCell(falseCell);
                }
            }
        }
        return cellMap;
    }
}
