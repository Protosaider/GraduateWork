using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Maze Binary Tree Settings")]
public class MazeBinaryTreeSettings : UpdatableData {

    public bool useFixedSeed = false;
    public string seed = "seed";

    public bool verticalCarveDirectionNorth;
    public bool horizontalCarveDirectionEast;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
