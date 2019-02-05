using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//settings
//public bool isCentered;
//public float cellScale;
//public float mapOffsetX;
//public float mapOffsetZ;

public static class ContiguousPointyHexMeshGenerator
{

    public static Mesh GenerateTerrainMesh(HeightMap heightMap, ContiguousTerrainSettings settings)
    {
        Mesh mesh = new Mesh();

        Vector2 bottomLeft;
        /// FIX CENTERED (vertexOffest breaks centre)
        if (settings.tileMapSettings.isCentered)
        {
            bottomLeft = new Vector2(-settings.tileMapSettings.mapWidth * settings.tileMapSettings.cellScale * (PointyHexTileData.innerRadius * 2f) * 0.5f,
                -((settings.tileMapSettings.mapHeight - 1) * (PointyHexTileData.outerRadius * 1.5f) + 0.5f * PointyHexTileData.outerRadius) * settings.tileMapSettings.cellScale * 0.5f)
                + new Vector2(settings.tileMapSettings.mapOffsetX, -settings.tileMapSettings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.tileMapSettings.mapOffsetX, -settings.tileMapSettings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        Vector3[] vertices = new Vector3[2 * settings.tileMapSettings.mapWidth + settings.tileMapSettings.mapHeight * (settings.tileMapSettings.mapWidth * 3 + 2)];
        int[,] cellCenters = new int[settings.tileMapSettings.mapWidth, settings.tileMapSettings.mapHeight];
        Vector2[] uv = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[settings.tileMapSettings.mapWidth * settings.tileMapSettings.mapHeight * 18]; //18 = 6 triangles * 3 vertices

        Vector3 vertexOffset = new Vector3(PointyHexTileData.innerRadius, 0.0f, PointyHexTileData.outerRadius) * settings.tileMapSettings.cellScale;

        Color cellColor = Color.blue;

        int vert = 0;
        int tri = 0;
        // Assign vertices
        for (int z = 0; z < settings.tileMapSettings.mapHeight; z++)
        {
            for (int x = 0; x < settings.tileMapSettings.mapWidth; x++)
            {
                Vector3 cellOffset = new Vector3((x + z * 0.5f - z / 2) * (PointyHexTileData.innerRadius * 2f) * settings.tileMapSettings.cellScale,
                    0.0f, z * (PointyHexTileData.outerRadius * 1.5f) * settings.tileMapSettings.cellScale) + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                Vector3 cellSpawnPoint = cellOffset + vertexOffset;

                Debug.DrawRay(cellSpawnPoint, Vector3.up * 3.0f, Color.green, 0.5f);

                cellCenters[x, z] = vert;
                CreateCenter(ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);

                CreateVertices(0, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);

                if (z == 0)
                {
                    CreateVertices(2, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);
                    if (x == 0)
                    {
                        CreateVertices(4, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);
                    }
                }
                else if (z % 2 == 0)
                {
                    if (x == 0)
                    {
                        CreateVertices(4, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);
                    }
                }
                else
                {
                    if (x == 0)
                    {
                        CreateVertices(5, 1, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);
                    }
                    if (x == settings.tileMapSettings.mapWidth - 1)
                    {
                        CreateVertices(2, 1, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.tileMapSettings.cellScale, ref cellColor, ref x, ref z, ref heightMap);
                    }
                }

                // Triangles
                CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 1), ref triangles, ref tri);

                if (z == 0)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, i), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, i + 1), ref triangles, ref tri);
                    }
                    if (x == 0)
                    {
                        for (int i = 3; i < 6; i++)
                        {
                            CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, i), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, i + 1), ref triangles, ref tri);
                        }
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 3), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 2), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 2), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 0), ref triangles, ref tri);
                    }
                }
                else if (z % 2 == 0)
                {
                    CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 0), ref triangles, ref tri);
                    if (x == 0)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 5), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 5), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 4), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 4), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 5), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 5), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 0), ref triangles, ref tri);
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z - 1, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z - 1, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z - 1, 0), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z - 1, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 0), ref triangles, ref tri);
                    }
                }
                else
                {
                    if (x == settings.tileMapSettings.mapWidth - 1)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 2), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 2), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 1), ref triangles, ref tri);
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x + 1, z - 1, 0), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x + 1, z - 1, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 1), ref triangles, ref tri);
                    }

                    CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 0), ref triangles, ref tri);

                    if (x == 0)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 5), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 5), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 0), ref triangles, ref tri);
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x - 1, z, 1), GetVertexInHex(ref cellCenters, ref settings.tileMapSettings.mapWidth, x, z, 0), ref triangles, ref tri);
                    }
                }

            }
        }

        mesh.name = "Contiguous Pointy Hex Terrain Mesh";
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static Mesh GenerateGridMesh(EmptyGrid map, TileMapSettings settings)
    {
        Mesh mesh = new Mesh();

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

        Vector3[] vertices = new Vector3[2 * map.width + map.height * (map.width * 3 + 2)];
        int[,] cellCenters = new int[map.width, map.height];
        Vector2[] uv = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];
        int[] triangles = new int[map.width * map.height * 18]; //6 = 6 triangles * 3 vertices

        Vector3 vertexOffset = new Vector3(PointyHexTileData.innerRadius, 0.0f, PointyHexTileData.outerRadius) * settings.cellScale;

        int vert = 0;
        int tri = 0;
        // Assign vertices
        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3((x + z * 0.5f - z / 2) * (PointyHexTileData.innerRadius * 2f) * settings.cellScale, 0.0f, z * (PointyHexTileData.outerRadius * 1.5f) * settings.cellScale)
                 + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                Vector3 cellSpawnPoint = cellOffset + vertexOffset;

                Debug.DrawRay(cellSpawnPoint, Vector3.up * 3.0f, Color.green, 0.5f);

                cellCenters[x, z] = vert;
                CreateCenter(ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);

                CreateVertices(0, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);

                if (z == 0)
                {
                    CreateVertices(2, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);
                    if (x == 0)
                    {
                        CreateVertices(4, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);
                    }                                                        
                }                                                            
                else if (z % 2 == 0)                                         
                {                                                            
                    if (x == 0)                                              
                    {                                                          
                        CreateVertices(4, 2, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);
                    }
                }
                else
                {
                    if (x == 0)
                    {
                        CreateVertices(5, 1, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);
                    }
                    if (x == map.width - 1)
                    {
                        CreateVertices(2, 1, ref vertices, ref uv, ref colors, ref vert, ref cellSpawnPoint, ref settings.cellScale, ref map.values[x, z].cellMapColor);
                    }
                }

                // Triangles
                CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z,  0), GetVertexInHex(ref cellCenters, ref map.width, x, z, 1), ref triangles, ref tri);

                if (z == 0)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, i), GetVertexInHex(ref cellCenters, ref map.width, x, z, i + 1), ref triangles, ref tri);
                    }
                    if (x == 0)
                    {
                        for (int i = 3; i < 6; i++)
                        {
                            CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, i), GetVertexInHex(ref cellCenters, ref map.width, x, z, i + 1), ref triangles, ref tri);
                        }
                    }
                    else
                    {                       
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 3), GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 2), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 2), GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 1), GetVertexInHex(ref cellCenters, ref map.width, x, z, 0), ref triangles, ref tri);
                    }
                }
                else if (z % 2 == 0)
                {
                    CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 1), GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 0), ref triangles, ref tri);
                    if (x == 0)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 5), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 5), GetVertexInHex(ref cellCenters, ref map.width, x, z, 4), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 4), GetVertexInHex(ref cellCenters, ref map.width, x, z, 5), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 5), GetVertexInHex(ref cellCenters, ref map.width, x, z, 0), ref triangles, ref tri);
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref map.width, x - 1, z - 1, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x - 1, z - 1, 1), GetVertexInHex(ref cellCenters, ref map.width, x - 1, z - 1, 0), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x - 1, z - 1, 0), GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 1), GetVertexInHex(ref cellCenters, ref map.width, x, z, 0), ref triangles, ref tri);
                    }
                }
                else
                {
                    if (x == map.width - 1)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 1), GetVertexInHex(ref cellCenters, ref map.width, x, z, 2), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 2), GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 1), ref triangles, ref tri);
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 1), GetVertexInHex(ref cellCenters, ref map.width, x + 1, z - 1, 0), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x + 1, z - 1, 0), GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 1), ref triangles, ref tri);
                    }

                    CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 1), GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 0), ref triangles, ref tri);

                    if (x == 0)
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref map.width, x, z, 5), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z, 5), GetVertexInHex(ref cellCenters, ref map.width, x, z, 0), ref triangles, ref tri);
                    }
                    else
                    {
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x, z - 1, 0), GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 1), ref triangles, ref tri);
                        CreateTriangle(ref cellCenters[x, z], GetVertexInHex(ref cellCenters, ref map.width, x - 1, z, 1), GetVertexInHex(ref cellCenters, ref map.width, x, z, 0), ref triangles, ref tri);
                    }                  
                }

            }
        }

        mesh.name = "Contiguous PointyHex Mesh";
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }

    public static float GetHeightMapValue(int vertexIndex, int x, int z, ref HeightMap heightMap)
    {
        // find center
        int hZ = (z + 1) * 3 - 1;
        int hX = (x + 1) * 2 + (z % 2 == 1 ? 0 : -1);

        switch (vertexIndex)
        {
            case 0:
                return heightMap.values[hX, hZ];
            case 1:
                hZ += 2;
                return heightMap.values[hX, hZ];
            case 2:
                hZ++;
                hX++;
                return heightMap.values[hX, hZ];
            case 3:
                hZ--;
                hX++;
                return heightMap.values[hX, hZ];
            case 4:
                hZ -= 2;
                return heightMap.values[hX, hZ];
            case 5:
                hZ--;
                hX--;
                return heightMap.values[hX, hZ];
            case 6:
                hZ++;
                hX--;
                return heightMap.values[hX, hZ];
            default:
                Debug.LogError("Wrong Vertex index in GetHeightMapValue.");
                return -1;
        }
    }

    public static void CreateVertices(int index, int verticesCount, ref Vector3[] vertices, ref Vector2[] uv, ref Color[] colors, ref int vert, ref Vector3 cellCenter, ref float cellScale, ref Color cellMapColor, ref int cellX, ref int cellZ, ref HeightMap heightMap)
    {
        for (int i = index; i < index + verticesCount; i++)
        {
            vertices[vert] = cellCenter + PointyHexTileData.vertices[i] * cellScale;
            vertices[vert].y += GetHeightMapValue(i + 1, cellX, cellZ, ref heightMap);
            uv[vert] = PointyHexTileData.uv[i];
            colors[vert] = cellMapColor;
            vert++;
        }
    }
    public static void CreateCenter(ref Vector3[] vertices, ref Vector2[] uv, ref Color[] colors, ref int vert, ref Vector3 cellCenter, ref float cellScale, ref Color cellMapColor, ref int cellX, ref int cellZ, ref HeightMap heightMap)
    {
        vertices[vert] = cellCenter;
        vertices[vert].y += GetHeightMapValue(0, cellX, cellZ, ref heightMap);
        uv[vert] = new Vector2(0.5f, 0.5f);
        colors[vert] = cellMapColor;
        vert++;
    }

    public static void CreateVertices(int index, int verticesCount, ref Vector3[] vertices, ref Vector2[] uv, ref Color[] colors, ref int vert, ref Vector3 cellCenter, ref float cellScale, ref Color cellMapColor)
    {
        for (int i = index; i < index + verticesCount; i++)
        {
            vertices[vert] = cellCenter + PointyHexTileData.vertices[i] * cellScale;
            uv[vert] = PointyHexTileData.uv[i];
            colors[vert] = cellMapColor;
            vert++;
        }
    }
    public static void CreateCenter(ref Vector3[] vertices, ref Vector2[] uv, ref Color[] colors, ref int vert, ref Vector3 cellCenter, ref float cellScale, ref Color cellMapColor)
    {
        vertices[vert] = cellCenter;
        uv[vert] = new Vector2(0.5f, 0.5f);
        colors[vert] = cellMapColor;
        vert++;
    }

    public static void CreateTriangle(ref int center, int v2, int v3, ref int[] triangles, ref int triangleIndex)
    {
        triangles[triangleIndex++] = center;
        triangles[triangleIndex++] = v2;
        triangles[triangleIndex++] = v3;
    }

    public static int GetVertexInHex(ref int[,] cellCenter, ref int width, int x, int z, int cornerCellIndex)
    {
        if (z == 0)
        {
            return cellCenter[x, z] + 1 + cornerCellIndex % 6;
        }
        else if (z % 2 == 0)
        {
            if (x == 0 && cornerCellIndex > 3)
            {
                return cellCenter[x, z] + cornerCellIndex - 1;
            }
        }
        else
        {
            if (x == 0 && cornerCellIndex == 5)
            {
                return cellCenter[x, z] + cornerCellIndex - 2;
            }
        }
        return cellCenter[x, z] + 1 + cornerCellIndex;
    }

}
