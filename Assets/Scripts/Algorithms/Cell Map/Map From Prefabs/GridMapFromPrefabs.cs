using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridMapFromPrefabs {

    private static MapFromPrefabs mapFromPrefabs;
    private static Dictionary<char, GameObject> dungeonGameObjects;

    private static void LoadDungeonCharacterMappings(ref GridMapFromPrefabsSettings settings)
    {
        //int i = 0;
        //if (settings.dungeonShapes.Count != settings.prefabCharacters.Length)
        //{
        //    Debug.LogError("Prefab shape must be set for each character if prefab characters. And wise versa.");
        //}

        for (int j = 0; j < settings.prefabCharacters.Length; j++)
        {
            char character = settings.prefabCharacters[j];
            int count = 0;

            for (int k = 0; k < settings.prefabCharacters.Length; k++)
            {            
                if (character == settings.prefabCharacters[k])
                {
                    count++;
                }
                if (count > 1)
                {
                    break;
                }
            }
            if (count > 1)
            {
                Debug.LogError("Characters can not appear twice in prefab characters string.");
            }
        }

        /// TODO
        /// Check if prefabs are in dungeonShapes twice. Maybe it is possible to check only inside Editor (using PrefabUtility)
        
        //dungeonGameObjects = new Dictionary<char, GameObject>();
        //// Create dictionary
        //foreach (GameObject dungeonShape in settings.dungeonShapes)
        //{
        //    dungeonGameObjects.Add(settings.prefabCharacters[i], dungeonShape);
        //    i++;
        //}
    }

    // Use this for initialization
    private static void InitializeAndGenerateMap(ref GridMapFromPrefabsSettings settings)
    {
        mapFromPrefabs = new MapFromPrefabs();
        mapFromPrefabs.InitializeMap(ref settings);
        LoadDungeonCharacterMappings(ref settings);

        int visitedCellsCount = 0;

        while (visitedCellsCount > Mathf.FloorToInt((mapFromPrefabs.mapRows - 2) * (mapFromPrefabs.mapColumns - 2) * settings.procentOfTilesThatAreWalkable))
        {
            mapFromPrefabs.InitializeMap(ref settings);
            var visitedCells = mapFromPrefabs.WalkThroughMap();
            visitedCellsCount = GetVisitedCellsCount(ref visitedCells);
        }

        mapFromPrefabs.LogOutputMap();
        //InstantiateDungeonPieces(ref settings);
    }

    private static void InstantiateDungeonPieces(ref GridMapFromPrefabsSettings settings)
    {
        bool[,] visitedCells = new bool[mapFromPrefabs.mapRows, mapFromPrefabs.mapColumns];
        int visitedCellsCount = 0;

        while (visitedCellsCount < Mathf.FloorToInt((mapFromPrefabs.mapRows - 2) * (mapFromPrefabs.mapColumns - 2) * 0.1f))
        {
            mapFromPrefabs.InitializeMap(ref settings);
            visitedCells = mapFromPrefabs.WalkThroughMap();
            visitedCellsCount = GetVisitedCellsCount(ref visitedCells);
        }

        mapFromPrefabs.LogOutputMap();

        for (int r = 1; r < mapFromPrefabs.mapRows - 1; r++)
        {
            for (int c = 1; c < mapFromPrefabs.mapColumns - 1; c++)
            {
                char ch = mapFromPrefabs.map[r, c];

                if (!dungeonGameObjects.ContainsKey(ch) || !visitedCells[r, c])
                {
                    continue;
                }
                else
                {
                    GameObject dungeonGameObject = dungeonGameObjects[ch];
                    //Instantiate(dungeonGameObject, new Vector3(r * 3, 0, c * 3), dungeonGameObject.transform.rotation);
                }
            }
        }
    }

    private static int GetVisitedCellsCount(ref bool[,] visitedCells)
    {
        int visitedCellsCount = 0;

        for (int r = 1; r < mapFromPrefabs.mapRows - 1; r++)
        {
            for (int c = 1; c < mapFromPrefabs.mapColumns - 1; c++)
            {
                if (visitedCells[r, c])
                {
                    visitedCellsCount++;
                }
            }
        }

        return visitedCellsCount;
    }

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, GridMapFromPrefabsSettings settings)
    {
        int maxRoomsCount = settings.gridWidth * settings.gridHeight;

        if (settings.gridWidth < 2)
        {
            settings.gridWidth = 2;
        }
        if (settings.gridHeight < 2)
        {
            settings.gridHeight = 2;
        }

        mapSettings.mapWidth = settings.gridWidth * settings.prefabWidthInTiles;
        mapSettings.mapHeight = settings.gridHeight * settings.prefabHeightInTiles;

        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, GridMapFromPrefabsSettings settings)
    {
        //int mapSeed = 925;
        //int mapSeed = 573;
        Random.State initialState = Random.state;
        if (settings.useFixedSeed)
        {
            Random.InitState(settings.seed.GetHashCode());
        }
        else
        {
            Random.InitState(Time.time.ToString().GetHashCode());
        }

        InitializeAndGenerateMap(ref settings);

        ConvertPrefabMapToCellMap(ref map.values, ref settings);

        Random.state = initialState;

        return map;
    }

    private static void ConvertPrefabMapToCellMap(ref Cell[,] map, ref GridMapFromPrefabsSettings settings, CellType roomWallTile = CellType.Wall, CellType roomFloorTile = CellType.Floor)
    {
        for (int y = 1; y < mapFromPrefabs.mapColumns - 1; y++)
        {
            for (int x = 1; x < mapFromPrefabs.mapRows - 1; x++)
                //for (int x = 1; x < mapFromPrefabs.mapRows - 1; x++)
            {
                int mapX = (x - 1) * settings.prefabWidthInTiles;
                int mapY = (y - 1) * settings.prefabHeightInTiles;

                Cell[,] prefabCell = PrefabToCell(ref mapFromPrefabs.map[x, y], roomWallTile, roomFloorTile);

                string str = "";

                for (int yy = mapY; yy < mapY + settings.prefabWidthInTiles; yy++)
                {
                    for (int xx = mapX; xx < mapX + settings.prefabHeightInTiles; xx++)
                    {
                        map[xx, yy] = prefabCell[xx - mapX, yy - mapY];
                        str += map[xx, yy].cellType;
                    }
                    str += "\n";
                }
                Debug.Log(str);
            }
        }
    }

    private static Cell[,] PrefabToCell(ref char prefabChar, CellType roomWallTile = CellType.Wall, CellType roomFloorTile = CellType.Floor)
    {
        Cell[,] prefab = new Cell[3, 3];

        //CleanUpCells roomWallCells = CleanUpCells.None;
        int x = 3;
        int y = 2;


        foreach (CleanUpCells bitPos in System.Enum.GetValues(typeof(CleanUpCells)))
        {
            if (bitPos == CleanUpCells.None)
            {
                continue;
            }
            switch (bitPos)
            {
                //case CleanUpCells.BottomRight:
                //case CleanUpCells.BottomLeft:
                case CleanUpCells.TopLeft:
                //case CleanUpCells.TopRight:
                    x = 0;
                    y = 2;
                    break;
                //case CleanUpCells.BottomCenter:
                case CleanUpCells.TopCenter:
                    x = 1;
                    y = 2;
                    break;
                //case CleanUpCells.BottomLeft:
                //case CleanUpCells.BottomRight:
                case CleanUpCells.TopRight:
                //case CleanUpCells.TopLeft:
                    x = 2;
                    y = 2;
                    break;
                //case CleanUpCells.MiddleRight:
                case CleanUpCells.MiddleLeft:
                    x = 0;
                    y = 1;
                    break;
                case CleanUpCells.MiddleCenter:
                    x = 1;
                    y = 1;
                    break;
                //case CleanUpCells.MiddleLeft:
                case CleanUpCells.MiddleRight:
                    x = 2;
                    y = 1;
                    break;
                //case CleanUpCells.TopRight:
                //case CleanUpCells.TopLeft:
                //case CleanUpCells.BottomRight:
                case CleanUpCells.BottomLeft:
                    x = 0;
                    y = 0;
                    break;
                //case CleanUpCells.TopCenter:
                case CleanUpCells.BottomCenter:
                    x = 1;
                    y = 0;
                    break;
                //case CleanUpCells.TopLeft:
                //case CleanUpCells.TopRight:
                //case CleanUpCells.BottomLeft:
                case CleanUpCells.BottomRight:
                    x = 2;
                    y = 0;
                    break;
                case CleanUpCells.None:
                    break;
                default:
                    break;
            }

            if (mapFromPrefabs.DoesPrefabHaveAWall(ref prefabChar, bitPos))
            {
                //roomWallCells |= bitPos;
                prefab[x, y] = Cell.CreateCell(roomWallTile);
                //str += prefab[x, y].cellMapChar;
            }
            else
            {
                prefab[x, y] = Cell.CreateCell(roomFloorTile);
                //str += prefab[x, y].cellMapChar;
            }

            //x--;
            //if (x < 0)
            //{
            //    y--;
            //    x = 2;
            //}
        }

        string str = "" + prefabChar + "\n";
        for (y = 2; y >= 0; y--)
        {
            for (x = 0; x < 3; x++)
            {
                str += prefab[x, y].cellMapChar;
            }
            str += "\n";
        }
        Debug.Log(str);

        return prefab;
    }

}
