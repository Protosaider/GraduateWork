using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelMeshData {

    public static Coordinate[] offsets =
{
        new Coordinate(0, 0, 1),    //North PositiveZ
        new Coordinate(1, 0, 0),    //East  PositiveX
        new Coordinate(0, 0, -1),   //South NegativeZ
        new Coordinate(-1, 0, 0),   //West  NegativeX
        new Coordinate(0, 1, 0),    //Up    PositiveY
        new Coordinate(0, -1, 0),   //Down  NegativeY
    };
    // from center
    public static Vector3[] vertices =
    {
        new Vector3( 1.0f,  1.0f, 1.0f),    //0 right     top     back
        new Vector3(-1.0f,  1.0f, 1.0f),    //1 left      top     back
        new Vector3(-1.0f, -1.0f, 1.0f),    //2 left      bottom  back
        new Vector3( 1.0f, -1.0f, 1.0f),    //3 right     bottom  back

        new Vector3(-1.0f,  1.0f, -1.0f),   //4 left      top     forward
        new Vector3( 1.0f,  1.0f, -1.0f),   //5 right     top     forward
        new Vector3( 1.0f, -1.0f, -1.0f),   //6 right     bottom  forward
        new Vector3(-1.0f, -1.0f, -1.0f),   //7 left      bottom  forward
    };

    public static int[][] faceTriangles =
    {
        //North
        new int[]
        {
            0, 1, 2, 3, //triangles are: 012, 023
        },

        //East
        new int[]
        {
            5, 0, 3, 6,
        },

        //South
        new int[]
        {
            4, 5, 6, 7,
        },

        //West
        new int[]
        {
            1, 4, 7, 2,
        },

        //Top
        new int[]
        {
            1, 0, 5, 4,
            //5, 4, 1, 0,
        },

        //Bottom
        new int[]
        {
            7, 6, 3, 2,
            //3, 2, 7, 6,
        },
    };

    //public static Vector3[] FaceVertices(int dir, float scale, Vector3 position)
    //{
    //    Vector3[] faceVertices = new Vector3[4];
    //    for (int i = 0; i < faceVertices.Length; i++)
    //    {
    //        faceVertices[i] = vertices[faceTriangles[dir][i]] * scale + position;
    //    }
    //    return faceVertices;
    //}

    //public static Vector3[] FaceVertices(Direction dir, float scale, Vector3 position)
    //{
    //    return FaceVertices((int)dir, scale, position);
    //}

    public static Vector3[] FaceVertices(Direction dir, float scale, Vector3 position)
    {
        Vector3[] faceVertices = new Vector3[4];
        for (int i = 0; i < faceVertices.Length; i++)
        {
            faceVertices[i] = vertices[faceTriangles[(int)dir][i]] * scale + position;
        }
        return faceVertices;
    }
}

