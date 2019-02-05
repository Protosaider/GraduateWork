using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Random Room Placement Settings")]
public class RandomRoomPlacementSettings : UpdatableData
{
    public bool useFixedSeed = false;
    public string seed = "seed";

    [SerializeField]
    [Range(1, 1000)]
    public int roomPlacementTriesCount = 15;
    [Space]
    public bool tryToGenerateSpecificAmountOfRooms;
    public int amountOfRoomsToGenerate;
    [Range(1, 100)]
    public int timesToRestartSpawn;
    [Space]
    [SerializeField]
    public int roomsMinHeight = 3;
    [SerializeField]
    public int roomsMinWidth = 3;
    [SerializeField]
    public int roomsMaxHeight = 12;
    [SerializeField]
    public int roomsMaxWidth = 20;
    [SerializeField]
    [Tooltip("Is overlapping check ignore walls (room's wall can overlap other room's wall).")]
    public bool canShareSingleWall;
    [Tooltip("Remove shared walls tiles if they are surrounded only with floor tiles.")]
    public bool hasCleanup;

    public bool isRoomsConnected;


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
