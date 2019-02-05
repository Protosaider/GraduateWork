using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Room Grid Settings")]
public class RoomGridSettings : UpdatableData
{
    public bool useFixedSeed;
    public string seed = "seed";

    public int gridWidth;
    public int gridHeight;

    public bool isSquareShaped;

    public int roomWidth;
    public int roomHeight;

    public int roomWidthCenter;
    public int roomHeightCenter;

    public float doorSpawnChance;

    public int roomCount;

    public bool isSpawnPointCentered;
    public Coordinate initialRoomSpawnPoint;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
