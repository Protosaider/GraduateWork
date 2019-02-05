using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Random Worms Settings")]
public class RandomWormsSettings : UpdatableData {

    [SerializeField]
    public bool useFixedSeed;
    public string seed;
    [Space]
    public float chanceToCreateRectangularRoom;
    
    [Space]
    [SerializeField]
    public bool allowTurnAround = false;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    public float chanceToTurnAround = 0.08f;
    [Range(0.0f, 1.0f)]
    [Tooltip("Chance to make a 90 degrees turn in clockwise or counterclockwise order (default chances are 50:50 percent).")]
    [SerializeField]
    public float chanceToTurn = 0.14f;
    //public float chanceToTurnClockwise = 0.5f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    public float chanceToCreateWorm = 0.0f;
    [Range(0.0f, 1.0f)]
    [Tooltip("This modifier is added to die chance for each worm that are alive (only if worms count are more then 2).")]
    [SerializeField]
    public float increaseDieChanceBy = 0.15f;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    [Tooltip("Full chance formula is: chanceToDie = initialChanceToDie + (wormsCount - 1) * increaseDieChanceBy")]
    public float initialChanceToDie = 0.0f;
    [SerializeField]
    public int cellsToCreateCount = 20;
    [SerializeField]
    [Range(1, 20)]
    public int wormsCount = 1;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
