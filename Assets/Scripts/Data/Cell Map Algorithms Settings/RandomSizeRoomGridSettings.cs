using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Random Size Room Grid Settings")]
public class RandomSizeRoomGridSettings : UpdatableData {

    public bool useFixedSeed;
    public int seed;

    public int gridWidth;
    public int gridHeight;

    public int roomMaxWidth;
    public int roomMaxHeight;

    public int roomMinWidth;
    public int roomMinHeight;

    public int roomCount;

    public bool isSquareShaped;
    public bool randomizeRoomPositions;

    public bool isSpawnPointCentered;
    public Coordinate initialRoomSpawnPoint;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
