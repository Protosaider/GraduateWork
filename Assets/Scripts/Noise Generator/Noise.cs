using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public enum HeightNormalizeMode
    {
        Local,
        Global,
    }

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Random.State initialState = Random.state;

        if (settings.useFixedSeed)
        {
            Random.InitState(settings.seed.GetHashCode());
        }
        else
        {
            Random.InitState(Time.time.ToString().GetHashCode());
        }

        //System.Random randomNumGen = new System.Random(settings.mapSeed);

        Vector2[] octavesOffsets = new Vector2[settings.numberOfOctaves];

        float maxGlobalNoiseHeight = 0;
        float noiseAmplitude = 1.0f;
        float noiseFrequency = 1.0f;
        float noiseHeight = 0.0f;

        for (int i = 0; i < settings.numberOfOctaves; i++)
        {
            float offsetX = + settings.octavesOffset.x + sampleCenter.x;
            float offsetY = - settings.octavesOffset.y - sampleCenter.y;

            if (settings.isEachOctaveHasRandomShift)
            {
                offsetX += Random.Range(-10000, 10001);
                offsetY += Random.Range(-10000, 10001);
                //offsetX += randomNumGen.Next(-100000, 100000);
                //offsetY += randomNumGen.Next(-100000, 100000);
            }
            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxGlobalNoiseHeight += noiseAmplitude;
            noiseAmplitude *= settings.persistance;
        }

        if (settings.scale <= 0)
        {
            settings.scale = 0.001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Allows us to scale to center
        float halfMapWidth = mapWidth * 0.5f;
        float halfMapHeight = mapHeight * 0.5f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseAmplitude = 1.0f;
                noiseFrequency = 1.0f;
                noiseHeight = 0.0f;

                for (int i = 0; i < settings.numberOfOctaves; i++)
                {
                    // The higher the frequency, the further apart the sample points will be (height values will change more rapidly)
                    float xSample = (x - halfMapWidth + octavesOffsets[i].x) / settings.scale * noiseFrequency;
                    float ySample = (y - halfMapHeight + octavesOffsets[i].y) / settings.scale * noiseFrequency;

                    //between 0.0 and 1.0
                    float perlinNoiseValue;

                    if (settings.enablePolarCoordinatesMode)
                    {
                        float theta = Mathf.PerlinNoise(xSample, ySample) * settings.polarScale;
                        perlinNoiseValue = Mathf.PerlinNoise(xSample + settings.polarRadius * Mathf.Cos(theta), ySample + settings.polarRadius * Mathf.Sin(theta)) * 2 - 1;
                    }
                    else
                    { // between 0.0 and 1.0 => between -1 and 1
                        perlinNoiseValue = Mathf.PerlinNoise(xSample, ySample) * 2 - 1;
                    }

                    noiseHeight += perlinNoiseValue * noiseAmplitude;
                    noiseAmplitude *= settings.persistance;
                    noiseFrequency *= settings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == HeightNormalizeMode.Global)
                {
                    //float noiseNormalizedHeight = Mathf.InverseLerp(-maxGlobalNoiseHeight, maxGlobalNoiseHeight, noiseMap[x, y]);
                    float noiseNormalizedHeight = (noiseMap[x, y] + 1) / (maxGlobalNoiseHeight * 1.1112f);                  
                    noiseMap[x, y] = Mathf.Clamp(noiseNormalizedHeight, 0, int.MaxValue);
                    //noiseMap[x, y] = Mathf.Clamp(noiseNormalizedHeight, -1.0f, 1.0f);

                    if (settings.enableAbsMod)
                    {
                        noiseMap[x, y] = Mathf.Abs(noiseMap[x, y] - settings.absSubtrahend) * settings.absMultiplier;
                    }
                }

            }
        }

        if (settings.normalizeMode == HeightNormalizeMode.Local)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    if (settings.enableAbsMod)
                    {
                        noiseMap[x, y] = Mathf.Abs(noiseMap[x, y] - settings.absSubtrahend) * settings.absMultiplier;
                    }
                }
            }         
        }

        Random.state = initialState;

        return noiseMap;
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseWaveOctavesSettings settings, Vector2 sampleCenter)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapHeight, mapWidth];

        Random.State initialState = Random.state;

        Vector2[] octavesOffsets = new Vector2[settings.octavesSettings.Length];

        float maxGlobalNoiseHeight = 0;

        for (int i = 0; i < settings.octavesSettings.Length; i++)
        {

            float offsetX = +settings.octavesOffset.x + sampleCenter.x;
            float offsetY = -settings.octavesOffset.y - sampleCenter.y;

            Random.InitState(settings.octavesSettings[i].seed.GetHashCode());

            offsetX += Random.Range(-10000, 10001);
            offsetY += Random.Range(-10000, 10001);

            octavesOffsets[i] = new Vector2(offsetX, offsetY);

            maxGlobalNoiseHeight += settings.octavesSettings[i].amplitude;
        }

        if (settings.scale <= 0)
        {
            settings.scale = 0.001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        // Allows us to scale to center
        float halfMapWidth = mapWidth * 0.5f;
        float halfMapHeight = mapHeight * 0.5f;

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float sampleX = 0;
                float sampleZ = 0;

                float noiseHeight = 0f;

                if (settings.enablePolarCoordinatesMode)
                {
                    for (int i = 0; i < settings.octavesSettings.Length; i++)
                    {
                        sampleX = (x - halfMapWidth + octavesOffsets[i].x) / settings.scale * settings.octavesSettings[i].frequency;
                        sampleZ = (z - halfMapHeight + octavesOffsets[i].y) / settings.scale * settings.octavesSettings[i].frequency;

                        float theta = Mathf.PerlinNoise(sampleX, sampleZ) * settings.polarScale;
                        noiseHeight = Mathf.PerlinNoise(sampleX + settings.polarRadius * Mathf.Cos(theta), sampleZ + settings.polarRadius * Mathf.Sin(theta)) * 2 - 1;
                    }
                }
                else
                {
                    for (int i = 0; i < settings.octavesSettings.Length; i++)
                    {
                        sampleX = (x - halfMapWidth + octavesOffsets[i].x) / settings.scale * settings.octavesSettings[i].frequency;
                        sampleZ = (z - halfMapHeight + octavesOffsets[i].y) / settings.scale * settings.octavesSettings[i].frequency;

                        noiseHeight += settings.octavesSettings[i].amplitude * (Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1);
                    }
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, z] = noiseHeight;

                if (settings.normalizeMode == Noise.HeightNormalizeMode.Global)
                {
                    //float noiseNormalizedHeight = Mathf.InverseLerp(-maxGlobalNoiseHeight, maxGlobalNoiseHeight, noiseMap[sampleX, y]);
                    float noiseNormalizedHeight = (noiseMap[x, z] + 1) / (maxGlobalNoiseHeight * 1.1112f);
                    noiseMap[x, z] = Mathf.Clamp(noiseNormalizedHeight, 0, int.MaxValue);
                    //noiseMap[xIndex, zIndex] = Mathf.Clamp(noiseNormalizedHeight, -1.0f, 1.0f);

                    if (settings.enableAbsMod)
                    {
                        noiseMap[x, z] = Mathf.Abs(noiseMap[x, z] - settings.absSubtrahend) * settings.absMultiplier;
                    }
                }
            }
        }

        if (settings.normalizeMode == Noise.HeightNormalizeMode.Local)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, z] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, z]);
                    if (settings.enableAbsMod)
                    {
                        noiseMap[x, z] = Mathf.Abs(noiseMap[x, z] - settings.absSubtrahend) * settings.absMultiplier;
                    }
                }
            }
        }

        Random.state = initialState;
        return noiseMap;
    }

    public static float[,] GenerateBoxLinearFilteredNoiseMap(int mapWidth, int mapHeight, NoiseBoxLinearFilterSettings settings)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapWidth, mapHeight];

        Random.State initialState = Random.state;

        if (settings.useFixedSeed)
        {
            Random.InitState(settings.seed.GetHashCode());
        }
        else
        {
            Random.InitState(Time.time.ToString().GetHashCode());
        }

        for (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, z] = 0.0f;
            }
        }

        //string debug = "";

        if (settings.usePredefinedIterations)
        {
            for (int i = 0; i < settings.iterationsSettings.Length; i++)
            {
                if (settings.iterationsSettings[i].spawnClusters)
                {
                    SpawnHeightClusterMap(ref noiseMap, ref mapWidth, ref mapHeight, 
                        ref settings.iterationsSettings[i].heightClusterMinWidth, ref settings.iterationsSettings[i].heightClusterMaxWidth, 
                        ref settings.iterationsSettings[i].heightClusterMinHeight, ref settings.iterationsSettings[i].heightClusterMaxHeight, ref settings.iterationsSettings[i].clustersCount, ref settings.iterationsSettings[i].clusterValue);

                    //Debug.Log("Spawned");
                    //debug = "";
                    //for (int z = mapHeight - 1; z >= 0; z--)
                    //{
                    //    for (int x = 0; x < mapWidth; x++)
                    //    {
                    //        debug += "" + noiseMap[x, z] + " ";
                    //    }
                    //    debug += "\n";
                    //}
                    //Debug.Log(debug);
                }

                if (settings.iterationsSettings[i].applyFilter)
                {
                    BlurPenaltyMap(ref noiseMap, ref mapWidth, ref mapHeight, ref settings.iterationsSettings[i].filterSize);
                    //Debug.Log("Blurred " + i);
                    //debug = "";
                    //for (int z = mapHeight - 1; z >= 0; z--)
                    //{
                    //    for (int x = 0; x < mapWidth; x++)
                    //    {
                    //        debug += "" + noiseMap[x, z] + " ";
                    //    }
                    //    debug += "\n";
                    //}
                    //Debug.Log(debug + "\n" + i);
                }
            }
        }
        else
        {          
            SpawnHeightClusterMap(ref noiseMap, ref mapWidth, ref mapHeight,
                ref settings.heightClusterMinWidth, ref settings.heightClusterMaxWidth,
                ref settings.heightClusterMinHeight, ref settings.heightClusterMaxHeight, ref settings.clustersCount, ref settings.clusterValue);

            //Debug.Log("Spawned");
            //debug = "";
            //for (int z = mapHeight - 1; z >= 0; z--)
            //{
            //    for (int x = 0; x < mapWidth; x++)
            //    {
            //        debug += "" + noiseMap[x, z] + " ";
            //    }
            //    debug += "\n";
            //}
            //Debug.Log(debug);

            for (int i = 0; i < settings.filterIterationsCount; i++)
            {
                BlurPenaltyMap(ref noiseMap, ref mapWidth, ref mapHeight, ref settings.filterSize);
                //Debug.Log("Blurred " + i);
                //debug = "";
                //for (int z = mapHeight - 1; z >= 0; z--)
                //{
                //    for (int x = 0; x < mapWidth; x++)
                //    {
                //        debug += "" + noiseMap[x, z] + " ";
                //    }
                //    debug += "\n";
                //}
                //Debug.Log(debug + "\n" + i);
            }
        }


    

        Random.state = initialState;

        return noiseMap;
    }

    private static void SpawnHeightClusterMap(ref float[,] noiseMap, ref int gridSizeX, ref int gridSizeY, ref int heightClusterMinWidth, ref int heightClusterMaxWidth, ref int heightClusterMinHeight, ref int heightClusterMaxHeight, ref int clustersCount, ref float clusterValue)
    {
        for (int i = 0; i < clustersCount; i++)
        {
            int clusterWidth = Random.Range(heightClusterMinWidth, heightClusterMaxWidth + 1);
            int clusterHeight = Random.Range(heightClusterMinHeight, heightClusterMaxHeight + 1);

            int spawnX = Random.Range(0, gridSizeX - clusterWidth + 1);
            int spawnY = Random.Range(0, gridSizeY - clusterHeight + 1);

            for (int y = spawnY; y < spawnY + clusterHeight; y++)
            {
                for (int x = spawnX; x < spawnX + clusterWidth; x++)
                {
                    noiseMap[x, y] = clusterValue;
                }
            }
        }
    }

    private static void BlurPenaltyMap(ref float[,] noiseMap, ref int gridSizeX, ref int gridSizeY, ref int blurSize)
    {
        int kernelSize = blurSize * 2 + 1; //width/height of kernel
        int kernelExtents = (kernelSize - 1) / 2; //how many squares between center square and edge square

        float[,] heightsHorizontalPass = new float[gridSizeX, gridSizeY];
        float[,] heightsVerticalPass = new float[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeY; y++)
        {
            //first node in each row
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents); // use values from grid[x, 0] for each square that are outside of grid
                heightsHorizontalPass[0, y] += noiseMap[sampleX, y];
            }

            for (int x = 1; x < gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX); //left-most value, that now are not inside the kernel (because this value are already in penalties grid)
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);    //right-most value, new value of a kernel

                heightsHorizontalPass[x, y] = heightsHorizontalPass[x - 1, y] - noiseMap[removeIndex, y] + noiseMap[addIndex, y];
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                heightsVerticalPass[x, 0] += heightsHorizontalPass[x, sampleY];
            }

            float blurredHeight = heightsVerticalPass[x, 0] / (kernelSize * kernelSize);
            noiseMap[x, 0] = blurredHeight;

            //if (blurredHeight > penaltyMax)
            //{
            //    penaltyMax = blurredHeight;
            //}
            //if (blurredHeight < penaltyMin)
            //{
            //    penaltyMin = blurredHeight;
            //}

            for (int y = 1; y < gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                heightsVerticalPass[x, y] = heightsVerticalPass[x, y - 1] - heightsHorizontalPass[x, removeIndex] + heightsHorizontalPass[x, addIndex];

                blurredHeight = heightsVerticalPass[x, y] / (kernelSize * kernelSize);
                noiseMap[x, y] = blurredHeight;

                //if (blurredHeight > penaltyMax)
                //{
                //    penaltyMax = blurredHeight;
                //}
                //if (blurredHeight < penaltyMin)
                //{
                //    penaltyMin = blurredHeight;
                //}
            }
        }
    }

    public static float[,] GenerateUniformNoiseMap(int mapWidth, int mapHeight, float centerSample, float distanceFromCenterToEdge, float offsetOfNoiseMap, bool isHeightSpreadingFromCenter = true, bool isSpreadingOrientationHorizontal = true)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapWidth, mapHeight];

        float baseNoise = 0;
        float sign = 1f;

        if (isHeightSpreadingFromCenter)
        {
            sign = -1f;
            baseNoise = 1f;
        }

        if (isSpreadingOrientationHorizontal)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                // calculate the sampleZ by summing the index and the offset
                float sampleZ = z + offsetOfNoiseMap;
                // calculate the noise proportional to the distance of the sample to the center of the level
                //The noise is basically the absolute distance from the sample to the center of the Level divided by the maximum distance.
                float noise = baseNoise + sign * Mathf.Abs(sampleZ - centerSample) / distanceFromCenterToEdge;
                // apply the noise for all points with this Z coordinate
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, mapHeight - z - 1] = noise; //need - zIndex - 1 for inverce = the noise will be lower for regions close to the center 
                }
            }
        }
        else
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // calculate the sampleZ by summing the index and the offset
                float sampleX = x + offsetOfNoiseMap;
                // calculate the noise proportional to the distance of the sample to the center of the level
                //The noise is basically the absolute distance from the sample to the center of the Level divided by the maximum distance.
                float noise = baseNoise + sign * Mathf.Abs(sampleX - centerSample) / distanceFromCenterToEdge;
                // apply the noise for all points with this Z coordinate
                for (int z = 0; z < mapWidth; z++)
                {
                    noiseMap[mapWidth - x - 1, z] = noise; //need - zIndex - 1 for inverce = the noise will be lower for regions close to the center 
                }
            }
        }

        return noiseMap;
    }

}


[System.Serializable]
public class NoiseBoxLinearFilterSettings
{
    public bool useFixedSeed = true;
    public string seed = "seed";

    public int clustersCount = 5;
    [Range(0f, 1f)]
    public float clusterValue = 1.0f;

    public int heightClusterMinWidth = 3;
    public int heightClusterMinHeight = 3;
    public int heightClusterMaxWidth = 3;
    public int heightClusterMaxHeight = 3;

    // Must be 
    [Range(1, 5)]
    public int filterSize = 1;
    public int filterIterationsCount = 1;

    public bool usePredefinedIterations;
    public BoxLinearFilterIteration[] iterationsSettings;
}

[System.Serializable]
public class BoxLinearFilterIteration
{
    public bool spawnClusters = true;
    public int clustersCount = 40;
    [Range(0f, 1f)]
    public float clusterValue = 1.0f;

    public int heightClusterMinWidth = 3;
    public int heightClusterMinHeight = 3;
    public int heightClusterMaxWidth = 3;
    public int heightClusterMaxHeight = 3;

    public bool applyFilter = true;
    // Must be 
    [Range(1, 5)]
    public int filterSize = 1;
}

// Lacunarity = controls increase in frequency of octaves (x-axis (length of sinusoid)) == (1..Inf)
// Persistance = controls decrease in amplitude of octaves (y-axis (height of sinusoid)) == (0..1)
// Offset = allows as to scroll through the generated noise
[System.Serializable]
public class NoiseSettings
{
    public bool useFixedSeed = true;
    public string seed = "seed";

    public Noise.HeightNormalizeMode normalizeMode;

    public int numberOfOctaves = 5;

    public float scale = 50.0f;
    /// <summary>
    /// Controls decrease in amplitude of octaves (y-axis (height of sinusoid)) == (0..1)
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float persistance = 0.4f; //Or gain?
    /// <summary>
    /// Controls increase in frequency of octaves (x-axis (length of sinusoid)) == (1..Inf)
    /// </summary>
    public float lacunarity = 2.0f;
    public Vector2 octavesOffset;
    public bool isEachOctaveHasRandomShift = true;

    public bool enableAbsMod = false;
    [Range(0.0f, 1.0f)]
    public float absSubtrahend = 0.5f;
    public float absMultiplier = 2.0f;

    public bool enablePolarCoordinatesMode = false;
    public float polarScale = 1.8f;
    public float polarRadius = 1f;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.0001f);
        numberOfOctaves = Mathf.Max(numberOfOctaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1.0f);
        persistance = Mathf.Clamp01(persistance);
        polarScale = Mathf.Max(polarScale, 0.0001f);
        polarRadius = Mathf.Max(polarRadius, 0.0001f);
        absMultiplier = Mathf.Max(absMultiplier, 1.0f);
    }
}


[System.Serializable]
public class NoiseWaveOctavesSettings
{
    public Noise.HeightNormalizeMode normalizeMode;

    public float scale = 50.0f;
    public Vector2 octavesOffset;

    public Wave[] octavesSettings;
    [Space]
    public bool enableAbsMod = false;
    [Range(0.0f, 1.0f)]
    public float absSubtrahend = 0.5f;
    public float absMultiplier = 2.0f;
    [Space]
    public bool enablePolarCoordinatesMode = false;
    public float polarScale = 1.8f;
    public float polarRadius = 1f;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.0001f);
        polarScale = Mathf.Max(polarScale, 0.0001f);
        polarRadius = Mathf.Max(polarRadius, 0.0001f);
        absMultiplier = Mathf.Max(absMultiplier, 1.0f);
    }
}

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}

