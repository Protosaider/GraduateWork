using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SolidPointyHexagonData
{
    // relative to the cell's center. Orientation - pointy side up.
    private static Vector3[] vertices =
    { // clockwise order
        new Vector3(0.0f, 1.0f, 0.0f),                                                  //0
        new Vector3(0f, 1f, PointyHexTileData.outerRadius),                                   //1
        new Vector3(PointyHexTileData.innerRadius, 1f, 0.5f * PointyHexTileData.outerRadius),       //2
        new Vector3(PointyHexTileData.innerRadius, 1f, -0.5f * PointyHexTileData.outerRadius),      //3
        new Vector3(0f, 1f, -PointyHexTileData.outerRadius),                                  //4
        new Vector3(-PointyHexTileData.innerRadius, 1f, -0.5f * PointyHexTileData.outerRadius),     //5
        new Vector3(-PointyHexTileData.innerRadius, 1f, 0.5f * PointyHexTileData.outerRadius),      //6

        new Vector3(0.0f, -1.0f, 0.0f),                                                 //7
        new Vector3(0f, -1f, PointyHexTileData.outerRadius),                                  //8
        new Vector3(PointyHexTileData.innerRadius, -1f, 0.5f * PointyHexTileData.outerRadius),      //9
        new Vector3(PointyHexTileData.innerRadius, -1f, -0.5f * PointyHexTileData.outerRadius),     //10
        new Vector3(0f, -1f, -PointyHexTileData.outerRadius),                                 //11
        new Vector3(-PointyHexTileData.innerRadius, -1f, -0.5f * PointyHexTileData.outerRadius),    //12
        new Vector3(-PointyHexTileData.innerRadius, -1f, 0.5f * PointyHexTileData.outerRadius),     //13
    };

    private static Dictionary<Direction, int[]> faceTriangles = new Dictionary<Direction, int[]>()
    {
        //{ Direction.NorthEast,  new int[] { 1, 2,  9,  8,  } },
        //{ Direction.East,       new int[] { 2, 3,  10, 9,  } },
        //{ Direction.EastSouth,  new int[] { 3, 4,  11, 10, } },
        //{ Direction.SouthWest,  new int[] { 4, 5,  12, 11, } },
        //{ Direction.West,       new int[] { 5, 6,  13, 12, } },
        //{ Direction.WestNorth,  new int[] { 6, 1,  8,  13, } },
        //{ Direction.Up,         new int[] { 0,  1, 2,  3,  4,  5,  6, } },
        //{ Direction.Down,       new int[] { 7,  8, 13, 12, 11, 10, 9, } },

        { Direction.NorthEast,  new int[] { 9,  2,  1,  8,  } },
        { Direction.East,       new int[] { 10, 3,  2,  9,  } },
        { Direction.EastSouth,  new int[] { 11, 4,  3,  10, } },
        { Direction.SouthWest,  new int[] { 12, 5,  4,  11, } },
        { Direction.West,       new int[] { 13, 6,  5,  12, } },
        { Direction.WestNorth,  new int[] { 8,  1,  6,  13, } },
        { Direction.Up,         new int[] { 0,  1,  2,  3,  4,  5,  6, } },
        { Direction.Down,       new int[] { 7,  8,  13, 12, 11, 10, 9, } },
    };

    private static int[][] faceTrianglesArray =
    {
        //NorthEast
        new int[]
        {
            9, 2, 1, 8,
        },

        //East
        new int[]
        {
            10, 3, 2, 9,
        },

        //EastSouth
        new int[]
        {
            11, 4, 3, 10,
        },

        //SouthWest
        new int[]
        {
            12, 5, 4, 11,
        },

        //West
        new int[]
        {
            13, 6, 5, 12,
        },

        //WestNorth
        new int[]
        {
            8, 1, 6, 13,
        },

        //Top
        new int[]
        {
            0, 1, 2, 3, 4, 5, 6, //1
        },

        //Bottom
        new int[]
        {
            7, 8, 13, 12, 11, 10, 9, //8
        },
    };

    private static Direction[] possibleDirections =
    {
        Direction.NorthEast,
        Direction.East,
        Direction.EastSouth,
        Direction.SouthWest,
        Direction.West,
        Direction.WestNorth,
        Direction.Up,
        Direction.Down,
    };

    public static Vector3[] FaceVertices(Direction dir, float scale, Vector3 position, out int verticesCount)
    {
        verticesCount = faceTriangles[dir].Length;

        Vector3[] faceVertices = new Vector3[verticesCount];

        for (int i = 0; i < verticesCount; i++)
        {
            faceVertices[i] = vertices[faceTriangles[dir][i]] * scale + position;
        }
        return faceVertices;
    }

    public static IEnumerable<Direction> PossibleDirections()
    {
        foreach (var direction in possibleDirections)
        {
            yield return direction;
        }
    }
}


