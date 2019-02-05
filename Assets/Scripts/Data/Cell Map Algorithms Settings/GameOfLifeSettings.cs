using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Game Of Life Settings")]
public class GameOfLifeSettings : UpdatableData {

    [SerializeField]
    public bool useFixedSeed = false;
    [SerializeField]
    //public int seed;
    public string seed;
    [Space]
    [SerializeField]
    [Header("Generator parameters")]
    [Tooltip("Sets the chance to have a wall in a cell on initial map filling.")]
    [Range(0.0f, 1.0f)]
    public float randomFillPercent = 0.45f;

    [SerializeField]
    [Tooltip("Number of evolution waves = how many times map will be smoothed.")]
    [Range(0, 10)]
    public int smoothCount = 3;

    [SerializeField]
    public bool useDoubleLayerGeneration = false;

    [SerializeField]
    [Tooltip("Parameters for smooth algorithm. If destroyWallLimit less then amount of walls around, then current wall will be destroyed. Max neighbours count = 8.")]
    [Range(0, 8)]
    public int destroyWallLimit = 5;
    [SerializeField]
    [Tooltip("Parameters for smooth algorithm. If amount of walls around more then createWallLimit, then we can create a wall. Max neighbours count = 8.")]
    [Range(0, 8)]
    public int createWallLimit = 4;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
