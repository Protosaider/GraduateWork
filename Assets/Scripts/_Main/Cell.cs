using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// It's possible to make value (like CellType) to be null
/// Introducing - Nullable!
/// Представляет тип значения, которому можно присвоить значение null.
/// Для этого при создании поля \ переменной используем ?
/// public CellType? type = null;
/// Какие минусы? Ну, это обертка, поэтому значения получаем через метод Value, проверяем через HasValue

[System.Flags]
public enum CleanUpCells
{
    None = 0,
    TopLeft = 1,
    TopCenter = 1 << 1,
    TopRight = 1 << 2,
    //BottomLeft = 1,
    //BottomCenter = 1 << 1,
    //BottomRight = 1 << 2,
    MiddleLeft = 1 << 3,
    MiddleCenter = 1 << 4,
    MiddleRight = 1 << 5,
    BottomLeft = 1 << 6,
    BottomCenter = 1 << 7,
    BottomRight = 1 << 8,
    //TopLeft = 1 << 6,
    //TopCenter = 1 << 7,
    //TopRight = 1 << 8,

    //BottomLeft = 1,
    //MiddleLeft = 1 << 1,
    //TopLeft = 1 << 2,
    //BottomCenter = 1 << 3,
    //MiddleCenter = 1 << 4,
    //TopCenter = 1 << 5,
    //BottomRight = 1 << 6,
    //MiddleRight = 1 << 7,
    //TopRight = 1 << 8
}

[System.Serializable]
public enum CellType
{
    //Null = -1,
    //Empty,
    Empty = -1,
    Floor,
    Wall,
    Door,
    OuterWall,
}

public enum Direction
{
    Nowhere = -1,
    North,
    East,
    South,
    West,
    Up,
    Down,
    NorthEast,
    EastSouth,
    SouthWest,
    WestNorth,
    //
    //N,
    //E,
    //S,
    //W,
    //U,
    //D,
    //NE,
    //ES,
    //SW,
    //WN,
}

[System.Serializable]
public struct Coordinate
{
    public int x;
    public int y;
    public int z;

    public Coordinate(int x, int z)
    {
        this.x = x;
        this.y = 0;
        this.z = z;
    }

    public Coordinate(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Coordinate GetOffset(Direction direction)
    {
        if (direction == Direction.Nowhere)
        {
            Debug.LogError("Can't get offset from Direction.Nowhere. Return empty Coordinate.");
            return new Coordinate(0, 0, 0);
        }
        return offsets[(int)direction];
    }

    public static Coordinate GetOffset(int direction)
    {
        if (direction < 0 || direction >= offsets.Length)
        {
            Debug.LogError("Can't get offset: index is out of range. Return empty Coordinate.");
            return new Coordinate(0, 0, 0);
        }
        return offsets[direction];
    }

    public static Coordinate GetPointyHexOffset(Direction direction, bool isEven)
    {
        if (direction == Direction.Nowhere)
        {
            Debug.LogError("Can't get offset from Direction.Nowhere. Return empty Coordinate.");
            return new Coordinate(0, 0, 0);
        }

        if (isEven && (direction == Direction.NorthEast || direction == Direction.EastSouth))
        {
            return pointyHexOffsets[0];
        }
        else if (!isEven && (direction == Direction.SouthWest || direction == Direction.WestNorth))
        {
            return pointyHexOffsets[1];
        }

        return new Coordinate(0, 0, 0);
    }

    private static Coordinate[] pointyHexOffsets =
    {
        new Coordinate(-1, 0, 0), //0, 2, 4
        new Coordinate(1, 0, 0),
    };

    private static Coordinate[] offsets =
    {
        new Coordinate(0, 0, 1),    //North PositiveZ
        new Coordinate(1, 0, 0),    //East  PositiveX
        new Coordinate(0, 0, -1),   //South NegativeZ
        new Coordinate(-1, 0, 0),   //West  NegativeX

        new Coordinate(0, 1, 0),    //Up    PositiveY
        new Coordinate(0, -1, 0),   //Down  NegativeY

        new Coordinate(1, 0, 1),    //NorthEast PositiveX PositiveZ
        new Coordinate(1, 0, -1),   //EastSouth PositiveX NegativeZ
        new Coordinate(-1, 0, -1),  //SouthWest NegativeX NegativeZ
        new Coordinate(-1, 0, 1),   //WestNorth NegativeX PositiveZ
    };

    public static bool operator ==(Coordinate coord1, Coordinate coord2)
    {
        return (coord1.x == coord2.x) && (coord1.y == coord2.y) && (coord1.z == coord2.z);
    }

    public static bool operator !=(Coordinate coord1, Coordinate coord2)
    {
        return !(coord1 == coord2);
    }

    //public static Coordinate operator -(Coordinate coord1, Coordinate coord2) => new Coordinate(coord1.x - coord2.x, coord1.y - coord2.y, coord1.z - coord2.z);
    public static Coordinate operator -(Coordinate coord1, Coordinate coord2)
    {
        return new Coordinate(coord1.x - coord2.x, coord1.y - coord2.y, coord1.z - coord2.z);
    }

    public static Coordinate operator +(Coordinate coord1, Coordinate coord2)
    {
        return new Coordinate(coord1.x + coord2.x, coord1.y + coord2.y, coord1.z + coord2.z);
    }

    public override string ToString()
    {
        return "Coordinate{\"x\":" + this.x + ", \"y\":" + this.y + ", \"z\":" + this.z + "}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Coordinate))
        {
            return false;
        }

        var coordinate = (Coordinate)obj;
        return x == coordinate.x &&
               y == coordinate.y &&
               z == coordinate.z;
    }

    public override int GetHashCode()
    {
        var hashCode = 373119288;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        hashCode = hashCode * -1521134295 + z.GetHashCode();
        return hashCode;
    }

    public static explicit operator UnityEngine.Vector3(Coordinate coordinate)
    {
        return new Vector3(coordinate.x, coordinate.y, coordinate.z);
    }

    public static explicit operator UnityEngine.Vector3Int(Coordinate coordinate)
    {
        return new Vector3Int(coordinate.x, coordinate.y, coordinate.z);
    }

    public static explicit operator Coordinate(Vector3Int v)
    {
        return new Coordinate(v.x, v.y, v.z);
    }
}

[System.Serializable]
public class Cell
{
    public bool isWalkable;
    public CellType cellType;
    public Color cellMapColor;
    public char cellMapChar;

    public Cell(bool isWalkable, CellType cellType, Color color, char cellChar)
    {
        this.isWalkable = isWalkable;
        this.cellType = cellType;
        this.cellMapColor = color;
        this.cellMapChar = cellChar;
    }

    public Cell()
    {
        isWalkable = false;
        cellType = CellType.Empty;
        cellMapColor = Color.clear;
        cellMapChar = ' ';
    }
    /// C# 7.0
    //public ref CellType GetType()
    //{
    //    return ref cellType;
    //}

    public static Cell CreateCell(Color color)
    {
        if (color == Color.grey || color == Color.gray)
        {
            return new Cell(true, CellType.Floor, Color.grey, '=');
        }      
        else if (color == Color.red)
        {
            return new Cell(false, CellType.Wall, Color.red, '#');
        }
        else if (color == Color.cyan)
        {
            return new Cell(true, CellType.Door, Color.cyan, '0');
        }
        else if (color == Color.blue)
        {
            return new Cell(false, CellType.OuterWall, Color.blue, '@');
        }

        //color == Color.clear
        return new Cell();
    }

    public static Cell CreateCell(char character)
    {
        if (character == '=')
        {
            return new Cell(true, CellType.Floor, Color.grey, '=');
        }
        else if (character == '#')
        {
            return new Cell(false, CellType.Wall, Color.red, '#');
        }
        else if (character == '0')
        {
            return new Cell(true, CellType.Door, Color.cyan, '0');
        }
        else if (character == '@')
        {
            return new Cell(false, CellType.OuterWall, Color.blue, '@');
        }

        //color == Color.clear
        return new Cell();
    }

    public static Cell CreateCell(CellType type)
    {
        if (type == CellType.Floor)
        {
            return new Cell(true, CellType.Floor, Color.grey, '=');
        }
        else if (type == CellType.Wall)
        {
            return new Cell(false, CellType.Wall, Color.red, '#');
        }
        else if (type == CellType.Door)
        {
            return new Cell(true, CellType.Door, Color.cyan, '0');
        }
        else if (type == CellType.OuterWall)
        {
            return new Cell(false, CellType.OuterWall, Color.blue, '@');
        }

        //color == Color.clear
        return new Cell();
    }
}
