using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distances : Dictionary<MazeCell, int> {

    MazeCell startingCell;

    public Distances(MazeCell root) : base()
    {
        this.startingCell = root;
        this[root] = 0;
    }

    public void SetDistance(MazeCell cell, int distance)
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

    public List<MazeCell> Cells()
    {
        return new List<MazeCell>(this.Keys);
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
    public Distances DijkstraShortestPathTo(MazeCell goal)
    {
        MazeCell currentCell = goal;
        // навигационная цепочка
        Distances breadcrumbTrail = new Distances(this.startingCell);
        breadcrumbTrail[currentCell] = this[currentCell];

        // starightforward algo -  For each cell along the path, the neighboring cell with the lowest distance will be the next step of the solution
        while (currentCell != startingCell)
        {
            foreach (MazeCell neighbourCell in currentCell.Links()) //get all linked cells of current cell
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

    public MazeCell MaxDistanceMazeCell()
    {
        int maxDistance = 0;
        MazeCell maxCell = this.startingCell;

        foreach (MazeCell cell in this.Keys)
        {
            if (this[cell] > maxDistance)
            {
                maxCell = cell;
                maxDistance = this[cell];
            }
        }
        return maxCell;
    }

    public Distances GetLongestPath()
    {
        //Distances distances = this.startingCell.FindDistanceForAllReachableLinkedCells();
        //MazeCell maxDistanceMazeCell = distances.MaxDistanceMazeCell();
        //Distances distancesFromMax = maxDistanceMazeCell.FindDistanceForAllReachableLinkedCells();
        //return distancesFromMax.DijkstraShortestPathTo(maxDistanceMazeCell);

        Distances distances = this.startingCell.FindDistanceForAllReachableLinkedCells().MaxDistanceMazeCell().FindDistanceForAllReachableLinkedCells();
        return distances.DijkstraShortestPathTo(distances.MaxDistanceMazeCell());
    }

    public Distances BFSPathTo(MazeCell goal)
    {
        Queue<MazeCell> grayQueue = new Queue<MazeCell>();
        grayQueue.Enqueue(this.startingCell);

        Distances distances = new Distances(startingCell);

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

                if (grayQueue.Contains(linkedCell))
                {
                    continue;
                }
                grayQueue.Enqueue(linkedCell);
            }
        }

        Distances pathToGoal = new Distances(this.startingCell);
        pathToGoal.Add(current, distances[current]);

        while (current != startingCell)
        {
            foreach (MazeCell neighbour in current.Links())
            {
                if (distances[neighbour] < distances[current])
                {
                    if (!pathToGoal.ContainsKey(neighbour))
                    {
                        pathToGoal.Add(neighbour, distances[neighbour]);
                    }
                    else
                    {
                        pathToGoal[neighbour] = distances[neighbour];
                    }
                    current = neighbour;
                    break;
                }
            }
        }

        return pathToGoal;
    }

    public Distances BFSMeetInMiddlePathTo(MazeCell goal)
    {
        Queue<MazeCell> grayQueueWithStartingCell = new Queue<MazeCell>();
        Queue<MazeCell> grayQueueWithGoalCell = new Queue<MazeCell>();

        grayQueueWithStartingCell.Enqueue(this.startingCell);
        grayQueueWithGoalCell.Enqueue(goal);

        Distances distancesStarting = new Distances(startingCell);
        Distances distancesGoal = new Distances(goal);

        bool notFound = true;

        var currentCell = startingCell;

        while (notFound)
        {
            currentCell = grayQueueWithStartingCell.Dequeue();

            foreach (var linkedCell in currentCell.Links())
            {
                if (distancesGoal.ContainsKey(linkedCell))
                {
                    notFound = false;
                    if (!distancesStarting.ContainsKey(linkedCell))
                    {
                        distancesStarting.Add(linkedCell, distancesStarting[currentCell] + 1);
                    }
                    currentCell = linkedCell;
                    break;
                }

                if (distancesStarting.ContainsKey(linkedCell))
                {
                    continue;
                }
                distancesStarting.Add(linkedCell, distancesStarting[currentCell] + 1);

                if (grayQueueWithStartingCell.Contains(linkedCell))
                {
                    continue;
                }
                grayQueueWithStartingCell.Enqueue(linkedCell);
            }

            if (!notFound)
            {
                break;
            }

            currentCell = grayQueueWithGoalCell.Dequeue();

            foreach (var linkedCell in currentCell.Links())
            {
                if (distancesStarting.ContainsKey(linkedCell))
                {
                    notFound = false;
                    if (!distancesGoal.ContainsKey(linkedCell))
                    {
                        distancesGoal.Add(linkedCell, distancesGoal[currentCell] + 1);
                    }
                    currentCell = linkedCell;
                    break;
                }

                if (distancesGoal.ContainsKey(linkedCell))
                {
                    continue;
                }
                distancesGoal.Add(linkedCell, distancesGoal[currentCell] + 1);

                if (grayQueueWithGoalCell.Contains(linkedCell))
                {
                    continue;
                }
                grayQueueWithGoalCell.Enqueue(linkedCell);
            }
        }

        Distances breadcrumbs = new Distances(this.startingCell);
        breadcrumbs.Add(currentCell, distancesStarting[currentCell]);

        MazeCell breadcrumbsCurrent = currentCell;

        while (currentCell != startingCell)
        {
            foreach (MazeCell neighbour in currentCell.Links())
            {
                if (!distancesStarting.ContainsKey(neighbour))
                {
                    continue;
                }

                if (distancesStarting[neighbour] < distancesStarting[currentCell])
                {
                    if (!breadcrumbs.ContainsKey(neighbour))
                    {
                        breadcrumbs.Add(neighbour, distancesStarting[neighbour]);
                    }
                    else
                    {
                        breadcrumbs[neighbour] = distancesStarting[neighbour];
                    }

                    currentCell = neighbour;
                    break;
                }
            }
        }

        currentCell = breadcrumbsCurrent;
        Distances breadcrumbsGoal = new Distances(goal);

        while (currentCell != goal)
        {
            foreach (MazeCell neighbour in currentCell.Links())
            {
                if (!distancesGoal.ContainsKey(neighbour))
                {
                    continue;
                }

                if (distancesGoal[neighbour] < distancesGoal[currentCell])
                {
                    if (!breadcrumbsGoal.ContainsKey(neighbour))
                    {
                        breadcrumbsGoal.Add(neighbour, distancesGoal[neighbour]);
                    }
                    else
                    {
                        breadcrumbsGoal[neighbour] = distancesGoal[neighbour];
                    }

                    currentCell = neighbour;
                    break;
                }
            }
        }

        MazeCell maxCell = null;
        MazeCell minCell = null;

        int max = breadcrumbsGoal.MaxDistanceValue();
        int min = breadcrumbsGoal.MinDistanceValue();

        while (max > min)
        {
            foreach (MazeCell cell in breadcrumbsGoal.Keys)
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

        foreach (MazeCell cell in breadcrumbsGoal.Keys)
        {
            breadcrumbs.Add(cell, breadcrumbsGoal[cell] + max);
        }

        return breadcrumbs;
    }


    public Distances DFSPathTo(MazeCell goal)
    {
        Stack<MazeCell> stack = new Stack<MazeCell>();
        stack.Push(this.startingCell);

        Distances distances = new Distances(startingCell);

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

                if (stack.Contains(linkedCell))
                {
                    continue;
                }
                stack.Push(linkedCell);
            }
        }

        Distances breadcrumbs = new Distances(this.startingCell);
        breadcrumbs.Add(current, distances[current]);

        while (current != startingCell)
        {
            foreach (MazeCell neighbour in current.Links())
            {
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

        return breadcrumbs;
    } 
}
