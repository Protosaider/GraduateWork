using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Diffusion Limited Aggregation Settings")]
public class DiffusionLimitedAggregationSettings : UpdatableData {

    public bool useFixedSeed = false;
    public string seed = "seed";

    public int aliveWalkersAmount = 5;

    public int cellsToCreateCount = 10;
    [Space]
    [SerializeField]
    public bool allowMovementDiagonal = false;
    [SerializeField]
    public bool allowMovementNSWE = true;

    public bool allowStickDiagonal = false;
    public bool allowStickNSWE = true;
    [SerializeField]
    [Range(0.1f, 1.0f)]
    public float chanceToStick = 1.0f;

    [Space]
    [SerializeField]
    public int maxStepsAmount = 100;
    public int maxStuckCount = 10;
    public int overallStuckLimit = 100;

    public bool usePredefinedSpawnPoints;
    public bool isAggregatorAtCenter;

    [Space]
    public bool spawnWalkersAlongNorthWall;
    public bool spawnWalkersAlongSouthWall;
    public bool spawnWalkersAlongEastWall;
    public bool spawnWalkersAlongWestWall;
    public bool spawnWalkersAtCenter;

    [Space]
    public bool spawnAggregatorsAlongNorthWall;
    public bool spawnAggregatorsAlongSouthWall;
    public bool spawnAggregatorsAlongEastWall;
    public bool spawnAggregatorsAlongWestWall;
    public bool spawnAggregatorsAtCenter;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
