using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Maze Sidewinder Settings")]
public class MazeSidewinderSettings : UpdatableData {

    public bool useFixedSeed = false;
    public string seed = "seed";

    public bool isVerticalDirectionMain;
    public float chanceToCarveOutMain = 0.5f;

    public bool verticalCarveDirectionNorth;
    public bool horizontalCarveDirectionEast;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
