using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid
{
    public Node[,] values;
    public int width, height;

    private int penaltyMin = int.MaxValue;
    private int penaltyMax = int.MaxValue;

    public int MaxHeapSize
    {
        get
        {
            return width * height;
        }
    }

    public NodeGrid(ref EmptyGrid map)
    {
        values = new Node[map.width, map.height];
        CreateNodeMapFromEmptyGrid(ref map);
        width = map.width;
        height = map.height;
    }

    public void CreateNodeMapFromEmptyGrid(ref EmptyGrid map)
    {
        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                //Debug.Log(map.values[x, z].cellType + " " + x + " " + z);
                values[x, z] = Node.CreateNode(map.values[x, z].cellType, x, z);
                //Debug.Log(values[x, z] + " " + x + " " + z);
            }
        }     
    }

    private void BlurPenaltyMap(int blurKernelSize = 3)
    {
        int kernelSize = blurKernelSize * 2 + 1; //width/height of kernel
        int kernelExtents = (kernelSize - 1) / 2; //how many squares between center square and edge square

        int[,] penaltiesHorizontalPass = new int[width, height];
        int[,] penaltiesVerticalPass = new int[width, height];

        for (int y = 0; y < height; y++)
        {
            //first node in each row
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents); // use values from grid[x, 0] for each square that are outside of grid
                penaltiesHorizontalPass[0, y] += values[sampleX, y].movementPenalty;
            }

            for (int x = 1; x < width; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, width); //left-most value, that now are not inside the kernel (because this value are already in penalties grid)
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, width - 1);    //right-most value, new value of a kernel

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - values[removeIndex, y].movementPenalty + values[addIndex, y].movementPenalty;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            values[x, 0].movementPenalty = blurredPenalty;

            if (blurredPenalty > penaltyMax)
            {
                penaltyMax = blurredPenalty;
            }
            if (blurredPenalty < penaltyMin)
            {
                penaltyMin = blurredPenalty;
            }

            for (int y = 1; y < height; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, height);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, height - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];

                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                values[x, y].movementPenalty = blurredPenalty;

                if (blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node, bool hasDiagonalMovement = true)
    {
        List<Node> neighbours = new List<Node>();

        for (int z = -1; z <= 1; z++)
        {
            int checkZ = node.z + z;           
            for (int x = -1; x <= 1; x++)
            {
                //without diagonal if(x != 0 && y != 0) continue;
                if (z == 0 && x == 0 || (!hasDiagonalMovement && x != 0 && z != 0))
                {
                    continue;
                }

                int checkX = node.x + x;

                if (checkX >= 0 && checkX < width && checkZ >= 0 && checkZ < height)
                {
                    neighbours.Add(values[checkX, checkZ]);
                }
            }
        }
        return neighbours;
    }

    public bool IsOutside(int x, int y)
    {
        return (x < 0 || x >= width || y < 0 || y >= height);
    }
}
