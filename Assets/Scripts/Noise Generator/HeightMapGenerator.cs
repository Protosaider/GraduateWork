using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoiseGeneratorType
{
    fBmNoiseMap,
    octavesNoiseMap,
    boxFilteredNoiseMap,
    uniformNoiseMap,
    //flattenUniformNoiseMap,
    //flattenNoiseMap,
}

public static class HeightMapGenerator {

    static float[,] falloffMap;

    //public static HeightMap GenerateHeightMap(HeightMapSettings settings, Vector2 sampleCenter, int mapWidth, int mapHeight = -1, NoiseGeneratorType type = NoiseGeneratorType.fBmNoiseMap)
    public static HeightMap GenerateHeightMap(HeightMapSettings settings, Vector2 sampleCenter, int mapWidth, int mapHeight = -1)
    {
        if (mapHeight == -1)
        {
            mapHeight = mapWidth;
        }

        float[,] values;

        //switch (type)
        switch (settings.applyingNoiseType)
        {
            case NoiseGeneratorType.fBmNoiseMap:
                values = Noise.GenerateNoiseMap(mapWidth, mapHeight, settings.noiseSettings, sampleCenter);
                break;
            case NoiseGeneratorType.octavesNoiseMap:
                values = Noise.GenerateNoiseMap(mapWidth, mapHeight, settings.octavesSettings, sampleCenter);
                break;
            case NoiseGeneratorType.boxFilteredNoiseMap:
                values = Noise.GenerateBoxLinearFilteredNoiseMap(mapWidth, mapHeight, settings.boxFilteredSettings);
                break;
            case NoiseGeneratorType.uniformNoiseMap:
                //values = Noise.GenerateUniformNoiseMap(mapWidth, mapHeight, mapWidth * 0.5f, mapWidth * 0.5f, sampleCenter.y);
                values = Noise.GenerateUniformNoiseMap(mapWidth, mapHeight, mapWidth * 0.5f, mapWidth * 0.5f, sampleCenter.y);
                break;
            default:
                Debug.LogError("Noise generetion type doesn't set. Use fractal Brounian moution by default.");
                values = Noise.GenerateNoiseMap(mapWidth, mapHeight, settings.noiseSettings, sampleCenter);
                break;
        }

        if (settings.applyFalloffMap)
        {
            if (falloffMap == null)
            {
                falloffMap = FalloffMapGenerator.GenerateFalloffMap(mapWidth, mapHeight);
            }
        }

        AnimationCurve heightCurveThreadsafe = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        if (settings.applyFalloffMap)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    values[i, j] *= heightCurveThreadsafe.Evaluate(values[i, j] - falloffMap[i, j]) * settings.heightMultiplier;

                    if (values[i, j] < minValue)
                    {
                        minValue = values[i, j];
                    }

                    if (values[i, j] > maxValue)
                    {
                        maxValue = values[i, j];
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    values[i, j] *= heightCurveThreadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                    if (values[i, j] < minValue)
                    {
                        minValue = values[i, j];
                    }

                    if (values[i, j] > maxValue)
                    {
                        maxValue = values[i, j];
                    }
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }

    public static HeightMapFlatten ConvertToFlatten(HeightMap heightMap)
    {
        return (HeightMapFlatten)heightMap;
    }

    //public static HeightMapFlatten GenerateFlattenHeightMap(HeightMapSettings settings, Vector2 sampleCenter, int mapWidth, int mapHeight = -1, NoiseGeneratorType type = NoiseGeneratorType.fBmNoiseMap)
    //{
    //}
}

[System.Serializable]
public struct HeightMap
{
    public readonly float[,] values;
    //public float[,] values;
    public readonly int width;
    public readonly int height;
    public readonly float minValue;
    public readonly float maxValue;

    //public static HeightMap Null = new HeightMap(new float[0, 0] { }, -1, -1, -1, -1);

    public HeightMap(float[,] heightMapValues, float minValue, float maxValue)
    {
        this.values = heightMapValues;
        this.width = heightMapValues.GetLength(0);
        this.height = heightMapValues.GetLength(1);
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public HeightMap(float[,] heightMapValues, int width, int height, float minValue, float maxValue)
    {
        this.values = heightMapValues;
        this.width = width;
        this.height = height;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public float[] ConvertToFlattenArray()
    {
        float[] flattenValues = new float[values.GetLength(1) * values.GetLength(0)];
        for (int y = 0; y < values.GetLength(1); y++)
        {
            for (int x = 0; x < values.GetLength(0); x++)
            {
                flattenValues[y * values.GetLength(0) + x] = values[x, y];
            }
        }
        return flattenValues;
    }

    public IEnumerator<float> EachValue()
    {
        foreach (var item in values)
        {
            yield return item;
        }
    }

    public float GetNeighbour(int x, int z, Direction direction, GridType mode = GridType.Square)
    {
        int y = 0;
        Coordinate offsetToCheck;

        if (mode == GridType.Square)
        {
            offsetToCheck = Coordinate.GetOffset(direction);
        }
        else
        {
            offsetToCheck = Coordinate.GetOffset(direction) + Coordinate.GetPointyHexOffset(direction, z % 2 == 0);
        }

        Coordinate neighbourCoord = new Coordinate(x + offsetToCheck.x, y + offsetToCheck.y, z + offsetToCheck.z);

        if ((neighbourCoord.x < 0 || neighbourCoord.x >= width) || (neighbourCoord.y != 0) || (neighbourCoord.z < 0 || neighbourCoord.z >= height))
        {
            return float.MinValue;
        }

        return values[neighbourCoord.x, neighbourCoord.z];
    }

    public static explicit operator HeightMapFlatten(HeightMap heightMap)
    {
        return new HeightMapFlatten(heightMap.values, heightMap.minValue, heightMap.maxValue);
    }

    //public static explicit operator HeightMap(HeightMapFlatten v)
    //{
    //    return new ;
    //}
}

[System.Serializable]
public struct HeightMapFlatten
{
    public readonly float[] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMapFlatten(float[] heightMapValues, float minValue, float maxValue)
    {
        this.values = heightMapValues;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public HeightMapFlatten(float[,] heightMapValues, float minValue, float maxValue)
    {
        this.values = new float[heightMapValues.GetLength(1) * heightMapValues.GetLength(0)];
        for (int y = 0; y < this.values.GetLength(1); y++)
        {
            for (int x = 0; x < this.values.GetLength(0); x++)
            {
                values[y * this.values.GetLength(0) + x] = heightMapValues[x, y];
            }
        }

        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    public HeightMapFlatten(HeightMap heightMap)
    {
        this.values = new float[heightMap.width * heightMap.height];
        for (int y = 0; y < heightMap.height; y++)
        {
            for (int x = 0; x < heightMap.width; x++)
            {
                values[y * heightMap.width + x] = heightMap.values[x, y];
            }
        }

        this.minValue = heightMap.minValue;
        this.maxValue = heightMap.maxValue;
    }

    public IEnumerator<float> EachValue()
    {
        foreach (var item in values)
        {
            yield return item;
        }
    }
}