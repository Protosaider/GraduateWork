using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {

    //! Returns List<Node>
    public List<Node> FindPath(Node startNode, Node targetNode, ref NodeGrid grid, bool isPathSimplified = false, bool isPathFromStartToTarget = true, bool hasDiagonalMovement = true)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        bool pathSuccess = false;

        //Node startNode = grid.NodeFromWorldPosition(startPos);
        //Node targetNode = grid.NodeFromWorldPosition(targetPos);

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            //Heap<Node> openSet = new Heap<Node>(grid.MaxHeapSize);
            Heap<Node> openSet = new Heap<Node>(grid.MaxHeapSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.Remove();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    pathSuccess = true;
                    Debug.Log("Path found: " + sw.ElapsedMilliseconds + " ms");
                    break;
                }

                foreach (var neighbourNode in grid.GetNeighbours(currentNode, hasDiagonalMovement))
                {
                    if (!neighbourNode.isWalkable || closedSet.Contains(neighbourNode))
                    {
                        continue;
                    }
                    //count distance for each neighbour at grid generation => create set/array/list of neighbours?
                    int movementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode) + neighbourNode.movementPenalty;
                    bool isInOpenSet = openSet.Contains(neighbourNode);

                    if (movementCostToNeighbour < neighbourNode.gCost || !isInOpenSet)
                    {
                        neighbourNode.gCost = movementCostToNeighbour;
                        neighbourNode.hCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.parent = currentNode;

                        if (!isInOpenSet)
                        {
                            openSet.Add(neighbourNode);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            List<Node> path = RetracePath(startNode, targetNode);

            if (isPathSimplified)
            {
                SimplifyPathReturnAsNodes(ref path);
                pathSuccess = path.Count > 0;              
            }

            if (isPathFromStartToTarget)
            {
                ReversePath(ref path);
            }

            if (pathSuccess)
            {
                return path;
            }
        }

        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        //Vector3[] waypoints = SimplifyPath(path);
        //System.Array.Reverse(waypoints);
        return path;
    }

    private List<Node> SimplifyPathReturnAsNodes(ref List<Node> path)
    {
        List<Node> waypoints = new List<Node>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].z - path[i].z);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i - 1]);
            }
            directionOld = directionNew;
        }
        return waypoints;
    }

    private void ReversePath(ref List<Node> path)
    {
        path.Reverse();
    }

    //! As Vector3[]

    public Vector3[] FindPathAsWorldPos(Node startNode, Node targetNode, ref NodeGrid grid, bool isPathSimplified = false, bool isPathFromStartToTarget = true)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        bool pathSuccess = false;

        //Node startNode = grid.NodeFromWorldPosition(startPos);
        //Node targetNode = grid.NodeFromWorldPosition(targetPos);

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxHeapSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.Remove();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    pathSuccess = true;
                    Debug.Log("Path found: " + sw.ElapsedMilliseconds + " ms");
                    break;
                }

                foreach (var neighbourNode in grid.GetNeighbours(currentNode))
                {
                    if (!neighbourNode.isWalkable || closedSet.Contains(neighbourNode))
                    {
                        continue;
                    }
                    //count distance for each neighbour at grid generation => create set/array/list of neighbours?
                    int movementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbourNode) + neighbourNode.movementPenalty;
                    bool isInOpenSet = openSet.Contains(neighbourNode);

                    if (movementCostToNeighbour < neighbourNode.gCost || !isInOpenSet)
                    {
                        neighbourNode.gCost = movementCostToNeighbour;
                        neighbourNode.hCost = GetDistance(neighbourNode, targetNode);
                        neighbourNode.parent = currentNode;

                        if (!isInOpenSet)
                        {
                            openSet.Add(neighbourNode);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbourNode);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            List<Node> path = RetracePath(startNode, targetNode);
            Vector3[] waypoints;

            if (isPathSimplified)
            {
                waypoints = SimplifyPath(path);
                pathSuccess = waypoints.Length > 0;
            }
            else
            {
                waypoints = new Vector3[path.Count];
                for (int i = 0; i < path.Count; i++)
                {
                    waypoints[i].x = path[i].x;
                    waypoints[i].z = path[i].z;
                }
            }

            if (isPathFromStartToTarget)
            {
                ReversePath(ref waypoints);
            }

            if (pathSuccess)
            {
                return waypoints;
            }
        }

        return null;
    }

    private void ReversePath(ref Vector3[] path)
    {
        System.Array.Reverse(path);
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].x - path[i].x, path[i - 1].z - path[i].z);
            if (directionNew != directionOld)
            {
                waypoints.Add(new Vector3(path[i - 1].x, 0.0f, path[i - 1].z));
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    //! Manhattan distance
    private int GetDistance(Node a, Node b)
    {
        int distanceX = Mathf.Abs(a.x - b.x);
        int distanceZ = Mathf.Abs(a.z - b.z);

        return distanceX > distanceZ ? 4 * distanceZ + 10 * distanceX : 4 * distanceX + 10 * distanceZ;

        //if (distanceX > distanceZ)
        //{
        //    //return 14 * distanceZ + 10 * (distanceX - distanceZ);
        //    // (10 + 4) * z + 10 * (x - z) = 10z + 4z + 10x - 10z
        //    return 4 * distanceZ + 10 * distanceX;
        //}
        //else
        //{
        //    // 10x + 4x + 10z - 10x
        //    //return 14 * distanceX + 10 * (distanceZ - distanceX);
        //    return 4 * distanceX + 10 * distanceZ;
        //}
    }
}
