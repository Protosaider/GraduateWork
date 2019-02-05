using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Fisher Yates Shuffle Settings")]
public class FisherYatesShuffleSettings : UpdatableData
{

    public bool useFixedSeed;
    public int seed;
    public float obstaclesFillingPercentage;
    public Coordinate spawnPoint;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
