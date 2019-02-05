using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool isWalkable;

    public int x;
    public int z;

    public int gCost;
    public int hCost;
    public Node parent;

    public int movementPenalty;

    private int heapIndex;

    public Node(bool walkable, int gridXPos, int gridZPos, int penalty)
    {
        isWalkable = walkable;
        x = gridXPos;
        z = gridZPos;
        movementPenalty = penalty;
    }

    public Node(bool walkable, int penalty)
    {
        isWalkable = walkable;
        movementPenalty = penalty;
        x = -1;
        z = -1;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
        private set { }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = this.fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = this.hCost.CompareTo(other.hCost);
        }
        return -compare; //return -1 if other node is lower (this node is higher)
    }

    public static Node CreateNode(CellType type, int x, int y)
    {
        if (type == CellType.Empty)
        {
            return new Node(true, x, y, 3);
        }
        else if (type == CellType.Floor)
        {
            return new Node(true, x, y, 2);
        }
        else if (type == CellType.Wall)
        {
            return new Node(true, x, y, 25);
        }
        else if (type == CellType.Door)
        {
            return new Node(true, x, y, 2);
        }
        else if (type == CellType.OuterWall)
        {
            return new Node(false, x, y, 10000);
        }

        //color == Color.clear
        return new Node(true, 0);
    }

    public override string ToString()
    {
        return movementPenalty.ToString("D6");
    }

    //// A shared instance that can be used for comparisons
    //public static readonly IHeapItem<Node> Null = new NullNode(default(bool), default(Vector3), default(int), default(int), default(int));

    //// The Null Case: this NullAnimal class should be used in place of C# null keyword.
    //private class NullNode : Node
    //{
    //    public NullNode(bool walkable, Vector3 worldPosition, int gridXPos, int gridZPos, int penalty) : base(walkable, worldPosition, gridXPos, gridZPos, penalty)
    //    {
    //    }
    //}
}
