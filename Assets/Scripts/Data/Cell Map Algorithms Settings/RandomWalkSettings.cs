using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Random Walk Settings")]
public class RandomWalkSettings : UpdatableData {

    public bool useFixedSeed = false;
    public string seed;
    [SerializeField]
    public bool allowMovementDiagonal = false;
    [SerializeField]
    public bool allowMovementNSWE = true;
    [Space]
    [SerializeField]
    public int stepsAmount = 1000;
    [SerializeField]
    public int stuckLimit = 8;
    [SerializeField]
    public int pathLength = 3;

    public bool canLeviFlight = false;
    public bool connectJumpPointsInLeviFlight = false;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
