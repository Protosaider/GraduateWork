using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedMazeCell
{

    public int weight
    {
        get;
        set;
    }

    public int X
    {
        get;
        private set;
    }

    public int Z
    {
        get;
        private set;
    }

    //neighbours
    public WeightedMazeCell North
    {
        get;
        set;
    }
    public WeightedMazeCell West
    {
        get;
        set;
    }
    public WeightedMazeCell South
    {
        get;
        set;
    }
    public WeightedMazeCell East
    {
        get;
        set;
    }

    public List<WeightedMazeCell> Neighbours
    {
        get
        {
            var list = new List<WeightedMazeCell>();
            if (North != null)
            {
                list.Add(North);
            }
            if (West != null)
            {
                list.Add(West);
            }
            if (South != null)
            {
                list.Add(South);
            }
            if (East != null)
            {
                list.Add(East);
            }
            return list;
        }
    }

    private Dictionary<WeightedMazeCell, bool> links;

    public WeightedMazeCell(int gridX, int gridZ)
    {
        this.X = gridX;
        this.Z = gridZ;
        weight = 1;

        links = new Dictionary<WeightedMazeCell, bool>();
    }

    public WeightedMazeCell Link(WeightedMazeCell cell, bool isBidirectional = true)
    {
        links[cell] = true;
        if (isBidirectional)
        {
            cell.Link(this, false);
        }
        return this;
    }

    public WeightedMazeCell Unlink(WeightedMazeCell cell, bool isBidirectional = true)
    {
        links.Remove(cell);
        if (isBidirectional)
        {
            cell.Unlink(this, false);
        }
        return this;
    }

    public List<WeightedMazeCell> Links()
    {
        return new List<WeightedMazeCell>(links.Keys);
    }

    public bool IsLinked(WeightedMazeCell cell)
    {
        if (cell == null) //don't forget!
        {
            return false;
        }
        return links.ContainsKey(cell);
    }

    public Cell GetNeighbourAsCell(Direction dir, ref CellType mazeFloor, ref CellType mazeWall)
    {
        switch (dir)
        {
            case Direction.North:
                return IsLinked(North) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            case Direction.East:
                return IsLinked(East) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            case Direction.South:
                return IsLinked(South) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            case Direction.West:
                return IsLinked(West) ? Cell.CreateCell(mazeFloor) : Cell.CreateCell(mazeWall);
            default:
                Debug.LogError("Can't find neighbour of maze cell");
                return Cell.CreateCell(CellType.Empty);
        }
    }

    public WeightedMazeCell GetRandomNeighbour()
    {
        return Neighbours[Random.Range(0, Neighbours.Count)];
    }


    public WeightedDistances FindDistanceForAllReachableLinkedCells()
    {
        WeightedDistances weights = new WeightedDistances(this);
        List<WeightedMazeCell> frontier = new List<WeightedMazeCell>() { this };

        // Will expands until last connected cell found
        while (frontier.Count > 0)
        {
            //frontier.Sort((x, y) => x.weight.CompareTo(y.weight)); frontier.Reverse();
            //or
            //frontier.Sort((x, y) => -1 * x.weight.CompareTo(y.weight)); //descending order

            frontier.Sort((x, y) => weights[x].CompareTo(weights[y])); //ascending orders
            WeightedMazeCell currentCell = frontier[0];
            frontier.Remove(currentCell);

            foreach (var neighbour in currentCell.Links())
            {
                int totalWeight = weights[currentCell] + neighbour.weight;

                if (!weights.ContainsKey(neighbour) || totalWeight < weights[neighbour])
                {
                    frontier.Add(neighbour);
                    weights[neighbour] = totalWeight;
                }
            }
        }

        return weights;
    }
}

public class PriorityQueue<TPriority, TItem>
{
    readonly SortedDictionary<TPriority, Queue<TItem>> subqueues;

    public PriorityQueue(IComparer<TPriority> priorityComparer)
    {
        subqueues = new SortedDictionary<TPriority, Queue<TItem>>(priorityComparer);

    }

    public PriorityQueue() : this(Comparer<TPriority>.Default) { }

    public bool HasItems
    {
        //get { return subqueues.Any(); }
        get { return subqueues.Count > 0; }
    }

    public int Count
    {
        //get { return subqueues.Sum(q => q.Value.Count); }
        get
        {
            int result = 0;
            foreach (var item in subqueues.Values)
            {
                result += item.Count;
            }
            return result;
        }
    }

    private void AddQueueOfPriority(TPriority priority)
    {
        subqueues.Add(priority, new Queue<TItem>());
    }

    public void Enqueue(TPriority priority, TItem item)
    {
        if (!subqueues.ContainsKey(priority))
        {
            AddQueueOfPriority(priority);
        }
        subqueues[priority].Enqueue(item);
    }

    private TItem DequeueFromHighPriorityQueue()
    {
        //KeyValuePair<TPriority, Queue<TItem>> first = subqueues.First();
        var enumerator = subqueues.GetEnumerator();
        enumerator.MoveNext();
        var first = enumerator.Current;

        TItem nextItem = first.Value.Dequeue();
        if (!(first.Value.Count > 0))
        //if (!first.Value.Any())
        {
            subqueues.Remove(first.Key);
        }
        return nextItem;
    }

    public TItem Dequeue()
    {
        //if (subqueues.Any())
        if (subqueues.Count > 0)
            return DequeueFromHighPriorityQueue();
        else
            throw new System.InvalidOperationException("The queue is empty");
    }

    public TItem Peek()
    {
        if (HasItems)
        {
            var enumerator = subqueues.GetEnumerator();
            enumerator.MoveNext();
            var first = enumerator.Current;
            return first.Value.Peek();
        }
            //return subqueues.First().Value.Peek();
        else
            throw new System.InvalidOperationException("The queue is empty");
    }
}

//public Any(this IEnumerable<T> coll, Func<T, bool> predicate)
//{
//    foreach (T t in coll)
//    {
//        if (predicate(t))
//            return true;
//    }
//    return false;
//}
