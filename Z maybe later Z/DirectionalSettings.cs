using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Directional Settings")]
public class DirectionalSettings : UpdatableData {

    [SerializeField]
    public bool useFixedSeed;
    public string seed;

    [SerializeField]
    public int mapWidth = 80;
    [SerializeField]
    public int mapHeight = 40;

    [Space]
    [SerializeField]
    public bool directionHorizontal = false;
    [SerializeField]
    public bool canGoBackwards = false;
    [SerializeField]
    [Tooltip("How long the cave shold be (in tiles)")]
    public int pathLength = 30;

    [Space]
    [SerializeField]
    [Tooltip("Chance to change width on each cells line (roughness).")]
    [Range(0, 100)]
    public int changeCellLineWidthChance = 20;
    [SerializeField]
    public bool useAutoLineWidthRange;
    [SerializeField]
    [Tooltip("How much the cave varies in width. (in tiles)")]
    public int lineWidthRange = 3;

    [Space]
    [SerializeField]
    [Tooltip("How much a path through it needs to 'wind' and 'swerve'")]
    [Range(0, 100)]
    public int chanceToChangePositioning = 20;
    [SerializeField]
    public bool useAutoPositioningRange;
    [SerializeField]
    [Tooltip("How much the cave varies in positioning. (in tiles)")]
    public int positioningRange = 2;

    [Space]
    [Header("How much the cave can vary in width. (in tiles)")]
    [SerializeField]
    public int minWidth = 5;
    [SerializeField]
    public int maxWidth = 12;
    [SerializeField]
    public bool useDefaultWidthRange;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
