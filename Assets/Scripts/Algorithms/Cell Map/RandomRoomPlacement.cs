using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomRoomPlacement {

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, RandomRoomPlacementSettings settings)
    {
        if (settings.roomsMinWidth < 3)
        {
            settings.roomsMinWidth = 3;
        }
        if (settings.roomsMinHeight < 3)
        {
            settings.roomsMinHeight = 3;
        }
        if (settings.roomsMaxWidth < settings.roomsMinWidth)
        {
            settings.roomsMaxWidth = settings.roomsMinWidth;
        }
        if (settings.roomsMaxHeight < settings.roomsMinHeight)
        {
            settings.roomsMaxHeight = settings.roomsMinHeight;
        }
        if (mapSettings.mapWidth < settings.roomsMaxWidth)
        {
            mapSettings.mapWidth = settings.roomsMaxWidth;
        }
        if (mapSettings.mapHeight < settings.roomsMaxHeight)
        {
            mapSettings.mapHeight = settings.roomsMaxHeight;
        }

        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, RandomRoomPlacementSettings settings)
    {
        // Random Generator
        Random.State initialState = Random.state;
        if (settings.useFixedSeed)
        {
            Random.InitState(settings.seed.GetHashCode());
        }
        else
        {
            Random.InitState(Time.time.ToString().GetHashCode());
        }
        //! Added
        if (settings.isRoomsConnected)
        {
            map.FillWholeGrid(CellType.Empty);
        }

        // Generate random rooms
        List<Room> roomList = GenerateRandomRooms(ref map.width, ref map.height, ref settings);

        // Random room placement on map
        foreach (Room room in roomList)
        {
            //Debug.Log("I'm inside " + room.boundsInt.xMin + " " + room.boundsInt.xMax + " " + room.boundsInt.zMin + " " + room.boundsInt.zMax);
            DrawRectangularRoom(ref map.values, room);
        }

        //! Added
        if (settings.isRoomsConnected)
        {
            List<Line> lines = new List<Line>();
            /// MST
            for (int i = 0; i < roomList.Count - 1; i++)
            {
                int minIndex = -1;
                float minDistance = float.MaxValue;
                for (int j = i + 1; j < roomList.Count; j++)
                {
                    //if (j == i)
                    //{
                    //    continue;
                    //}
                    float distance = UnityUtilities.Distances.GetManhattanDistance(roomList[i].Midpoint, roomList[j].Midpoint);
                    if (distance < minDistance)
                    {
                        minIndex = j;
                        minDistance = distance;
                    }
                }
                Line line = new Line(roomList[i].Midpoint, roomList[minIndex].Midpoint);
                lines.Add(line);
            }

            NodeGrid nodeGrid = new NodeGrid(ref map);
            Pathfinding pathfinding = new Pathfinding();

            foreach (var item in lines)
            {
                List<Node> path = pathfinding.FindPath(nodeGrid.values[item.start.x, item.start.y], nodeGrid.values[item.end.x, item.end.y], ref nodeGrid, false, true, false);
                foreach (var node in path)
                {
                    map.values[node.x, node.z] = Cell.CreateCell(CellType.Floor);
                    for (int z = node.z - 1; z <= node.z + 1; z++)
                    {
                        for (int x = node.x - 1; x <= node.x + 1; x++)
                        {
                            if (!nodeGrid.IsOutside(x, z) || (x != z && x != node.x))
                            {
                                if (map.values[x, z].cellType == CellType.Empty)
                                {
                                    map.values[x, z] = Cell.CreateCell(CellType.Wall);
                                }
                            }
                        }
                    }
                }
            }
        }
        ////////List<Corridor> corridors = new List<Corridor>();
        ////////foreach (var room in roomList)
        ////////{
        ////////    map.values[room.CenterX, room.CenterZ] = Cell.CreateCell(CellType.OuterWall);
        ////////}
        ////////// Connect rooms
        ////////while (roomList.Count > 1)
        ////////{
        ////////    Room room = roomList[Random.Range(0, roomList.Count)];
        ////////    roomList.Remove(room);
        ////////    Room room2 = roomList[Random.Range(0, roomList.Count)];
        ////////    roomList.Remove(room2);
        ////////    Corridor corridor = new Corridor(room.GetCenter(), room2.GetCenter(), Direction.Nowhere);
        ////////    //if (Random.value < 0.2f)
        ////////    //{
        ////////        if (Random.value < 0.5f)
        ////////        {
        ////////            roomList.Add(room);
        ////////        }
        ////////        else
        ////////        {
        ////////            roomList.Add(room2);
        ////////        }
        ////////    //}
        ////////    corridors.Add(corridor);
        ////////}
        ////////for (int i = 0; i < corridors.Count; i++)
        ////////{
        ////////    //DrawVerticalCorridor(ref map.values, corridors[i], CellType.Wall, CellType.OuterWall);
        ////////    //DrawHorizontalCorridor(ref map.values, corridors[i], CellType.Wall, CellType.OuterWall);
        ////////}

        // Connect floor tiles that are between single wall
        if (settings.hasCleanup)
        {
            CleanUpRoomPlacement(ref map.values, ref map.width, ref map.height);
        }

        Random.state = initialState;

        return map;
    }

    private static void DrawRectangularRoom(ref Cell[,] map, Room room, CellType roomWallsCell = CellType.Wall, CellType roomFloorCell = CellType.Floor)
    {
        for (int z = room.boundsInt.zMin + 1; z < room.boundsInt.zMax; z++)
        {
            for (int x = room.boundsInt.xMin + 1; x < room.boundsInt.xMax; x++)
            {
                map[x, z] = Cell.CreateCell(roomFloorCell);
            }
        }
        for (int x = room.boundsInt.xMin; x <= room.boundsInt.xMax; x++)
        {
            map[x, room.boundsInt.zMin] = Cell.CreateCell(roomWallsCell);
            map[x, room.boundsInt.zMax] = Cell.CreateCell(roomWallsCell);
        }
        for (int z = room.boundsInt.zMin; z <= room.boundsInt.zMax; z++)
        {
            map[room.boundsInt.xMin, z] = Cell.CreateCell(roomWallsCell);
            map[room.boundsInt.xMax, z] = Cell.CreateCell(roomWallsCell);
        }
    }

    private static void DrawVerticalCorridor(ref Cell[,] map, Corridor corridor, CellType roomWallsCell = CellType.Wall, CellType roomFloorCell = CellType.Floor)
    {
        for (int i = Mathf.Min(corridor.startPoint.z, corridor.endPoint.z); i < Mathf.Max(corridor.startPoint.z, corridor.endPoint.z) + 1; i++)
        {
            map[corridor.startPoint.x, i] = Cell.CreateCell(roomFloorCell);
            //if (map[corridor.startPoint.x - 1, i].cellType == CellType.Empty)
            //{
            //    map[corridor.startPoint.x - 1, i] = Cell.CreateCell(roomWallsCell);
            //}
            //if (map[corridor.startPoint.x + 1, i].cellType == CellType.Empty)
            //{
            //    map[corridor.startPoint.x + 1, i] = Cell.CreateCell(roomWallsCell);
            //}
        }
    }

    private static void DrawHorizontalCorridor(ref Cell[,] map, Corridor corridor, CellType roomWallsCell = CellType.Wall, CellType roomFloorCell = CellType.Floor)
    {
        for (int i = Mathf.Min(corridor.startPoint.x, corridor.endPoint.x); i < Mathf.Max(corridor.startPoint.x, corridor.endPoint.x) + 1; i++)
        {
            map[i, corridor.startPoint.z] = Cell.CreateCell(roomFloorCell);
            //if (map[i, corridor.endPoint.z - 1].cellType == CellType.Empty)
            //{
            //    map[i, corridor.endPoint.z - 1] = Cell.CreateCell(roomWallsCell);
            //}
            //if (map[i, corridor.endPoint.z + 1].cellType == CellType.Empty)
            //{
            //    map[i, corridor.endPoint.z + 1] = Cell.CreateCell(roomWallsCell);
            //}
        }
    }

    private static List<Room> GenerateRandomRooms(ref int mapWidth, ref int mapHeight, ref RandomRoomPlacementSettings settings)
    {
        List<Room> roomList = new List<Room>();
        int roomsMinWidth = settings.roomsMinWidth - 1;
        int roomsMinHeight = settings.roomsMinHeight - 1;

        //int roomWidth = Random.Range(settings.roomsMinWidth - 1, settings.roomsMaxWidth); // - 1 = because width 3 == 4 tiles 
        //int roomHeight = Random.Range(settings.roomsMinHeight - 1, settings.roomsMaxHeight); // not sub 1 from max 'cause Random.Range exclude max (generate range [min;max-1] or [min;max))

        int roomWidth = Random.Range(roomsMinWidth, settings.roomsMaxWidth);
        int roomHeight = Random.Range(roomsMinHeight, settings.roomsMaxHeight);
        int x = Random.Range(0, mapWidth - roomWidth); // 20 - 2 => 18, but tiles will be: 18, 19, 20. That's why we need to sub 1
        int z = Random.Range(0, mapHeight - roomHeight); //but - because Random.Range exclude max, we do not need to sub 1
        Room newRoom = new Room(x, z, roomWidth, roomHeight);

        int roomsCreated = 0;
        int placementTriesCounter = 0;
        int spawnRestartsCounter = 0;

        bool canContinuePlacement = true;

        while (canContinuePlacement)
        {
            if (settings.tryToGenerateSpecificAmountOfRooms && (roomsCreated == settings.amountOfRoomsToGenerate))
            {
                canContinuePlacement = false;
                continue;
            }

            if (placementTriesCounter >= settings.roomPlacementTriesCount)
            {
                if (settings.tryToGenerateSpecificAmountOfRooms && (spawnRestartsCounter < settings.timesToRestartSpawn))
                {
                    roomsCreated = 0;
                    placementTriesCounter = 0;
                    roomList.Clear();
                    spawnRestartsCounter++;
                }
                else
                {
                    canContinuePlacement = false;
                }
            }

            roomWidth = Random.Range(roomsMinWidth, settings.roomsMaxWidth);
            roomHeight = Random.Range(roomsMinHeight, settings.roomsMaxHeight);
            x = Random.Range(0, mapWidth - roomWidth);
            z = Random.Range(0, mapHeight - roomHeight);

            bool isOverlapping = false;

            //newRoom.Respawn(x, z, roomWidth, roomHeight);
            newRoom = new Room(x, z, roomWidth, roomHeight);
            placementTriesCounter++;

            foreach (Room room in roomList)
            {
                if (settings.canShareSingleWall ? room.IsOverlappingExclusive(newRoom) : room.IsOverlapping(newRoom))
                {
                    isOverlapping = true;
                    break;
                }
            }

            if (isOverlapping)
            {
                continue;
            }
            else
            {
                roomsCreated++;
                roomList.Add(newRoom);
            }
        }

        return roomList;
    }

    private static void CleanUpRoomPlacement(ref Cell[,] map, ref int width, ref int height, CellType floorCell = CellType.Floor)
    {
        CleanUpCells cleaner = CleanUpCells.None;
        int byteCounter = 1;
        int debugCounter = 0;
        for (int mapX = 1; mapX < width - 3; mapX++)
        {
            for (int mapY = 1; mapY < height - 3; mapY++)
            {
                byteCounter = 1;
                cleaner = CleanUpCells.None;
                debugCounter++;
                for (int x = mapX; x < mapX + 3; x++)
                {
                    for (int y = mapY; y < mapY + 3; y++)
                    {
                        if (map[x, y].cellType == floorCell)
                        {
                            cleaner |= (CleanUpCells)byteCounter; //OR operator for each Floor Tile
                        }
                        byteCounter *= 2;
                    }
                }

                //if (cleaner == (CleanUpCells.TopLeft | CleanUpCells.TopRight))
                if ((cleaner & (CleanUpCells.TopLeft | CleanUpCells.TopRight)) == (CleanUpCells.TopLeft | CleanUpCells.TopRight) && (cleaner & CleanUpCells.TopCenter) == CleanUpCells.None)
                {
                    map[mapX + 1, mapY + 2] = Cell.CreateCell(floorCell);
                }

                //if (cleaner == (CleanUpCells.TopLeft | CleanUpCells.BottomLeft))
                if ((cleaner & (CleanUpCells.TopLeft | CleanUpCells.BottomLeft)) == (CleanUpCells.TopLeft | CleanUpCells.BottomLeft) && (cleaner & CleanUpCells.MiddleLeft) == CleanUpCells.None)
                {
                    map[mapX, mapY + 1] = Cell.CreateCell(floorCell);
                }

                //if ((cleaner == (CleanUpCells.TopCenter | CleanUpCells.BottomCenter)) || (cleaner == (CleanUpCells.MiddleLeft | CleanUpCells.MiddleRight)))
                if (((cleaner & (CleanUpCells.TopCenter | CleanUpCells.BottomCenter)) == (CleanUpCells.TopCenter | CleanUpCells.BottomCenter)) || ((cleaner & (CleanUpCells.MiddleLeft | CleanUpCells.MiddleRight)) == (CleanUpCells.MiddleLeft | CleanUpCells.MiddleRight)) && (cleaner & CleanUpCells.MiddleCenter) == CleanUpCells.None)
                {
                    map[mapX + 1, mapY + 1] = Cell.CreateCell(floorCell);
                }

                //if (cleaner == (CleanUpCells.BottomRight | CleanUpCells.TopRight))
                if ((cleaner & (CleanUpCells.BottomRight | CleanUpCells.TopRight)) == (CleanUpCells.BottomRight | CleanUpCells.TopRight) && (cleaner & CleanUpCells.MiddleRight) == CleanUpCells.None)
                {
                    map[mapX + 2, mapY + 1] = Cell.CreateCell(floorCell);
                }

                //if (cleaner == (CleanUpCells.BottomRight | CleanUpCells.BottomLeft))
                if ((cleaner & (CleanUpCells.BottomRight | CleanUpCells.BottomLeft)) == (CleanUpCells.BottomRight | CleanUpCells.BottomLeft) && (cleaner & CleanUpCells.BottomCenter) == CleanUpCells.None)
                {
                    map[mapX + 1, mapY] = Cell.CreateCell(floorCell);
                }
            }
        }
    }
}
