using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MazeGenerator {

    public static MazeGrid GenerateMazeGrid(MazeSettings settings)
    {
        if (settings.useDistanceGrid)
        {
            return new DistanceMazeGrid(settings.gridWidth, settings.gridHeight);
        }
        else
        {
            return new MazeGrid(settings.gridWidth, settings.gridHeight);
        }
        ///return new MazeGrid(settings.gridWidth, settings.gridHeight);
    }

    public static EmptyGrid ConvertToEmptyGrid(MazeGrid maze, CellType mazeFloor = CellType.Floor, CellType mazeWall = CellType.Wall)
    {
        return maze.ConvertToEmptyGrid(mazeFloor, mazeWall);
    }
}
