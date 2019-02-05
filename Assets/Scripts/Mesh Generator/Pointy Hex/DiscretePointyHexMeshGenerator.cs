using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiscretePointyHexMeshGenerator
{
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

        Vector3[] vertices = new Vector3[map.width * map.height * 18]; //6 triangles, 3 vertices in each
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[vertices.Length]; //6 = 2 triangles * 3 vertices
        Color[] colors = new Color[vertices.Length];

        Vector3 vertexOffset = new Vector3(PointyHexTileData.innerRadius, 0.0f, PointyHexTileData.outerRadius) * settings.cellScale;

        int vert = 0;

        for (int z = 0; z < map.height; z++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3((x + z * 0.5f - z / 2) * (PointyHexTileData.innerRadius * 2f) * settings.cellScale, 0.0f, z * (PointyHexTileData.outerRadius * 1.5f) * settings.cellScale)
                 + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                Debug.DrawRay(cellOffset + vertexOffset, Vector3.up * 3.0f, Color.green, 0.5f);

                for (int i = 0; i < 6; i++)
                {
                    vertices[vert] = cellOffset + vertexOffset;
                    uv[vert] = new Vector2(0.5f, 0.5f);
                    colors[vert] = map.values[x, z].cellMapColor;
                    triangles[vert] = vert++;

                    vertices[vert] = cellOffset + vertexOffset + PointyHexTileData.vertices[i] * settings.cellScale;
                    uv[vert] = PointyHexTileData.uv[i];
                    colors[vert] = map.values[x, z].cellMapColor;
                    triangles[vert] = vert++;

                    vertices[vert] = cellOffset + vertexOffset + PointyHexTileData.vertices[i + 1] * settings.cellScale;
                    uv[vert] = PointyHexTileData.uv[i + 1];
                    colors[vert] = map.values[x, z].cellMapColor;
                    triangles[vert] = vert++;
                }
            }
        }

        mesh.name = "Discrete PointyHex Mesh";
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }
}

