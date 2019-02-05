using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedDistances : Dictionary<WeightedMazeCell, int> {

    WeightedMazeCell startingCell;

    public WeightedDistances(WeightedMazeCell root) : base()
    {
        this.startingCell = root;
        this[root] = 0;
    }

    public void SetDistance(WeightedMazeCell cell, int distance)
    {
        if (!this.ContainsKey(cell))
        {
            this.Add(cell, distance);
        }
        else
        {
            this[cell] = distance;
        }
    }

    public List<WeightedMazeCell> Cells()
    {
        return new List<WeightedMazeCell>(this.Keys);
    }

    public int MaxDistanceValue()
    {
        int max = 0;
        foreach (int value in this.Values)
        {
            if (max < value)
            {
                max = value;
            }
        }
        return max;
    }

    public int MinDistanceValue()
    {
        int min = 0;
        foreach (int value in this.Values)
        {
            if (min > value)
            {
                min = value;
            }
        }
        return min;
    }

    //Dijkstra algorithm - go backwards - from goal cell to root cell
    public WeightedDistances DijkstraShortestPathTo(WeightedMazeCell goal)
    {
        WeightedMazeCell currentCell = goal;
        // навигационная цепочка
        WeightedDistances breadcrumbTrail = new WeightedDistances(this.startingCell);
        breadcrumbTrail[currentCell] = this[currentCell];

        // starightforward algo -  For each cell along the path, the neighboring cell with the lowest distance will be the next step of the solution
        while (currentCell != startingCell)
        {
            foreach (WeightedMazeCell neighbourCell in currentCell.Links()) //get all linked cells of current cell
            {
                if (this[neighbourCell] < this[currentCell])        //find linked cell with min distance (closest to the startingCell)
                {
                    if (!breadcrumbTrail.ContainsKey(neighbourCell))// add to dictionary if not exists in dictionary
                    {
                        breadcrumbTrail.Add(neighbourCell, this[neighbourCell]);
                    }
                    else
                    {
                        breadcrumbTrail[neighbourCell] = this[neighbourCell]; // replace distance if less distance value found
                    }

                    currentCell = neighbourCell;                    // switch current cell to the neighbour
                    break;                 
                }
            }
        }
        return breadcrumbTrail;
    }

    public WeightedMazeCell MaxDistanceMazeCell()
    {
        int maxDistance = 0;
        WeightedMazeCell maxCell = this.startingCell;

        foreach (WeightedMazeCell cell in this.Keys)
        {
            if (this[cell] > maxDistance)
            {
                maxCell = cell;
                maxDistance = this[cell];
            }
        }
        return maxCell;
    }

    public WeightedDistances GetLongestPath()
    {
        //Distances distances = this.startingCell.FindDistanceForAllReachableLinkedCells();
        //MazeCell maxDistanceMazeCell = distances.MaxDistanceMazeCell();
        //Distances distancesFromMax = maxDistanceMazeCell.FindDistanceForAllReachableLinkedCells();
        //return distancesFromMax.DijkstraShortestPathTo(maxDistanceMazeCell);

        WeightedDistances distances = this.startingCell.FindDistanceForAllReachableLinkedCells().MaxDistanceMazeCell().FindDistanceForAllReachableLinkedCells();
        return distances.DijkstraShortestPathTo(distances.MaxDistanceMazeCell());
    }

    public WeightedDistances BFSPathTo(WeightedMazeCell goal)
    {
        Queue<WeightedMazeCell> grayQueue = new Queue<WeightedMazeCell>();
        grayQueue.Enqueue(this.startingCell);

        WeightedDistances distances = new WeightedDistances(startingCell);

        var current = startingCell;

        while (current != goal)
        {
            current = grayQueue.Dequeue();

            foreach (var linkedCell in current.Links())
            {
                if (distances.ContainsKey(linkedCell))
                {
                    continue;
                }
                if (linkedCell == goal)
                {
                    distances.Add(linkedCell, distances[current] + 1);
                    current = linkedCell;
                    break;
                }
                distances.Add(linkedCell, distances[current] + 1);

                //if (this.ContainsKey(linkedCell))
                //{
                //    continue;
                //}
                //this.Add(linkedCell, this[current] + 1);

                if (grayQueue.Contains(linkedCell))
                {
                    continue;
                }
                grayQueue.Enqueue(linkedCell);
            }
        }

        WeightedDistances breadcrumbs = new WeightedDistances(this.startingCell);
        //breadcrumbs.Add(current, this[current]);
        breadcrumbs.Add(current, distances[current]);

        while (current != startingCell)
        {
            foreach (WeightedMazeCell neighbour in current.Links())
            {
                //if (this[neighbour] < this[current])
                if (distances[neighbour] < distances[current])
                {
                    if (!breadcrumbs.ContainsKey(neighbour))
                    {
                        //breadcrumbs.Add(neighbour, this[neighbour]);
                        breadcrumbs.Add(neighbour, distances[neighbour]);
                    }
                    else
                    {
                        //breadcrumbs[neighbour] = this[neighbour];
                        breadcrumbs[neighbour] = distances[neighbour];
                    }

                    current = neighbour;

                    break;
                }
            }
        }

        return breadcrumbs;
    }

    public WeightedDistances BFSMeetInMiddlePathTo(WeightedMazeCell goal)
    {
        Queue<WeightedMazeCell> grayQueue = new Queue<WeightedMazeCell>();
        Queue<WeightedMazeCell> grayQueueGoal = new Queue<WeightedMazeCell>();
        grayQueue.Enqueue(this.startingCell);
        grayQueueGoal.Enqueue(goal);

        WeightedDistances distances = new WeightedDistances(startingCell);
        WeightedDistances distancesGoal = new WeightedDistances(goal);

        bool notFound = true;

        var current = startingCell;

        while (notFound)
        {
            current = grayQueue.Dequeue();

            foreach (var linkedCell in current.Links())
            {
                if (distancesGoal.ContainsKey(linkedCell))
                {
                    notFound = false;
                    if (!distances.ContainsKey(linkedCell))
                    {
                        distances.Add(linkedCell, distances[current] + 1);
                    }
                    current = linkedCell;
                    break;
                }

                if (distances.ContainsKey(linkedCell))
                {
                    continue;
                }

                distances.Add(linkedCell, distances[current] + 1);

                if (grayQueue.Contains(linkedCell))
                {
                    continue;
                }
                grayQueue.Enqueue(linkedCell);
            }

            if (!notFound)
            {
                break;
            }

            current = grayQueueGoal.Dequeue();

            foreach (var linkedCell in current.Links())
            {

                if (distances.ContainsKey(linkedCell))
                {
                    notFound = false;
                    if (!distancesGoal.ContainsKey(linkedCell))
                    {
                        distancesGoal.Add(linkedCell, distancesGoal[current] + 1);
                    }
                    current = linkedCell;
                    break;
                }

                if (distancesGoal.ContainsKey(linkedCell))
                {
                    continue;
                }

                distancesGoal.Add(linkedCell, distancesGoal[current] + 1);

                if (grayQueueGoal.Contains(linkedCell))
                {
                    continue;
                }
                grayQueueGoal.Enqueue(linkedCell);
            }
        }

        WeightedDistances breadcrumbs = new WeightedDistances(this.startingCell);
        breadcrumbs.Add(current, distances[current]);

        WeightedMazeCell breadcrumbsCurrent = current;

        while (current != startingCell)
        {
            foreach (WeightedMazeCell neighbour in current.Links())
            {
                if (!distances.ContainsKey(neighbour))
                {
                    continue;
                }

                if (distances[neighbour] < distances[current])
                {
                    if (!breadcrumbs.ContainsKey(neighbour))
                    {
                        breadcrumbs.Add(neighbour, distances[neighbour]);
                    }
                    else
                    {
                        breadcrumbs[neighbour] = distances[neighbour];
                    }

                    current = neighbour;
                    break;
                }
            }
        }

        current = breadcrumbsCurrent;
        WeightedDistances breadcrumbsGoal = new WeightedDistances(goal);

        while (current != goal)
        {
            foreach (WeightedMazeCell neighbour in current.Links())
            {
                if (!distancesGoal.ContainsKey(neighbour))
                {
                    continue;
                }

                if (distancesGoal[neighbour] < distancesGoal[current])
                {
                    if (!breadcrumbsGoal.ContainsKey(neighbour))
                    {
                        breadcrumbsGoal.Add(neighbour, distancesGoal[neighbour]);
                    }
                    else
                    {
                        breadcrumbsGoal[neighbour] = distancesGoal[neighbour];
                    }

                    current = neighbour;
                    break;
                }
            }
        }

        WeightedMazeCell maxCell = null;
        WeightedMazeCell minCell = null;

        int max = breadcrumbsGoal.MaxDistanceValue();
        int min = breadcrumbsGoal.MinDistanceValue();

        while (max > min)
        {
            foreach (WeightedMazeCell cell in breadcrumbsGoal.Keys)
            {
                if (breadcrumbsGoal[cell] == min)
                {
                    minCell = cell;
                }

                if (breadcrumbsGoal[cell] == max)
                {
                    maxCell = cell;
                }
            }

            breadcrumbsGoal[minCell] = max;
            breadcrumbsGoal[maxCell] = min;
            max--;
            min++;
        }

        max = breadcrumbs.MaxDistanceValue();
        max++;

        foreach (WeightedMazeCell cell in breadcrumbsGoal.Keys)
        {
            breadcrumbs.Add(cell, breadcrumbsGoal[cell] + max);
        }

        return breadcrumbs;
    }


    public WeightedDistances DFSPathTo(WeightedMazeCell goal)
    {
        Stack<WeightedMazeCell> stack = new Stack<WeightedMazeCell>();
        stack.Push(this.startingCell);

        WeightedDistances distances = new WeightedDistances(startingCell);

        var current = startingCell;

        while (current != goal)
        {
            current = stack.Peek();

            foreach (var linkedCell in current.Links())
            {
                if (distances.ContainsKey(linkedCell))
                {
                    continue;
                }

                distances.Add(linkedCell, distances[current] + 1);

                if (linkedCell == goal)
                {
                    current = linkedCell;
                    break;
                }                    

                //if (this.ContainsKey(linkedCell))
                //{
                //    continue;
                //}
                //this.Add(linkedCell, this[current] + 1);

                if (stack.Contains(linkedCell))
                {
                    continue;
                }

                stack.Push(linkedCell);
            }
        }

        WeightedDistances breadcrumbs = new WeightedDistances(this.startingCell);
        //breadcrumbs.Add(current, this[current]);
        breadcrumbs.Add(current, distances[current]);

        while (current != startingCell)
        {
            foreach (WeightedMazeCell neighbour in current.Links())
            {
                //if (this[neighbour] < this[current])
                if (distances[neighbour] < distances[current])
                {
                    if (!breadcrumbs.ContainsKey(neighbour))
                    {
                        //breadcrumbs.Add(neighbour, this[neighbour]);
                        breadcrumbs.Add(neighbour, distances[neighbour]);
                    }
                    else
                    {
                        //breadcrumbs[neighbour] = this[neighbour];
                        breadcrumbs[neighbour] = distances[neighbour];
                    }

                    current = neighbour;

                    break;
                }
            }
        }

        return breadcrumbs;
    }
   
}
