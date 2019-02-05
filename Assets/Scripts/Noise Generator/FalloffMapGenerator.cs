using UnityEngine;

public static class FalloffMapGenerator  {

    public static float[,] GenerateFalloffMap(int mapWidth, int mapHeight = -1)
    {
        if (mapHeight == -1)
        {
            mapHeight = mapWidth;
        }

        float[,] falloffMap = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                float x = j / (float)mapWidth * 2.0f - 1.0f;
                float y = i / (float)mapHeight * 2.0f - 1.0f;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                falloffMap[j, i] = Evaluate(value);
            }
        }
        return falloffMap;
    }

    private static float Evaluate(float value)
    {
        float a = 3.0f;
        float b = 6.0f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    //public static float[,] GenerateFolloffMap(int mapWidthHeight)
    //{
    //    float[,] map = new float[mapWidthHeight, mapWidthHeight];

    //    Vector2 center = new Vector2(mapWidthHeight / 2f, mapWidthHeight / 2f);

    //    for (int i = 0; i < mapWidthHeight; i++)
    //    {
    //        for (int j = 0; j < mapWidthHeight; j++)
    //        {
    //            float DistanceFromCenter = Vector2.Distance(center, new Vector2(i, j));
    //            float currentAlpha = 1;

    //            if ((1 - (DistanceFromCenter / mapWidthHeight)) >= 0)
    //            {
    //                currentAlpha = (1 - (DistanceFromCenter / mapWidthHeight));
    //            }
    //            else
    //            {
    //                currentAlpha = 0;
    //            }

    //            map[i, j] = Evaluate(currentAlpha);
    //        }
    //    }
    //    return map;
    //}
}
