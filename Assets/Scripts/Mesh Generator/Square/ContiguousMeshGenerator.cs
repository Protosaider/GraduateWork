using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//settings
//public bool isCentered;
//public float cellScale;
//public float mapOffsetX;
//public float mapOffsetZ;

public static class ContiguousMeshGenerator
{

    public static Mesh GenerateTerrainMesh(HeightMap map, ContiguousTerrainSettings settings)
    {
        Mesh mesh = new Mesh();

        Vector2 bottomLeft;

        if (settings.tileMapSettings.isCentered)
        {
            bottomLeft = new Vector2(-map.width, -map.height) * settings.tileMapSettings.cellScale * 0.5f + new Vector2(settings.tileMapSettings.mapOffsetX, -settings.tileMapSettings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.tileMapSettings.mapOffsetX, -settings.tileMapSettings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        Vector3[] vertices = new Vector3[map.width * map.height];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(map.width - 1) * (map.height - 1) * 6]; //6 = 2 triangles * 3 vertices
        Color[] colors = new Color[vertices.Length];

        float vertexOffset = settings.tileMapSettings.cellScale;

        int vert = 0;
        int tri = 0;

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3(x * settings.tileMapSettings.cellScale, 0.0f, y * settings.tileMapSettings.cellScale) + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);
                //cellOffset.y += map.values[x, y] * settings.tileMapSettings.cellScale;
                cellOffset.y += map.values[x, y];

                vertices[vert] = cellOffset;
                uv[vert] = new Vector2((float)x / map.width, (float)y / map.height);
                //colors[vert] = map.values[x == 0 ? x : x - 1, y == 0 ? y : y - 1].cellMapColor;
                colors[vert] = Color.blue;

                Debug.DrawRay(vertices[vert], Vector3.up * 3.0f, Color.green, 0.5f);

                vert++;
            }
        }

        vert = 0;
        for (int y = 0; y < map.height - 1; y++) //careful, <, not <=  Because triangles use i and i+1 column inside the loop
        {
            for (int x = 0; x < map.width - 1; x++)
            {
                triangles[tri] = triangles[tri + 3] = vert;
                triangles[tri + 1] = vert + (map.width);
                triangles[tri + 2] = triangles[tri + 4] = vert + (map.width) + 1;
                triangles[tri + 5] = vert + 1;

                vert++;
                tri += 6;
            }

            vert++;
        }

        mesh.name = "Contiguous Terrain Mesh";
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

        if (settings.isCentered)
        {
            bottomLeft = new Vector2(-map.width, -map.height) * settings.cellScale * 0.5f + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        Vector3[] vertices = new Vector3[(map.width + 1) * (map.height + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[map.width * map.height * 6]; //6 = 2 triangles * 3 vertices
        Color[] colors = new Color[vertices.Length];

        float vertexOffset = settings.cellScale;

        int vert = 0;
        int tri = 0;

        for (int y = 0; y <= map.height; y++)
        {
            for (int x = 0; x <= map.width; x++)
            {
                Vector3 cellOffset = new Vector3(x * settings.cellScale, 0.0f, y * settings.cellScale) + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);
                vertices[vert] = cellOffset;
                uv[vert] = new Vector2((float)x / map.width, (float)y / map.height);
                colors[vert] = map.values[x == 0 ? x : x - 1, y == 0 ? y : y - 1].cellMapColor;

                Debug.DrawRay(vertices[vert], Vector3.up * 3.0f, Color.green, 0.5f);

                vert++;
            }
        }

        vert = 0;
        for (int y = 0; y < map.height; y++) //careful, <, not <=  Because triangles use i and i+1 column inside the loop
        {
            for (int x = 0; x < map.width; x++)
            {
                triangles[tri + 2] = vert;                                      //_ _ 1 | _ _ _
                triangles[tri + 1] = triangles[tri + 3] = vert + 1;             //_ 2 1 | 2 _ _
                //up one row. gridSize + 1 = Column Length;
                triangles[tri] = triangles[tri + 4] = vert + (map.width + 1);   //3 2 1 | 2 3 _
                triangles[tri + 5] = vert + (map.width + 1) + 1;                //3 2 1 | 2 3 4

                vert++;
                tri += 6;
            }

            vert++;
        }

        mesh.name = "Contiguous Mesh";
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }
}
