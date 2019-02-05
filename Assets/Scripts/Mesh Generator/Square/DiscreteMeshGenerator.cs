using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//settings
//public bool isCentered;
//public float cellScale;
//public float mapOffsetX;
//public float mapOffsetZ;

public static class DiscreteMeshGenerator
{
    public static Mesh GenerateGridMesh(EmptyGrid map, TileMapSettings settings)
    {
        Mesh mesh = new Mesh();

        Vector2 bottomLeft;

        if (settings.isCentered)
        {
            bottomLeft = new Vector2(-map.width, -map.height) * settings.cellScale * 0.5f + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
            //bottomLeft = new Vector2(map.width, map.height) * settings.cellScale + new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }
        else
        {
            bottomLeft = new Vector2(settings.mapOffsetX, -settings.mapOffsetZ);
        }

        Debug.DrawRay(new Vector3(bottomLeft.x, 0.0f, bottomLeft.y), Vector3.up * 5.0f, Color.red, 0.5f);

        Vector3[] vertices = new Vector3[map.width * map.height * 4];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[map.width * map.height * 6]; //6 = 2 triangles * 3 vertices
        Color[] colors = new Color[vertices.Length];

        float vertexOffset = settings.cellScale;

        int vert = 0;
        int tri = 0;

        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Vector3 cellOffset = new Vector3(x * settings.cellScale, 0.0f, y * settings.cellScale) + new Vector3(bottomLeft.x, 0.0f, bottomLeft.y);

                vertices[vert] = cellOffset;                                                        //bottom left
                vertices[vert + 1] = new Vector3(vertexOffset, 0.0f, 0.0f) + cellOffset;            //bottom right 
                vertices[vert + 2] = new Vector3(0.0f, 0.0f, vertexOffset) + cellOffset;            //top left
                vertices[vert + 3] = new Vector3(vertexOffset, 0.0f, vertexOffset) + cellOffset;    //top right

                uv[vert] = new Vector2((float)x / map.width, (float)y / map.height);
                uv[vert + 1] = new Vector2((float)(x + 1) / map.width, (float)y / map.height);
                uv[vert + 2] = new Vector2((float)x / map.width, (float)(y + 1) / map.height);
                uv[vert + 3] = new Vector2((float)(x + 1) / map.width, (float)(y + 1) / map.height);

                colors[vert] = colors[vert + 1] = colors[vert + 2] = colors[vert + 3] = map.values[x, y].cellMapColor;

                Debug.DrawRay(vertices[vert], Vector3.up * 3.0f, Color.green, 0.5f);
                Debug.DrawRay(vertices[vert + 1], Vector3.up * 3.0f, Color.green, 0.5f);
                Debug.DrawRay(vertices[vert + 2], Vector3.up * 3.0f, Color.green, 0.5f);
                Debug.DrawRay(vertices[vert + 3], Vector3.up * 3.0f, Color.green, 0.5f);

                triangles[tri + 2] = vert++;                        //_ _ 1 | _ _ _
                triangles[tri + 1] = triangles[tri + 3] = vert++;   //_ 2 1 | 2 _ _
                triangles[tri] = triangles[tri + 4] = vert++;       //3 2 1 | 2 3 _
                triangles[tri + 5] = vert++;                        //3 2 1 | 2 3 4
                tri += 6;

            }
        }

        mesh.name = "Discrete Mesh";
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;
    }

    //public void AddVertex(Vector3 vertexPosition, Vector2 vertexUV)
    //{
    //vertices[vertexIndex] = vertexPosition;
    //uv[vertexIndex] = vertexUV;
    //}

    //public void AddTriangle(int v1, int v2, int v3)
    //{
    //triangles[triangleIndex] = v1;
    //triangles[triangleIndex + 1] = v2;
    //triangles[triangleIndex + 2] = v3;
    //triangleIndex += 3;
    //}

    //public Mesh CreateMesh()
    //{
    //    Mesh mesh = new Mesh()
    //    {
    //        vertices = this.vertices,
    //        triangles = this.triangles,
    //        uv = this.uv,
    //    };

    //    if (!isDiscrete)
    //    {
    //        mesh.RecalculateNormals();
    //    }
    //    else
    //    {
    //        mesh.normals = bakedNormals;
    //    }

    //    mesh.name = "Map Mesh";

    //    return mesh;
    //}

    //private void FlatShading()
    //{
    //    Vector3[] flatShadedVertices = new Vector3[triangles.Length];
    //    Vector2[] flatShadedUV = new Vector2[triangles.Length];

    //    for (int i = 0; i < triangles.Length; i++)
    //    {
    //        flatShadedVertices[i] = vertices[triangles[i]];
    //        flatShadedUV[i] = uv[triangles[i]];
    //        triangles[i] = i;
    //    }

    //    vertices = flatShadedVertices;
    //    uv = flatShadedUV;
    //}

    //public void ProcessMesh()
    //{
    //    if (!isDiscrete)
    //    {
    //        FlatShading();
    //    }
    //    else
    //    {
    //        BakeNormals();
    //    }
    //}

    //private void BakeNormals()
    //{
    //    bakedNormals = CalculateNormals();
    //}

    //private Vector3[] CalculateNormals()
    //{
    //    Vector3[] vertexNormals = new Vector3[vertices.Length];
    //    int trianglesCount = triangles.Length / 3;

    //    for (int i = 0; i < trianglesCount; i++)
    //    {
    //        int triangleIndex = i * 3;

    //        int vertexIndex1 = triangles[triangleIndex];
    //        int vertexIndex2 = triangles[triangleIndex + 1];
    //        int vertexIndex3 = triangles[triangleIndex + 2];

    //        Vector3 normalVector = CalculateSurfaceNormal(vertexIndex1, vertexIndex2, vertexIndex3);

    //        vertexNormals[vertexIndex1] += normalVector;
    //        vertexNormals[vertexIndex2] += normalVector;
    //        vertexNormals[vertexIndex3] += normalVector;
    //    }

    //    for (int i = 0; i < vertexNormals.Length; i++)
    //    {
    //        vertexNormals[i].Normalize();
    //    }

    //    return vertexNormals;
    //}

    //private Vector3 CalculateSurfaceNormal(int vertexIndex1, int vertexIndex2, int vertexIndex3)
    //{
    //    Vector3 point1 = vertices[vertexIndex1];
    //    Vector3 point2 = vertices[vertexIndex2];
    //    Vector3 point3 = vertices[vertexIndex3];

    //    Vector3 edge1 = point2 - point1;
    //    Vector3 edge2 = point3 - point1;

    //    return Vector3.Cross(edge1, edge2).normalized;
    //}
}

