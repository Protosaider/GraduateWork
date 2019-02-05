using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMazeGrid : MazeGrid {

    public Distances distances
    {
        get;
        set;
    }

    public DistanceMazeGrid(int columns, int rows) : base(columns, rows) {  }

    public override string ContentsOfCell(MazeCell cell)
    {
        if (distances != null && distances.ContainsKey(cell))
        {
            return distances[cell].ToString();
        }
        else
        {
            return base.ContentsOfCell(cell);
        }
    }

    public int GetCellDistance(int x, int z)
    {
        if (IsOutside(x, z))
        {
            Debug.Log("GetCell: Out of bounds.");
        }

        MazeCell cell = GetCell(x, z);

        if (distances != null && distances.ContainsKey(cell))
        {
            return distances[cell];
        }
        else
        {
            return -1;
        }
    }
    
}