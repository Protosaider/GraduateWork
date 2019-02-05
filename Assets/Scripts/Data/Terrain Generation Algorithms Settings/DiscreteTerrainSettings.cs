using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DiscreteTerrainSettings : UpdatableData
{
    //public int terrainMaxWidth = 241; //in vertices
    //public int terrainMaxHeight = 241; //in vertices

    public TileMapSettings tileMapSettings;
    public HeightMapSettings heightMapSettings;
    public TerrainHeightLayers heightLayersSettings;

    public bool useHeightLayers = false;
    public GridType terrainMeshType = GridType.Square;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
