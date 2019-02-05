using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapManager : MonoBehaviour {

    [Header("Unity objects for map visualization")]
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public Material terrainMaterial;
    [Space]
    public TextureData textureData;
    [Space]
    public bool autoUpdate;

    [Header("General Settings")]
    public TileMapSettings tileMapSettings;

    [Header("Cell map algorithms settings")]
    public FisherYatesShuffleSettings fisherYates;
    public RoomGridSettings roomGrid;
    public RandomSizeRoomGridSettings randomRoomGrid;
    public GameOfLifeSettings gameOfLife;
    public RandomWalkSettings randomWalkers;
    public RandomWormsSettings randomWorms;
    public DiffusionLimitedAggregationSettings difLimAgr;
    public RandomRoomPlacementSettings roomPlacement;
    public BinarySpacePartitioningSettings bsp;
    public GridMapFromPrefabsSettings mapFromPrefab;
    public RandomRoomWithCorridorSettings corridor;
    public SettlingRoomsSettings settling;

    [Header("Height map algorithms settings")]
    public HeightMapSettings heightMapSettings;
    public TerrainHeightLayers heightLayersSettings;
    public DiscreteTerrainSettings discreteTerrainSettings;
    public ContiguousTerrainSettings contiguousTerrainSettings;

    [Header("Maze map algorithms settings")]
    public MazeSettings mazeSettings;
    public MazeBinaryTreeSettings btree;
    public MazeSidewinderSettings sidewinder;
    public RandomRoomPlacementMazeAdderSettings roomAdder;

    public static Texture2D mapInTexture;

    private void Awake()
    {
        //mesh = GetComponent<MeshFilter>().mesh;
    }

    // Use this for initialization
    void Start ()
    {
        //mesh = DiscreteMeshGenerator.GenerateGridMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //mesh = DiscreteMeshGenerator.GenerateGridMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings);
    }

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1.0f, texture.height) / 10.0f;

        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(Mesh mesh)
    {
        meshFilter.sharedMesh = mesh;

        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }

    public void DrawMapInEditor()
    {
        if (tileMapSettings.createOuterWalls)
        {
            if (tileMapSettings.isVoxelMap)
            {
                ///
                /// Room Adder
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(Sidewinder.CreateMaze(RandomRoomPlacementMazeAdder.AddRoomsToMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), roomAdder), sidewinder))), tileMapSettings));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(MazeGenerator.GenerateMazeGrid(mazeSettings))), tileMapSettings));
                ///
                /// Fractal
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(RecursiveDivision.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                ///
                /// Eller
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(Eller.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                /// 
                /// Growing Tree
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(GrowingTree.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(GrowingTree.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), null, x => x[x.Count-1]))), tileMapSettings));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(GrowingTree.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), null, x => Random.value < 0.5f ? x[x.Count-1] : x[Random.Range(0, x.Count)]))), tileMapSettings));
                ///
                /// Prim Weighted Cells
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(PrimWeightedCells.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                ///
                /// Prim Same Weight
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(PrimSameWeight.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));           
                /// 
                /// Kruskal
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(RandomizedKruskal.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                ///
                /// Recursive Backtracker
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(RecursiveBacktracker.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                ///
                /// Hunt and Kill
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(HuntAndKill.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                /// 
                /// Wilson
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(Wilson.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                /// 
                /// Aldous-Broder
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(AldousBroder.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings)))), tileMapSettings));
                /// 
                /// Sidewinder
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(Sidewinder.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), sidewinder))), tileMapSettings));
                /// 
                /// Binary Tree
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(BinaryTree.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), btree))), tileMapSettings));
                /// With Distances
                //DistanceMazeGrid mazeGrid = (DistanceMazeGrid)BinaryTree.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), btree);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(MazeGenerator.ConvertToEmptyGrid(BinaryTree.CreateMaze(MazeGenerator.GenerateMazeGrid(mazeSettings), btree))), tileMapSettings));
                //mazeGrid.distances = mazeGrid.values[0, 0].FindDistanceForAllReachableLinkedCells();
                //Debug.Log(mazeGrid.ToString());
                /// 
                /// MAZES
                /// ///
                /// 
                /// Settling
                //SettlingRooms.PreprocessMap(tileMapSettings, settling);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(SettlingRooms.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), settling)), tileMapSettings));
                /// 
                /// From prefab
                //GridMapFromPrefabs.PreprocessMap(tileMapSettings, mapFromPrefab);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridMapFromPrefabs.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), mapFromPrefab)), tileMapSettings));
                /// 
                /// Corridor
                //RandomRoomWithCorridor.PreprocessMap(tileMapSettings, corridor);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(RandomRoomWithCorridor.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), corridor)), tileMapSettings));
                /// 
                /// BSP
                //BinarySpacePartitioning.PreprocessMap(tileMapSettings, bsp);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(BinarySpacePartitioning.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), bsp)), tileMapSettings));
                ///
                /// Random Room Placement
                //RandomRoomPlacement.PreprocessMap(tileMapSettings, roomPlacement);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(RandomRoomPlacement.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomPlacement)), tileMapSettings));
                /// 
                /// Diffusion Limited Aggregation
                //DiffusionLimitedAggregation.PreprocessMap(tileMapSettings, difLimAgr);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(DiffusionLimitedAggregation.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), difLimAgr)), tileMapSettings));
                /// 
                /// Random Worms
                //RandomWorms.PreprocessMap(tileMapSettings, randomWorms);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(RandomWorms.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), randomWorms)), tileMapSettings));
                ///
                /// Random Walkers
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(RandomWalkers.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), randomWalkers)), tileMapSettings));
                /// 
                /// GameOfLife
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GameOfLifeCave.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), gameOfLife)), tileMapSettings));
                /// 
                /// RandomRoomGrid
                //RandomRoomSizeGrid.PreprocessMap(tileMapSettings, randomRoomGrid);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(RandomRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), randomRoomGrid)), tileMapSettings));
                ///
                /// RoomGrid
                //FixedRoomSizeGrid.PreprocessMap(tileMapSettings, roomGrid);
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid)), tileMapSettings));
                /// RoomGrid + GameOfLife
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid, gameOfLife)), tileMapSettings));
                ///
                ///FisherYates
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(FisherYatesShuffle.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), fisherYates)), tileMapSettings));
                ///
                /// Without Processing
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridGenerator.GenerateEmptyGrid(tileMapSettings)), tileMapSettings));

                /// PointyHex test
                //DrawMesh(DiscretePointyHexMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridGenerator.GenerateEmptyGrid(tileMapSettings)), tileMapSettings));
                //DrawMesh(ContiguousPointyHexMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridGenerator.GenerateEmptyGrid(tileMapSettings)), tileMapSettings));

                //DrawMesh(DiscreteSolidPointyHexMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridGenerator.GenerateEmptyGrid(tileMapSettings)), tileMapSettings));
                //DrawMesh(DiscreteSolidPointyHexMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(FisherYatesShuffle.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), fisherYates)), tileMapSettings));
                //DrawMesh(DiscreteSolidPointyHexMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GameOfLifeCave.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), gameOfLife)), tileMapSettings));

                /// Height maps
                //DrawMesh(DiscreteSolidPointyHexMeshGenerator.GenerateTerrainMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings).FillWholeGrid(CellType.Wall), tileMapSettings, HeightMapGenerator.GenerateHeightMap(heightMapSettings, new Vector2(0.0f, 0.0f), tileMapSettings.mapWidth, tileMapSettings.mapHeight)));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateTerrainMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings, HeightMapGenerator.GenerateHeightMap(heightMapSettings, new Vector2(0.0f, 0.0f), tileMapSettings.mapWidth, tileMapSettings.mapHeight)));

                /// Layers Height
                //DrawMesh(DiscreteSolidPointyHexMeshGenerator.GenerateTerrainMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings, HeightMapGenerator.GenerateHeightMap(heightMapSettings, new Vector2(0.0f, 0.0f), tileMapSettings.mapWidth, tileMapSettings.mapHeight), heightLayersSettings));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateTerrainMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings, HeightMapGenerator.GenerateHeightMap(heightMapSettings, new Vector2(0.0f, 0.0f), tileMapSettings.mapWidth, tileMapSettings.mapHeight), heightLayersSettings));

                //DrawMesh(DiscreteTerrainGenerator.GenerateDiscreteTerrainMesh(discreteTerrainSettings));

                //DrawMesh(ContiguousTerrainGenerator.GenerateContiguousTerrainMesh(contiguousTerrainSettings));

                HeightMap heightMap = ContiguousTerrainGenerator.GenerateContiguousTerrainHeightMap(contiguousTerrainSettings);
                textureData.ApplyToMaterial(terrainMaterial);
                textureData.UpdateMeshHeight(terrainMaterial, heightMap.minValue, heightMap.maxValue);
                DrawMesh(ContiguousTerrainGenerator.GenerateContiguousTerrainFromHM(heightMap, contiguousTerrainSettings));

                ///
                ///IO Tests
                ///
                /// Texture2D
                //ExportMap.ExportGraphic(OuterWallsGenerator.CreateOuterWalls(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid)));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(ExportMap.ImportGraphic("f", "u", "c"), tileMapSettings));
                /// TXT
                //ExportMap.ExportText(OuterWallsGenerator.CreateOuterWalls(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid)));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(ExportMap.ImportText("f", "u", "c"), tileMapSettings));
                /// Binary
                /// BUG: FAILED TO WRITE\READ ARRAY => Need to make custom loader
                //ExportMap.SaveDataBinary(OuterWallsGenerator.CreateOuterWalls(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid)));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(ExportMap.LoadDataBinary(), tileMapSettings));
                /// JSON
                //ExportMap.ExportJSON(OuterWallsGenerator.CreateOuterWalls(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid)));
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(ExportMap.ImportJSON("f","u","c"), tileMapSettings));
            }
            else
            {
                if (tileMapSettings.isDiscrete)
                {
                    DrawMesh(DiscreteMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridGenerator.GenerateEmptyGrid(tileMapSettings)), tileMapSettings));
                }
                else
                {
                    DrawMesh(ContiguousMeshGenerator.GenerateGridMesh(OuterWallsGenerator.CreateOuterWalls(GridGenerator.GenerateEmptyGrid(tileMapSettings)), tileMapSettings));
                }
            }
        }
        else
        {
            if (tileMapSettings.isVoxelMap)
            {
                ///RoomGrid
                FixedRoomSizeGrid.PreprocessMap(tileMapSettings, roomGrid);
                DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(FixedRoomSizeGrid.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), roomGrid), tileMapSettings));
                ///FisherYates
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(FisherYatesShuffle.ProcessMap(GridGenerator.GenerateEmptyGrid(tileMapSettings), fisherYates), tileMapSettings));
                ///Without Processing
                //DrawMesh(DiscreteVoxelMeshGenerator.GenerateGridMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings));
            }
            else
            {
                if (tileMapSettings.isDiscrete)
                {
                    DrawMesh(DiscreteMeshGenerator.GenerateGridMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings));
                }
                else
                {
                    DrawMesh(ContiguousMeshGenerator.GenerateGridMesh(GridGenerator.GenerateEmptyGrid(tileMapSettings), tileMapSettings));
                }
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    //     BoundsInt boundsInt = new BoundsInt(new Vector3Int(1, 1, 1), new Vector3Int(2, 2, 3));
    //     Bounds bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
    //     Bounds boundsCheck = new Bounds(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(1, 1, 1));
    //     Ray ray = new Ray(new Vector3(-0.5f, -0.5f, 10f), Vector3.back);
    //     Ray rayInt = new Ray(new Vector3(1f, 1f, 10f), Vector3.back);

    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawWireCube(boundsInt.center, boundsInt.size);

    //     //Debug.Log(boundsInt.xMin + " " + boundsInt.xMax);
    //     //Debug.Log(boundsInt.size.x + " " + boundsInt.size.z);

    //     if (boundsInt.Contains(new Vector3Int(1, 1, 1)))
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawSphere(new Vector3Int(1, 2, 1), 0.2f);
    //     }

    //     Gizmos.color = Color.black;
    //     Gizmos.DrawRay(ray.origin, ray.direction * 50);

    //     if (bounds.IntersectRay(ray))
    //     {
    //         Gizmos.color = Color.white;
    //         Gizmos.DrawSphere(new Vector3(-0.5f, -0.5f, 0.5f), 0.2f);
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawSphere(bounds.max, 0.1f);
    //         Gizmos.DrawSphere(bounds.min, 0.1f);
    //     }

    //     if (bounds.Intersects(boundsCheck))
    //     {
    //         Gizmos.color = Color.cyan;
    //         Gizmos.DrawLine(bounds.center, boundsCheck.center);
    //         Gizmos.color = Color.green;
    //         Gizmos.DrawWireCube(boundsCheck.center, boundsCheck.size);
    //     }
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireCube(bounds.center, bounds.size);
    // }

    private void OnValuesUpdated()
    {
#if UNITY_EDITOR
        DrawMapInEditor();
#endif     
    }

    private void OnValidate()
    {
        if (tileMapSettings != null)
        {
            tileMapSettings.OnValuesUpdated -= OnValuesUpdated;
            tileMapSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnValuesUpdated;
            textureData.OnValuesUpdated += OnValuesUpdated;
        }
    }
}
