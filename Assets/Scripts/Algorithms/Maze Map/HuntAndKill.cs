using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntAndKill {

    public static T CreateMaze<T> (T grid) where T : MazeGrid
    {
        MazeCell currentCell = grid.GetRandomCell();

        while (currentCell != null)
        {
            // Random Walk
            List<MazeCell> unvisitedNeighbours = currentCell.Neighbours.FindAll(c => c.Links().Count == 0); //Unvisited neighbours of current cell

            if (unvisitedNeighbours.Count != 0) // if has unvisited - link to current cell and set neighbour as new current cell
            {
                MazeCell neighbour = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                currentCell.Link(neighbour);
                currentCell = neighbour;
            }
            else
            {
                currentCell = null;

                foreach (MazeCell cell in grid.EachCell())
                {
                    /*
                    List<MazeCell> visitedNeighbours = cell.Neighbours.FindAll(delegate(MazeCell obj) {
                        if (obj.Links().Count != 0)
                        {
                            return true;
                        }
                        else
                        {
                            false;
                        }
                    });
                    */
                    List<MazeCell> visitedNeighbours = cell.Neighbours.FindAll(c => c.Links().Count != 0); //was already visited
                        
                    if (cell.Links().Count == 0 && visitedNeighbours.Count != 0) //unvisited cell with one visited neighbour
                    {
                        currentCell = cell;
                        MazeCell neighbour = visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
                        currentCell.Link(neighbour); //connect unvisited cell to one of the availible visited neighbours
                        break;
                    }
                }
            }
        }

        return grid;
    }

}
