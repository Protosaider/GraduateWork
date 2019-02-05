using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointyHexTileData  {

    public const float outerRadius = 1f;

    public const float innerRadius = outerRadius * 0.866025404f;

    // relative to the cell's center. Orientation - pointy side up.
    public static Vector3[] vertices = 
    { // clockwise order
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius), //Same as first
    };

    public static Vector2[] uv =
    { // clockwise order
        new Vector2(0.5f, 1f),
        new Vector2(1f, 0.75f),
        new Vector2(1f, 0.25f),
        new Vector2(0.5f, 0f),
        new Vector2(0f, 0.25f),
        new Vector2(0f, 0.75f),
        new Vector2(0.5f, 1f), //Same as first
    };

    // public static Vector3 GetFirstCorner(EHexDirection direction)
    // {
    //     return vertices[(int)direction];
    // }

    // public static Vector3 GetSecondCorner(EHexDirection direction)
    // {
    //     return vertices[(int)direction + 1];
    // }

    // // Retrieve the vertices of solid inner hexagons
    // public static Vector3 GetFirstSolidCorner(EHexDirection direction)
    // {
    //     return vertices[(int)direction] * solidFactor;
    // }

    // public static Vector3 GetSecondSolidCorner(EHexDirection direction)
    // {
    //     return vertices[(int)direction + 1] * solidFactor;
    // }

    // // the midpoint between the two relevant vertices, then applying the blend factor to that. 
    // public static Vector3 GetBridge(EHexDirection direction)
    // {
    //     return (vertices[(int)direction] + vertices[(int)direction + 1]) * blendFactor;
    // }
}
