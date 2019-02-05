using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWorm {

    public static event System.Action<Coordinate, int, int> createRoomEvent;

    private static float chanceToTurnAround;
    private static float chanceToTurn;
    private static bool allowTurnAround;

    private static float chanceToCreateRectangularRoom;

    private static float chanceToDie;
    private static int wormsCount = 0;

    private static int maxZ = 0;
    private static int minZ = 0;
    private static int minX = 0;
    private static int maxX = 0;

    private static int cellsCreated = 0;

    private Coordinate currentCoord;
    private Direction moveOrientation;

    /// <summary>
    /// Initial worm spawner. Use for creating first worm and clear up all static variables.
    /// </summary>
    /// <param name="chanceToTurnAround"></param>
    /// <param name="chanceToTurn"></param>
    /// <param name="allowTurnAround"></param>
    public RandomWorm(float chanceToTurnAround, float chanceToTurn, bool allowTurnAround)
    {
        //RandomWorm.wormsCount++;
        RandomWorm.wormsCount = 1;
        RandomWorm.chanceToTurnAround = chanceToTurnAround;
        RandomWorm.chanceToTurn = chanceToTurn;
        RandomWorm.allowTurnAround = allowTurnAround;
        RandomWorm.cellsCreated = 1;
        RandomWorm.maxZ = RandomWorm.minZ = RandomWorm.minX = RandomWorm.maxX = 0;
        currentCoord = new Coordinate(0, 0);
        moveOrientation = Direction.North;
    }
    /// <summary>
    /// Method to create (spawn) a worm. Has random moving direction.
    /// </summary>
    /// <param name="aliveWormCoordinate">Point where worm is spawned</param>

    public RandomWorm(Coordinate aliveWormCoordinate)
    //private RandomWorm(Coordinate aliveWormCoordinate)
    {
        RandomWorm.wormsCount++;
        currentCoord = aliveWormCoordinate;
        moveOrientation = Direction.North;
        while (Random.value < 0.5f)
        {
            Turn();
        }
    }
    //~RandomWorm()
    //{
    //    wormsCount--;
    //}
    //public static RandomWorm SpawnAdditionalWorm(Coordinate aliveWormCoordinate)
    //{
    //    return new RandomWorm(aliveWormCoordinate);
    //}

    public static void DecreaseWormsCount()
    {
        RandomWorm.wormsCount--;
    }

    public static void IncreaseCreatedCellsCount()
    {
        cellsCreated++;
    }

    public Coordinate GetCurrentCoord()
    {
        return currentCoord;
    }

    public static void SetChanceToDie(float initialChance, float increaseDieChance)
    {
        RandomWorm.chanceToDie = initialChance + (RandomWorm.wormsCount - 1) * increaseDieChance;
    }

    public static void GetMaxCoord(ref int minX, ref int maxX, ref int minZ, ref int maxZ)
    {
        minZ = RandomWorm.minZ;
        maxZ = RandomWorm.maxZ;
        minX = RandomWorm.minX;
        maxX = RandomWorm.maxX;
    }

    private bool IsAlive()
    {
        return Random.value < RandomWorm.chanceToDie;
    }

    private void CreateRoom()
    {
        if (createRoomEvent != null)
        {
            createRoomEvent(currentCoord, 3, 3); //3x3 room
        }
    }

    private void Turn()
    {
        switch (moveOrientation)
        {
            case Direction.North:
                moveOrientation = (Random.value < 0.5f) ? Direction.East : Direction.West;
                break;
            case Direction.South:
                moveOrientation = (Random.value < 0.5f) ? Direction.East : Direction.West;
                break;
            case Direction.East:
                moveOrientation = (Random.value < 0.5f) ? Direction.North : Direction.South;
                break;
            case Direction.West:
                moveOrientation = (Random.value < 0.5f) ? Direction.North : Direction.South;
                break;
            default:
                Debug.Log("Error in Turn()");
                break;
        }
    }

    private void TurnAround()
    {
        switch (moveOrientation)
        {
            case Direction.North:
                moveOrientation = Direction.South;
                break;
            case Direction.South:
                moveOrientation = Direction.North;
                break;
            case Direction.East:
                moveOrientation = Direction.West;
                break;
            case Direction.West:
                moveOrientation = Direction.East;
                break;
            default:
                Debug.Log("Error in TurnAround()");
                break;
        }
    }

    public static bool IsEnoughCellsCreated(int cellsToCreateCount)
    {
        return cellsCreated >= cellsToCreateCount;
    }

    public static void UpdateMinMaxMapCoordinates(int x, int z)
    {
        if (x > maxX)
        {
            maxX = x;
        }
        if (x < minX)
        {
            minX = x;
        }
        if (z > maxZ)
        {
            maxZ = z;
        }
        if (z < minZ)
        {
            minZ = z;
        }
    }

    public bool Move()
    {
        currentCoord += Coordinate.GetOffset(moveOrientation);
        UpdateMinMaxMapCoordinates(currentCoord.x, currentCoord.z);

        if (!IsAlive())
        {
            return false;
        }

        if (Random.value < chanceToTurn) 
        {
            Turn();
        }
        else if (allowTurnAround && Random.value < chanceToTurnAround)
        {
            TurnAround();
        }

        if (Random.value < chanceToCreateRectangularRoom)
        {
            CreateRoom();
        }

        return true;
    }
}
