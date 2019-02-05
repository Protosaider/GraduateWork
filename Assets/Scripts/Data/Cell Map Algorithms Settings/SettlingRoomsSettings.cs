using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SettlingRoomsSettings : UpdatableData
{
    public string seed = "seed";
    public bool useFixedSeed = false;
    public bool isDebugLogEnabled = false;
    [Space]
    public float roomMinWidth = 4;
    public float roomMinHeight = 4;
    public float roomMaxWidth = 8;
    public float roomMaxHeight = 8;
    [Space]
    public int numberOfRoomsToSpawn = 40;
    public float roomSpawnCircleRadius = 3;
    [Space]
    public float roomWidthMeanConversionToMainRoomThreshold = 2;
    public float roomHeightMeanConversionToMainRoomThreshold = 2;
    [Space]
    [Range(0f, 1f)]
    public float percentsOfPathsAmountToAddFromDelaunayGraphToMST = 0.1f;
    [Space]
    [Range(0f, 1f)]
    public float chanceToAddPathRooms = 0.1f;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
