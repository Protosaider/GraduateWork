using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ContiguousTerrainGenerator
{
    public static Mesh GenerateContiguousTerrainMesh(ContiguousTerrainSettings terrainSettings)
    {
        //HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(terrainSettings.heightMapSettings, new Vector2(0, 0), terrainSettings.tileMapSettings.mapWidth, terrainSettings.tileMapSettings.mapHeight);
        HeightMap heightMap;

        switch (terrainSettings.terrainMeshType)
        {
            case GridType.Square:
                heightMap = HeightMapGenerator.GenerateHeightMap(terrainSettings.heightMapSettings, new Vector2(0, 0), terrainSettings.tileMapSettings.mapWidth, terrainSettings.tileMapSettings.mapHeight);
                return ContiguousMeshGenerator.GenerateTerrainMesh(heightMap, terrainSettings);
            case GridType.PointyHex:
                heightMap = HeightMapGenerator.GenerateHeightMap(terrainSettings.heightMapSettings, new Vector2(0, 0), terrainSettings.tileMapSettings.mapWidth * 2 + (terrainSettings.tileMapSettings.mapHeight > 1 ? 2 : 1), terrainSettings.tileMapSettings.mapHeight * 3 + 2);
                return ContiguousPointyHexMeshGenerator.GenerateTerrainMesh(heightMap, terrainSettings);
            default:
                Debug.LogError("Terrain underlying grid type hasn't set up.");
                break;
        }
        return new Mesh();
    }

    public static HeightMap GenerateContiguousTerrainHeightMap(ContiguousTerrainSettings terrainSettings)
    {
        HeightMap heightMap;

        switch (terrainSettings.terrainMeshType)
        {
            case GridType.Square:
                return HeightMapGenerator.GenerateHeightMap(terrainSettings.heightMapSettings, new Vector2(0, 0), terrainSettings.tileMapSettings.mapWidth, terrainSettings.tileMapSettings.mapHeight);
            case GridType.PointyHex:
                return HeightMapGenerator.GenerateHeightMap(terrainSettings.heightMapSettings, new Vector2(0, 0), terrainSettings.tileMapSettings.mapWidth * 2 + (terrainSettings.tileMapSettings.mapHeight > 1 ? 2 : 1), terrainSettings.tileMapSettings.mapHeight * 3 + 2);
            default:
                Debug.LogError("Terrain underlying grid type hasn't set up.");
                break;
        }
        return new HeightMap();
    }

    public static Mesh GenerateContiguousTerrainFromHM(HeightMap heightMap, ContiguousTerrainSettings terrainSettings)
    {

        switch (terrainSettings.terrainMeshType)
        {
            case GridType.Square:
                return ContiguousMeshGenerator.GenerateTerrainMesh(heightMap, terrainSettings);
            case GridType.PointyHex:
                return ContiguousPointyHexMeshGenerator.GenerateTerrainMesh(heightMap, terrainSettings);
            default:
                Debug.LogError("Terrain underlying grid type hasn't set up.");
                break;
        }
        return new Mesh();
    }
}
