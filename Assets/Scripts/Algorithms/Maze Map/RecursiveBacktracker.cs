using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursiveBacktracker {
    // Recursive backtracker is a depth-first search. But only the difference is that neighbour chooses randomly.
    public static T CreateMaze<T> (T grid, MazeCell startCell = null) where T : MazeGrid
    {
        Stack<MazeCell> stack = new Stack<MazeCell>();
        if (startCell == null)
        {
            startCell = grid.GetRandomCell();
        }
        stack.Push(startCell); //first cell

        while (stack.Count != 0) //while have unvisited cells
        {
            MazeCell currentCell = stack.Peek();
            List<MazeCell> neighbours = currentCell.Neighbours.FindAll(c => c.Links().Count == 0); //check if current cell has unvisited neighbours

            if (neighbours.Count == 0) //if has no unvisited neighbours
            {
                stack.Pop();    //pop that cell (remove from stack) - it makes previous cell in the stack the current node
            }
            else //else link neighbour to cur.cell and push in stack
            {
                MazeCell neighbour = neighbours[Random.Range(0, neighbours.Count)]; // we can add bias => it's more possibly to choose especial neighbour (on North, for example)
                currentCell.Link(neighbour);
                stack.Push(neighbour);
            }
        }
        return grid;
    }
}
