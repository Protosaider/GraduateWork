using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscreteSolidPointyHexMeshGenerator
{
    private static readonly float epsilon = 1E-4F;

    static List<Vector3> vertices = new List<Vector3>();
    static List<Color> colors = new List<Color>();
    static List<int> triangles = new List<int>();
    static Mesh mesh;

    public static Mesh GenerateTerrainMesh(EmptyGrid map, TileMapSettings settings, HeightMap heightMap, TerrainHeightLayers heightLayersSettings = null)
    {
        mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();

        Vector2 bottomLeft;
        /// FIX CENTERED (vertexOffest breaks centre)
        if (settings.isCentered)
        {
            bottomLeft = new Vector2(-map.width * settings.cellScale * (PointyHexTileData.innerRadius * 2f) * 0.5f,
                -((map.height - 1) * (PointyHexTileData.outerRadius * 1.5f) + 0.5f * PointyHexTileData.outerRadius) * settings.cellScale * 0.5f)
                + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        Vector3 vertexOffset = new Vector3(PointyHexTileData.innerRadius, 1.0f, PointyHexTileData.outerRadius) * settings.cellScale;

        if (heightLayersSettings != null)
        {
            TerrainHeightLayersGenerator.SeparateTerrainOnLayers(map, settings, heightMap, heightLayersSettings);
        }

        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3((x + z * 0.5f - z / 2) * (PointyHexTileData.innerRadius * 2f) * settings.cellScale, 0.0f, z * (PointyHexTileData.outerRadius * 1.5f) * settings.cellScale)
                 + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                Vector3 spawnPos = cellOffset + vertexOffset;

                spawnPos.y += heightMap.values[x, z];

                CreateSolidHex(spawnPos, settings.cellScale, x, z, map, heightMap);
                Debug.DrawRay(spawnPos, Vector3.up * 3.0f, Color.green, 0.5f);
            }
        }

        mesh.name = "Discrete Solid PointyHex Terrain Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Mesh GenerateGridMesh(EmptyGrid map, TileMapSettings settings)
    {
        mesh = new Mesh();
        //Mesh mesh = new Mesh();
        vertices  = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();

        Vector2 bottomLeft;
        /// FIX CENTERED (vertexOffest breaks centre)
        if (settings.isCentered)
        {
            bottomLeft = new Vector2(-map.width * settings.cellScale * (PointyHexTileData.innerRadius * 2f) * 0.5f,
                -((map.height - 1) * (PointyHexTileData.outerRadius * 1.5f) + 0.5f * PointyHexTileData.outerRadius) * settings.cellScale * 0.5f)
                + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        Vector3 vertexOffset = new Vector3(PointyHexTileData.innerRadius, 1.0f, PointyHexTileData.outerRadius) * settings.cellScale;

        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3((x + z * 0.5f - z / 2) * (PointyHexTileData.innerRadius * 2f) * settings.cellScale, 0.0f, z * (PointyHexTileData.outerRadius * 1.5f) * settings.cellScale)
                 + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                Vector3 spawnPos = cellOffset + vertexOffset;

                if (!settings.showWallsOnly)
                {
                    if (map.values[x, z].cellType == CellType.Empty || map.values[x, z].cellType == CellType.Floor)
                    {
                        int verticesCount;
                        CreateFace(Direction.Up, settings.cellScale, spawnPos + 2 * Vector3.down * settings.cellScale, out verticesCount);
                        ColorizeFace(map.values[x, z].cellMapColor, verticesCount);
                        continue;
                    }
                }
                else
                {
                    if (map.values[x, z].cellType == CellType.Empty || map.values[x, z].cellType == CellType.Floor)
                    {
                        continue;
                    }
                }

                CreateSolidHex(spawnPos, settings.cellScale, x, z, map);
                Debug.DrawRay(spawnPos, Vector3.up * 3.0f, Color.green, 0.5f);
            }
        }

        mesh.name = "Discrete Solid PointyHex Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private static void CreateSolidHex(Vector3 hexPosition, float scale, int x, int z, EmptyGrid map)
    {
        Color faceColor = map.values[x, z].cellMapColor;

        foreach (var direction in SolidPointyHexagonData.PossibleDirections())
        {
            if (map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType == CellType.Empty || map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType == CellType.Floor)
            {
                int verticesCount;
                CreateFace(direction, scale, hexPosition, out verticesCount);
                ColorizeFace(faceColor, verticesCount);
            }
        }
    }

    private static void CreateSolidHex(Vector3 hexPosition, float scale, int x, int z, EmptyGrid map, HeightMap heightMap)
    {
        Color faceColor = map.values[x, z].cellMapColor;

        foreach (var direction in SolidPointyHexagonData.PossibleDirections())
        {
            //CellType neighbourType = map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType;
            //if (neighbourType == CellType.Empty || neighbourType == CellType.Floor ||
            //    (neighbourType == CellType.Wall && heightMap.GetNeighbour(x, z, direction, GridType.PointyHex) < heightMap.values[x, z])
            //    )

            if (map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType == CellType.Empty || map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType == CellType.Floor || 
            ((map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType != CellType.Empty && map.GetNeighbour(x, z, direction, GridType.PointyHex).cellType != CellType.Floor) && heightMap.GetNeighbour(x, z, direction, GridType.PointyHex) < heightMap.values[x, z] - epsilon)
            )
            {
                int verticesCount;
                CreateFace(direction, scale, hexPosition, out verticesCount);
                ColorizeFace(faceColor, verticesCount);
            }
        }
    }

    private static void ColorizeFace(Color color, int verticesCount)
    {
        for (int i = 0; i < verticesCount; i++)
        {
            colors.Add(color);
        }
    }

    private static void CreateFace(Direction direction, float scale, Vector3 hexPosition, out int verticesCount)
    {
        vertices.AddRange(SolidPointyHexagonData.FaceVertices(direction, scale, hexPosition, out verticesCount));

        int triFirstVertCount = vertices.Count - verticesCount;

        for (int i = 1; i < verticesCount - 1; i++)
        {
            triangles.Add(triFirstVertCount);
            triangles.Add(triFirstVertCount + i);
            triangles.Add(triFirstVertCount + i + 1);
        }

        if (direction == Direction.Down || direction == Direction.Up)
        {
            triangles.Add(triFirstVertCount);
            triangles.Add(vertices.Count - 1);
            triangles.Add(triFirstVertCount + 1);
        }
    }
}
