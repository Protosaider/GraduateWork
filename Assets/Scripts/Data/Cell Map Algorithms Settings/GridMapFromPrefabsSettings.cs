using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Grid Map From Prefabs Settings")]
public class GridMapFromPrefabsSettings : UpdatableData
{
    public bool useFixedSeed = false;
    public string seed = "seed";

    public int gridWidth = 10;
    public int gridHeight = 10;

    public bool hasRandomRooms;
    public int roomsCount;
    public int roomPlacementTriesCount = 5;

    public int roomMinWidth = 2;
    public int roomMinHeight = 2;

    public int roomMaxWidth = 5;
    public int roomMaxHeight = 5;

    [Range(1, 4)]
    public int maxRandomEntrances = 3;
    [Range(0, 1)]
    [Tooltip("Percentage that subtrahended from spawn door chance after door spawn.")]
    public float randomEntranceCreationChanceSubtrahend = 0.30f;
    [Range(0, 1)]
    public float randomEntranceCreationChance = 0.9f;

    [Range(0, 1)]
    public float procentOfTilesThatAreWalkable = 0.9f;

    //[Tooltip("Creates dictionary, where each character has exclusive prefab.")]
    //public List<GameObject> dungeonShapes;
    public string prefabCharacters;

    public int prefabWidthInTiles = 3;
    public int prefabHeightInTiles = 3;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
