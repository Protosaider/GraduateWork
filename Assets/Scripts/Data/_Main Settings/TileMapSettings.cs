using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Tile Map Settings")]
public class TileMapSettings : UpdatableData
{
    [Header("Map settings")]
    public bool isSquareShape;
    public int mapWidth;
    public int mapHeight;
    [Space]
    public bool createOuterWalls;
    [Header("Rendering mode")]
    //public bool usePrefabs;
    public bool isVoxelMap;
    public bool isDiscrete;
    public bool isCentered;
    public bool showWallsOnly;
    [Header("Render settings")]
    public float mapOffsetX;
    public float mapOffsetZ;
    public float cellScale;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
