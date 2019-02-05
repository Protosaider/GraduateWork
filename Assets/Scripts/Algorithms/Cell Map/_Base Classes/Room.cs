using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public RoomType type;

    public BoundsInt boundsInt;
    public int SizeX
    {
        get
        {
            return boundsInt.size.x;
        }
        private set { }
    }
    public int SizeZ
    {
        get
        {
            return boundsInt.size.z;
        }
        private set { }
    }
    public int CenterZ
    {
        get
        {
            //return boundsInt.zMin + Mathf.FloorToInt(SizeZ * 0.5f);
            return boundsInt.zMin + SizeZ / 2;
        }
        private set { }
    }
    public int CenterX
    {
        get
        {
            //return boundsInt.xMin + Mathf.FloorToInt(SizeX * 0.5f);
            return boundsInt.xMin + SizeX / 2;
        }
        private set { }
    }
    public Vector2 Midpoint
    {
        get
        {
            return new Vector2(CenterX, CenterZ);
        }
        private set { }
    }

    public Room(Coordinate bottomLeft, int width, int height)
    {
        this.boundsInt = new BoundsInt((Vector3Int)bottomLeft, new Vector3Int(width, 2, height));
    }
    public Room(Vector3Int bottomLeft, int width, int height)
    {
        this.boundsInt = new BoundsInt(bottomLeft, new Vector3Int(width, 2, height));
    }
    public Room(int x, int z, int width, int height)
    {
        this.boundsInt = new BoundsInt(new Vector3Int(x, 0, z), new Vector3Int(width, 2, height));
    }

    public bool IsOverlapping(Room room)
    {
        return (boundsInt.xMax >= room.boundsInt.xMin) && (room.boundsInt.xMax >= boundsInt.xMin) && (boundsInt.zMax >= room.boundsInt.zMin) && (room.boundsInt.zMax >= boundsInt.zMin);
    }
    public bool IsOverlappingExclusive(Room room)
    {
        return (boundsInt.xMax > room.boundsInt.xMin) && (room.boundsInt.xMax > boundsInt.xMin) && (boundsInt.zMax > room.boundsInt.zMin) && (room.boundsInt.zMax > boundsInt.zMin);
    }
    public bool IsOverlapping(Room room, bool isExclusive)
    {
        if (isExclusive)
        {
            return IsOverlappingExclusive(room);
        }
        else
        {
            return IsOverlapping(room);
        }
    }

    public Coordinate GetCenter()
    {
        return new Coordinate(CenterX, CenterZ);
    }

    public Coordinate GetRandomWallCoordinate(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new Coordinate(Random.Range(boundsInt.xMin + 1, boundsInt.xMax), boundsInt.zMax);
            case Direction.East:
                return new Coordinate(boundsInt.xMax, Random.Range(boundsInt.zMin + 1, boundsInt.zMax));
            case Direction.South:
                return new Coordinate(Random.Range(boundsInt.xMin + 1, boundsInt.xMax), boundsInt.zMin);
            case Direction.West:
                return new Coordinate(boundsInt.xMin, Random.Range(boundsInt.zMin + 1, boundsInt.zMax));
            default:
                Debug.LogError("Wrong direction!");
                return new Coordinate();
        }
    }

    public void Respawn(int x, int z, int width, int height)
    {
        ChangePosition(x, z);
        Resize(width, height);
    }

    public void ChangePosition(int x, int z)
    {
        boundsInt.position = new Vector3Int(x, 0, z);
    }
    public void Resize(int width, int height)
    {
        boundsInt.size = new Vector3Int(width, 2, height);
    }

    public override string ToString()
    {
        return "Room type: " + type + "; xMin = " + boundsInt.xMin + "; zMin = " + boundsInt.zMin + "; width = " + SizeX + "; height = " + SizeZ;
    }
}
