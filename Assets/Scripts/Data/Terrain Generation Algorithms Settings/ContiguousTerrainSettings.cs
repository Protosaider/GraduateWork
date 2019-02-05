using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contiguous - сплошной
[CreateAssetMenu()]
public class ContiguousTerrainSettings : UpdatableData
{
    //// For square grid
    //public const int numberOfSupportedChunkSize = 9;
    //public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    //[Range(0, numberOfSupportedChunkSize - 1)]
    //public int terrainSquareSizeIndex;

    //// number of vertices per line of mesh rendered at LOD = 0. Includes the 2 extra vertices that are excluded from final mesh, but used for calculating normals.
    //public int NumberOfVerticesPerLine
    //{
    //    get
    //    {
    //        /// supportedChunkSizes are in unit => width/height while we need it in number of quads/triangle pairs
    //        ///   0 1 2 3 4 5 6 7 8 9 10
    //        /// 0 @ @ @ @ @ @ @ @ @ @ @
    //        /// 1 @ o o o o o o o o o @
    //        /// 2 @ o # * * # * * # o @
    //        /// 3 @ o * - - - - - * o @
    //        /// 4 @ o * - - - - - * o @
    //        /// 5 @ o # - - # - - # o @
    //        /// 6 @ o * - - - - - * o @
    //        /// 7 @ o * - - - - - * o @
    //        /// 8 @ o # * * # * * # o @
    //        /// 9 @ o o o o o o o o o @
    //        ///10 @ @ @ @ @ @ @ @ @ @ @
    //        ///
    //        /// # - main vertices
    //        /// - - skipped vertices
    //        /// o - out of mesh vertices
    //        /// @ - mesh edge vertices
    //        /// * - edge connection vertices
    //        /// 
    //        /// + 1 - to create triangles\quads
    //        /// + 2 - to create out of mesh vertices to calculate normals
    //        /// + 2 - to create mesh edge vertices to create connection between chunks
    //        return supportedChunkSizes[(useFlatShading) ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
    //    }
    //}

    public const int numberOfSupportedChunkSize = 9;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    [Range(0, numberOfSupportedChunkSize - 1)]
    public int terrainSquareSizeIndex;

    public int NumberOfVerticesPerLine
    {
        get
        {
            return supportedChunkSizes[terrainSquareSizeIndex];
        }
    }

    [Space]
    public bool useRectangularSize;
    public int widthInVertices;
    public int heightInVertices;

    [Space]
    public GridType terrainMeshType = GridType.Square;

    [Space]
    public HeightMapSettings heightMapSettings;
    public TileMapSettings tileMapSettings;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
#endif
}
