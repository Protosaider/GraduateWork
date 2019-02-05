using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Maze Settings")]
public class MazeSettings : UpdatableData {

    public bool useFixedSeed = false;
    public string seed = "seed";

    public int gridWidth = 10;
    public int gridHeight = 10;

    public bool useDistanceGrid = true;
    public Coordinate initialCell;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
