using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Random Room Placement Maze Adder Settings")]
public class RandomRoomPlacementMazeAdderSettings : UpdatableData {

    public bool useFixedSeed = false;
    public string seed = "seed";

    public int roomsCount = 2;

    public int roomPlacementTriesCount = 5;

    public int roomMinHeight = 1;
    public int roomMinWidth = 1;

    public int roomMaxHeight = 4;
    public int roomMaxWidth = 4;

    [Range(1, 4)]
    public int maxRandomEntrances = 3;
    [Range(0, 1)]
    [Tooltip("Percentage that subtrahended from spawn door chance after door spawn.")]
    public float randomEntranceCreationChanceSubtrahend = 0.30f;
    [Range(0, 1)]
    public float randomEntranceCreationChance = 0.9f;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
