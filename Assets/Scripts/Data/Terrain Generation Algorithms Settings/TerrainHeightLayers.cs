using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainHeightLayers : UpdatableData
{
    public TerrainHeightLayer[] layers;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}


[System.Serializable]
public class TerrainHeightLayer
{
    public CellType cellType = CellType.Empty;

    [Range(0, 1)]
    public float startingHeight = 0;
}
