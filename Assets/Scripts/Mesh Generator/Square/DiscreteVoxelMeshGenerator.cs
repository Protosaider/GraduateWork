using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscreteVoxelMeshGenerator
{
    private static readonly float epsilon = 1E-4F;

    //static Vector3[] vertices;
    //static Vector2[] uv;
    //static int[] triangles;
    //static Color[] colors;
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

        if (settings.isCentered)
        {
            bottomLeft = new Vector2(-map.width, -map.height) * settings.cellScale + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        //vertices = new Vector3[(map.width + 1) * (map.height + 1)];
        //uv = new Vector2[vertices.Length];
        //triangles = new int[map.width * map.height * 6]; //6 = 2 triangles * 3 vertices
        //colors = new Color[vertices.Length];

        if (heightLayersSettings != null)
        {
            TerrainHeightLayersGenerator.SeparateTerrainOnLayers(map, settings, heightMap, heightLayersSettings);
        }

        // * 0.5f because it's cube
        float cellScale = settings.cellScale * 2;
        Vector3 vertexOffset = Vector3.one * settings.cellScale;

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3(x * cellScale, 0.0f, y * cellScale) + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);
                Vector3 spawnPoint = cellOffset + vertexOffset;

                spawnPoint.y += heightMap.values[x, y];

                CreateCube(spawnPoint, settings.cellScale, x, y, map, heightMap);
                Debug.DrawRay(spawnPoint, Vector3.up * 3.0f, Color.green, 0.5f);
            }
        }

        mesh.name = "Discrete Voxel Terrain Mesh";
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

        if (settings.isCentered)
        {
            bottomLeft = new Vector2(-map.width, -map.height) * settings.cellScale * 0.5f + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        //vertices = new Vector3[(map.width + 1) * (map.height + 1)];
        //uv = new Vector2[vertices.Length];
        //triangles = new int[map.width * map.height * 6]; //6 = 2 triangles * 3 vertices
        //colors = new Color[vertices.Length];

        //Vector3[] vertices = new Vector3[(map.width + 1) * (map.height + 1)];
        //Vector2[] uv = new Vector2[vertices.Length];
        //int[] triangles = new int[map.width * map.height * 6]; //6 = 2 triangles * 3 vertices
        //Color[] colors = new Color[vertices.Length];

        // * 0.5f because it's cube
        float vertexOffset = settings.cellScale * 0.5f;

        int vert = 0;
        int tri = 0;

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3(x * settings.cellScale, 0.0f, y * settings.cellScale) + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                if (!settings.showWallsOnly)
                {
                    if (map.values[x, y].cellType == CellType.Empty || map.values[x, y].cellType == CellType.Floor)
                    {
                        CreateFace(Direction.Up, vertexOffset, cellOffset + (Vector3.forward + Vector3.right) * vertexOffset + Vector3.down * settings.cellScale);
                        ColorizeFace(map.values[x, y].cellMapColor);
                        continue;
                    }
                }
                else
                {
                    if (map.values[x, y].cellType == CellType.Empty || map.values[x, y].cellType == CellType.Floor)
                    {
                        continue;
                    }
                }

                CreateCube(cellOffset + (Vector3.forward + Vector3.right) * vertexOffset, vertexOffset, x, y, map);
                Debug.DrawRay(cellOffset + (Vector3.forward + Vector3.right) * vertexOffset, Vector3.up * 3.0f, Color.green, 0.5f);
            }
        }

        mesh.name = "Discrete Voxel Mesh";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        //mesh.vertices = vertices;
        //mesh.uv = uv;
        //mesh.triangles = triangles;
        //mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }

    private static void CreateCube(Vector3 cubePosition, float scale, int x, int z, EmptyGrid map)
    {
        Color faceColor = map.values[x, z].cellMapColor;

        for (int i = 0; i < 6; i++)
        {
            if (map.GetNeighbour(x, z, (Direction)i).cellType == CellType.Empty || map.GetNeighbour(x, z, (Direction)i).cellType == CellType.Floor)
            {
                CreateFace((Direction)i, scale, cubePosition);
                ColorizeFace(faceColor);
            }
        }
    }

    private static void CreateCube(Vector3 cubePosition, float scale, int x, int z, EmptyGrid map, HeightMap heightMap)
    {
        Color faceColor = map.values[x, z].cellMapColor;

        for (int i = 0; i < 6; i++)
        {
            //if (map.GetNeighbour(x, z, (Direction)i).cellType == CellType.Empty && heightMap.GetNeighbour(x, z, (Direction)i) < heightMap.values[x, z] || map.GetNeighbour(x, z, (Direction)i).cellType == CellType.Floor || 
            //    (map.GetNeighbour(x, z, (Direction)i).cellType == CellType.Wall && heightMap.GetNeighbour(x, z, (Direction)i) < heightMap.values[x, z]) 
            //    )
            if (map.GetNeighbour(x, z, (Direction)i, GridType.PointyHex).cellType == CellType.Empty || map.GetNeighbour(x, z, (Direction)i, GridType.PointyHex).cellType == CellType.Floor ||
                ((map.GetNeighbour(x, z, (Direction)i, GridType.PointyHex).cellType != CellType.Empty && map.GetNeighbour(x, z, (Direction)i, GridType.PointyHex).cellType != CellType.Floor) && heightMap.GetNeighbour(x, z, (Direction)i, GridType.PointyHex) < heightMap.values[x, z] - epsilon)
                )
            {
                CreateFace((Direction)i, scale, cubePosition);
                ColorizeFace(faceColor);
            }
        }
    }

    private static void ColorizeFace(Color color)
    {
        for (int i = 0; i < 4; i++)
        {
            colors.Add(color);
        }
    }

    private static void CreateFace(Direction direction, float scale, Vector3 cubePosition)
    {
        vertices.AddRange(VoxelMeshData.FaceVertices(direction, scale, cubePosition));

        int triFirstVertCount = vertices.Count - 4; //every face have 4 vertices. for example there are 4 vertices, so we need to pass in triangles 012, 023 vertices
                                                    //NUMBER IN VERTICES LIST. Vertex ACTUAL number can be, for example, 1472 (west). But triangles need 012, 023 => 147, 172

        triangles.Add(triFirstVertCount);
        triangles.Add(triFirstVertCount + 1);
        triangles.Add(triFirstVertCount + 2);
        triangles.Add(triFirstVertCount);
        triangles.Add(triFirstVertCount + 2);
        triangles.Add(triFirstVertCount + 3);
    }
}
