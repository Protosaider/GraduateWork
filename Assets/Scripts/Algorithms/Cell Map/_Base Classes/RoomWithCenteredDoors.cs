using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWithCenteredDoors
{
    public RoomType type;

    public bool hasNorthNeighbour, hasEastNeighbour, hasSouthNeighbour, hasWestNeighbour = false;

    public bool checkNorth, checkEast, checkSouth, checkWest = false;

    public Coordinate positionInGrid;

    public RoomWithCenteredDoors()
    {
        this.positionInGrid = new Coordinate(0, 0, 0);
        this.type = RoomType.Empty;
    }

    public RoomWithCenteredDoors(Coordinate positionInGrid, RoomType type)
    {
        this.positionInGrid = positionInGrid;
        this.type = type;
    }

    public void CheckDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                checkNorth = true;
                break;
            case Direction.East:
                checkEast = true;
                break;
            case Direction.South:
                checkSouth = true;
                break;
            case Direction.West:
                checkWest = true;
                break;
        }
    }

    public Direction CheckDoor(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                hasSouthNeighbour = true;
                return Direction.South;
            case Direction.East:
                hasWestNeighbour = true;
                return Direction.West;
            case Direction.South:
                hasNorthNeighbour = true;
                return Direction.North;
            case Direction.West:
                hasEastNeighbour = true;
                return Direction.East;
        }
        return Direction.Nowhere;
    }

    public bool HasDoors()
    {
        return !hasNorthNeighbour || !hasEastNeighbour || !hasSouthNeighbour || !hasWestNeighbour;
    }

    public bool HasNeighbours()
    {
        return !checkNorth || !checkEast || !checkSouth || !checkWest;
    }
}

public enum RoomType
{
    Empty = -1,
    Initial,
    Ordinary,
}
