using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Binary Space Partitioning Settings")]
public class BinarySpacePartitioningSettings : UpdatableData
{
    public bool useFixedSeed = false;
    public string seed = "seed";

    [Range(0.5f, 3.0f)]
    public float widthHeightRatioToSplitNotRandom = 1.25f;
    [Range(1.0f, 3.0f)]
    public float minSpaceSizeToSplitMiltiplier = 1.15f;
    [Range(0.0f, 1.0f)]
    public float chanceToStopSpaceSplitting = 0.125f;

    public int roomsMinHeight = 3;
    public int minSplitSizeHorizontal = 3;
    public int minSpaceSizeHorizontal = 5;

    public bool useOnlyRecursiveDivision = false;

    public int roomsMinWidth = 3;
    public int minSplitSizeVertical = 3;
    //minSplitSize * 2
    public int minSpaceSizeVertical = 5;

    public bool useRoomsMaxSizeValues;
    public int roomsMaxHeight = 8;
    public int roomsMaxWidth = 8;

    [Tooltip("Is overlapping check ignore walls (room's wall can overlap other room's wall).")]
    public bool canShareSingleWall;
    [Tooltip("Remove shared walls tiles if they are surrounded only with floor tiles.")]
    public bool hasCleanup;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
