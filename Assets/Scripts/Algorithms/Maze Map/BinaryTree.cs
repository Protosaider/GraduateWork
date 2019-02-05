using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTree {

    public static T CreateMaze<T>(T grid, MazeBinaryTreeSettings settings) where T : MazeGrid
    {
        // Random Generator
        Random.State initialState = Random.state;
        if (settings.useFixedSeed)
        {
            Random.InitState(settings.seed.GetHashCode());
        }
        else
        {
            Random.InitState(Time.time.ToString().GetHashCode());
        }

        foreach (var cell in grid.EachCell())
        {
            var neighbours = new List<MazeCell>();

            if (settings.verticalCarveDirectionNorth)
            {
                if (cell.North != null)
                {
                    neighbours.Add(cell.North);
                }
            }
            else
            {
                if (cell.South != null)
                {
                    neighbours.Add(cell.South);
                }
            }

            if (settings.horizontalCarveDirectionEast)
            {
                if (cell.East != null)
                {
                    neighbours.Add(cell.East);
                }
            }
            else
            {
                if (cell.West != null)
                {
                    neighbours.Add(cell.West);
                }
            }

            if (neighbours.Count > 0)
            {
                cell.Link(neighbours[Random.Range(0, neighbours.Count)]);
            }   
 
        }

        Random.state = initialState;

        return grid;
    }

}
