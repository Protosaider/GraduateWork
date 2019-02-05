using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoomWithCorridor {

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, RandomRoomWithCorridorSettings settings)
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
        if (settings.minCorridorLength < 0)
        {
            settings.minCorridorLength = 0;
        }
        if (settings.maxCorridorLength < settings.minCorridorLength)
        {
            settings.maxCorridorLength = settings.minCorridorLength;
        }
        if (mapSettings.mapWidth < settings.roomsMaxWidth + settings.maxCorridorLength)
        {
            mapSettings.mapWidth = settings.roomsMaxWidth + settings.maxCorridorLength;
        }
        if (mapSettings.mapHeight < settings.roomsMaxHeight + settings.maxCorridorLength)
        {
            mapSettings.mapHeight = settings.roomsMaxHeight + settings.maxCorridorLength;
        }

        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, RandomRoomWithCorridorSettings settings)
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

        List<Corridor> corridorsList = new List<Corridor>();

        // Generate random rooms
        List<Room> roomList = GenerateRandomRooms(ref corridorsList, ref map.width, ref map.height, ref settings);

        // Random room placement on map
        for (int i = 0; i < roomList.Count; i++)
        {
            Room room = roomList[i];
            DrawRectangularRoom(ref map.values, room);         
        }
        for (int i = 0; i < corridorsList.Count; i++)
        {
            BresenhamLine(ref map.values, corridorsList[i].startPoint.x, corridorsList[i].startPoint.z, corridorsList[i].endPoint.x, corridorsList[i].endPoint.z, corridorsList[i].direction);
        }

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

    private static List<Room> GenerateRandomRooms(ref List<Corridor> corridors, ref int mapWidth, ref int mapHeight, ref RandomRoomWithCorridorSettings settings)
    {
        List<Room> roomList = new List<Room>();
        int roomsMinWidth = settings.roomsMinWidth - 1;
        int roomsMinHeight = settings.roomsMinHeight - 1;

        int roomWidth = Random.Range(roomsMinWidth, settings.roomsMaxWidth);
        int roomHeight = Random.Range(roomsMinHeight, settings.roomsMaxHeight);
        int x = Random.Range(0, mapWidth - roomWidth);
        int z = Random.Range(0, mapHeight - roomHeight); 

        // center room
        Room newRoom = new Room(Mathf.FloorToInt((mapWidth - roomWidth) * 0.5f), Mathf.FloorToInt((mapHeight - roomHeight) * 0.5f), roomWidth, roomHeight);
        roomList.Add(newRoom);

        int roomsCreated = 1;
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
                    roomsCreated = 1;
                    placementTriesCounter = 0;
                    roomList.Clear();
                    newRoom = new Room(Mathf.FloorToInt((mapWidth - roomWidth) * 0.5f), Mathf.FloorToInt((mapHeight - roomHeight) * 0.5f), roomWidth, roomHeight);
                    roomList.Add(newRoom);
                    spawnRestartsCounter++;
                }
                else
                {
                    canContinuePlacement = false;
                }
            }

            roomWidth = Random.Range(roomsMinWidth, settings.roomsMaxWidth);
            roomHeight = Random.Range(roomsMinHeight, settings.roomsMaxHeight);

            int maxX = mapWidth - roomWidth - 1;
            int maxZ = mapHeight - roomHeight - 1;

            Direction spawnDir = (Direction)Random.Range(0, 4);
            Coordinate doorCoord = roomList[Random.Range(0, roomList.Count)].GetRandomWallCoordinate(spawnDir);

            x = doorCoord.x;
            z = doorCoord.z;

            int corridorLength = Random.Range(settings.minCorridorLength, settings.maxCorridorLength);

            if (spawnDir == Direction.North || spawnDir == Direction.South)
            {
                z += spawnDir == Direction.North ? corridorLength + 1 : -corridorLength - 1;
                spawnDir = spawnDir == Direction.North ? Direction.South : Direction.North;
            }
            else
            {
                x += spawnDir == Direction.East ? corridorLength + 1 : -corridorLength - 1;
                spawnDir = spawnDir == Direction.East ? Direction.West : Direction.East;
            }

            newRoom = new Room(x, z, roomWidth, roomHeight);
            
            Coordinate endDoorCoord = newRoom.GetRandomWallCoordinate(spawnDir);

            int xOffset = endDoorCoord.x - x;
            int zOffset = endDoorCoord.z - z;
            x -= xOffset;
            z -= zOffset;
            endDoorCoord.x -= xOffset;
            endDoorCoord.z -= zOffset;

            bool isOverlapping = false;

            placementTriesCounter++;

            if (x < 0 || x > maxX)
            {
                continue;
            }
            if (z < 0 || z > maxZ)
            {
                continue;
            }

            newRoom.ChangePosition(x, z);

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
                corridors.Add(new Corridor(doorCoord, endDoorCoord, spawnDir));
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

    private static void BresenhamLine(ref Cell[,] map, int x0, int z0, int x1, int z1, Direction dir, CellType floorCell = CellType.Floor, CellType wallType = CellType.Wall)
    {
        bool steep = false;
        if (System.Math.Abs(x0 - x1) < System.Math.Abs(z0 - z1))
        {
            Swap(ref x0, ref z0);
            Swap(ref x1, ref z1);
            steep = true;
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref z0, ref z1);
        }

        int deltaX = x1 - x0;
        int deltaZ = z1 - z0;
        
        int deltaError = System.Math.Abs(deltaZ) * 2;
        int err = 0;

        int z = z0;

        for (int x = x0; x <= x1; x++)
        {
            if (steep)
            {
                map[z, x] = Cell.CreateCell(floorCell);
                if (dir == Direction.North || dir == Direction.South)
                {
                    map[z + 1, x] = Cell.CreateCell(wallType);
                    map[z - 1, x] = Cell.CreateCell(wallType);
                }
                else
                {
                    map[z, x + 1] = Cell.CreateCell(wallType);
                    map[z, x - 1] = Cell.CreateCell(wallType);
                }
            }
            else
            {
                map[x, z] = Cell.CreateCell(floorCell);
                if (dir == Direction.North || dir == Direction.South)
                {
                    map[x + 1, z] = Cell.CreateCell(wallType);
                    map[x - 1, z] = Cell.CreateCell(wallType);
                }       
                else    
                {       
                    map[x, z + 1] = Cell.CreateCell(wallType);
                    map[x, z - 1] = Cell.CreateCell(wallType);
                }
            }

            err += deltaError;
            if (err > deltaX) 
            {
                z += System.Math.Sign(deltaZ);
                err -= deltaX * 2;
            }
        }

    }

    private static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
}


public class Corridor
{
    public Coordinate startPoint;
    public Coordinate endPoint;
    public Direction direction;

    public Corridor(Coordinate start, Coordinate end, Direction dir)
    {
        startPoint = start;
        endPoint = end;
        direction = dir;
    }
}