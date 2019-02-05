using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscreteTerrainGenerator {

    public static Mesh GenerateDiscreteTerrainMesh(DiscreteTerrainSettings terrainSettings)
    {
        EmptyGrid cellMap = GridGenerator.GenerateEmptyGrid(terrainSettings.tileMapSettings);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(terrainSettings.heightMapSettings, new Vector2(0, 0), terrainSettings.tileMapSettings.mapWidth, terrainSettings.tileMapSettings.mapHeight);

        if (terrainSettings.useHeightLayers)
        {
            TerrainHeightLayersGenerator.SeparateTerrainOnLayers(ref cellMap, ref terrainSettings.tileMapSettings, ref heightMap, ref terrainSettings.heightLayersSettings);
        }

        switch (terrainSettings.terrainMeshType)
        {
            case GridType.Square:
                return DiscreteVoxelMeshGenerator.GenerateTerrainMesh(cellMap, terrainSettings.tileMapSettings, heightMap, terrainSettings.heightLayersSettings);
            case GridType.PointyHex:
                return DiscreteSolidPointyHexMeshGenerator.GenerateTerrainMesh(cellMap, terrainSettings.tileMapSettings, heightMap, terrainSettings.heightLayersSettings);
            default:
                Debug.LogError("Terrain underlying grid type hasn't set up.");
                break;
        }

        return new Mesh();
    }
}
