using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainHeightLayersGenerator {

    public static EmptyGrid SeparateTerrainOnLayers(EmptyGrid map, TileMapSettings settings, HeightMap heightMap, TerrainHeightLayers heightLayers)
    {
        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                float heightPercent = Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, z]);

                for (int i = 0; i < heightLayers.layers.Length; i++)
                {
                    if (heightPercent <= heightLayers.layers[i].startingHeight)
                    {
                        map.values[x, z] = Cell.CreateCell(heightLayers.layers[i].cellType);
                        heightMap.values[x, z] = heightLayers.layers[i].startingHeight;
                        break;
                    }
                }
            }
        }
        return map;
    }

    public static EmptyGrid SeparateTerrainOnLayers(ref EmptyGrid map, ref TileMapSettings settings, ref HeightMap heightMap, ref TerrainHeightLayers heightLayers)
    {
        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                float heightPercent = Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, z]);

                for (int i = 0; i < heightLayers.layers.Length; i++)
                {
                    if (heightPercent <= heightLayers.layers[i].startingHeight)
                    {
                        map.values[x, z] = Cell.CreateCell(heightLayers.layers[i].cellType);
                        heightMap.values[x, z] = heightLayers.layers[i].startingHeight;
                        break;
                    }
                }
            }
        }
        return map;
    }

}
