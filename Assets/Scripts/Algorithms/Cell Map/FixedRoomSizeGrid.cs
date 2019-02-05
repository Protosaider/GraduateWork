using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FixedRoomSizeGrid
{

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, RoomGridSettings settings)
    {
        int maxRoomsCount = settings.gridWidth * settings.gridHeight;

        if (settings.roomCount > maxRoomsCount)
        {
            settings.roomCount = maxRoomsCount;
        }
        else if (settings.roomCount <= 0)
        {
            settings.roomCount = 1;
        }

        if (settings.roomWidth < 3)
        {
            settings.roomWidth = 3;
        }
        if (settings.roomHeight < 3)
        {
            settings.roomHeight = 3;
        }

        if (settings.isSquareShaped)
        {
            settings.roomWidth = settings.roomWidth > settings.roomHeight ? settings.roomWidth : settings.roomHeight;
            settings.roomHeight = settings.roomWidth;
        }

        settings.roomWidthCenter = Mathf.FloorToInt((settings.roomWidth - 1) * 0.5f);
        settings.roomHeightCenter = Mathf.FloorToInt((settings.roomHeight - 1) * 0.5f);

        bool isSpawnPointOutsideGrid = (settings.initialRoomSpawnPoint.x < 0) || (settings.initialRoomSpawnPoint.z < 0) || 
        (settings.initialRoomSpawnPoint.x >= settings.gridWidth) || (settings.initialRoomSpawnPoint.z >= settings.gridHeight);

        if (isSpawnPointOutsideGrid || settings.isSpawnPointCentered)
        {
            settings.initialRoomSpawnPoint.x = Mathf.FloorToInt((settings.gridWidth - 1) * 0.5f);
            settings.initialRoomSpawnPoint.z = Mathf.FloorToInt((settings.gridHeight - 1) * 0.5f);
        }

        int mapWidth = settings.roomWidth * settings.gridWidth;
        int mapHeight = settings.roomHeight * settings.gridHeight;

        mapSettings.mapWidth = mapWidth;
        mapSettings.mapHeight = mapHeight;

        return mapSettings;
    }

    //public static EmptyGrid ProcessMap(EmptyGrid map, RoomGridSettings settings, GameOfLifeSettings set)
    public static EmptyGrid ProcessMap(EmptyGrid map, RoomGridSettings settings)
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

        RoomWithCenteredDoors[,] roomGrid = new RoomWithCenteredDoors[settings.gridWidth, settings.gridHeight];
        List<Coordinate> rooms = new List<Coordinate>(settings.gridWidth * settings.gridHeight);

        for (int z = 0; z < settings.gridHeight; z++)
        {
            for (int x = 0; x < settings.gridWidth; x++)
            {
                roomGrid[x, z] = new RoomWithCenteredDoors(new Coordinate(x, 0, z), RoomType.Empty);
            }
        }

        // set initial room
        roomGrid[settings.initialRoomSpawnPoint.x, settings.initialRoomSpawnPoint.z] = new RoomWithCenteredDoors(settings.initialRoomSpawnPoint, RoomType.Initial);
        rooms.Add(roomGrid[settings.initialRoomSpawnPoint.x, settings.initialRoomSpawnPoint.z].positionInGrid);

        int totalRoomCount = 1;
        Coordinate currentCoord = settings.initialRoomSpawnPoint;

        // fill the grid
        while (totalRoomCount < settings.roomCount)
        {
            int roomIndex = Random.Range(0, rooms.Count);
            currentCoord = rooms[roomIndex];

            if (!roomGrid[currentCoord.x, currentCoord.z].HasNeighbours())
            {
                rooms.Remove(currentCoord);
                continue;
            }

            Direction direction = RandomDirection();

            Coordinate stepTo = Coordinate.GetOffset(direction);
            Coordinate nextCoord = currentCoord + stepTo;

            bool isOutsideGrid = (nextCoord.x < 0) || (nextCoord.z < 0) || (nextCoord.x >= settings.gridWidth) || (nextCoord.z >= settings.gridHeight);
            /// !!!
            /// IsOutsideGrid check MUST perform FIRST
            if (!isOutsideGrid && roomGrid[nextCoord.x, nextCoord.z].type == RoomType.Empty)
            {
                currentCoord = nextCoord;
                roomGrid[currentCoord.x, currentCoord.z] = new RoomWithCenteredDoors(currentCoord, RoomType.Ordinary);
                rooms.Add(currentCoord);
                totalRoomCount++;
            }
            else
            {
                roomGrid[currentCoord.x, currentCoord.z].CheckDirection(direction);
            }

            /// TODO:
            /// вероятность сгенерировать комнату - добавить
            /// float randomRoomGen = 0.2f, randomStart = 0.2f, randomEnd = 0.01f;
            /// float randomPerc = ((float)i) / (((float)roomsCount - 1));
            /// randomRoomGen = Mathf.Lerp(randomStart, randomEnd, randomPerc);
        }

        // generate doors
        for (int y = 0; y < settings.gridHeight; y++)
        {
            for (int x = 0; x < settings.gridWidth; x++)
            {
                // find neighbours
                Coordinate coordinate = roomGrid[x, y].positionInGrid;

                if (roomGrid[x, y].type == RoomType.Empty)
                {
                    continue;
                }

                for (int i = 0; i < 4; i++)
                {
                    Direction direction = (Direction)i;

                    Coordinate stepTo = Coordinate.GetOffset(direction);
                    Coordinate checkCoord = coordinate + stepTo;

                    bool isOutsideGrid = (checkCoord.x < 0) || (checkCoord.z < 0) || (checkCoord.x >= settings.gridWidth) || (checkCoord.z >= settings.gridHeight);

                    if (isOutsideGrid)
                    {
                        continue;
                    }

                    if (roomGrid[checkCoord.x, checkCoord.z].type != RoomType.Empty)
                    {
                        roomGrid[x, y].CheckDoor(roomGrid[checkCoord.x, checkCoord.z].CheckDoor(direction));
                    }
                }
            }
        }

        //set all rooms
        for (int z = 0; z < settings.gridHeight; z++)
        {
            for (int x = 0; x < settings.gridWidth; x++)
            {
                Coordinate pos = roomGrid[x, z].positionInGrid;

                pos.x = pos.x * settings.roomWidth;
                pos.z = pos.z * settings.roomHeight;

                //! Changed
                //for (int zz = 0; zz < settings.roomHeight; zz++)
                //{
                //    for (int xx = 0; xx < settings.roomWidth; xx++)
                //    {
                //        map.values[pos.x + xx, pos.z + zz] = new Cell(true, CellType.Floor, Color.grey, '=');
                //    }
                //}

                //if (roomGrid[x, z].type != RoomType.Empty)
                //{
                //    // Fill with cells
                //    Cell[,] mask = GameOfLife.TransformBoolToCell(GameOfLife.ProcessMap(settings.roomWidth, settings.roomHeight, set), CellType.Wall, CellType.Floor);

                //    for (int zz = 0; zz < settings.roomHeight; zz++)
                //    {
                //        for (int xx = 0; xx < settings.roomWidth; xx++)
                //        {
                //            map.values[pos.x + xx, pos.z + zz] = mask[xx, zz];
                //        }
                //    }
                //}

                //! Changed
                for (int zz = 1; zz < settings.roomHeight - 1; zz++)
                {
                    for (int xx = 1; xx < settings.roomWidth - 1; xx++)
                    {
                        map.values[pos.x + xx, pos.z + zz] = Cell.CreateCell(CellType.Floor);
                    }
                }

                switch (roomGrid[x, z].type)
                {
                    case RoomType.Initial:
                        for (int zz = 0; zz < settings.roomHeight; zz++)
                        {
                            map.values[pos.x, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + settings.roomWidth - 1, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        for (int xx = 0; xx < settings.roomWidth; xx++)
                        {
                            map.values[pos.x + xx, pos.z] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + xx, pos.z + settings.roomHeight - 1] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        map.values[pos.x + settings.roomWidthCenter, pos.z + settings.roomHeightCenter] = new Cell(false, CellType.Wall, Color.red, '#');
                        break;
                    case RoomType.Ordinary:
                        for (int zz = 0; zz < settings.roomHeight; zz++)
                        {
                            map.values[pos.x, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + settings.roomWidth - 1, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        for (int xx = 0; xx < settings.roomWidth; xx++)
                        {
                            map.values[pos.x + xx, pos.z] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + xx, pos.z + settings.roomHeight - 1] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        break;
                }

                // doors
                if (roomGrid[x, z].hasNorthNeighbour)
                {
                    //map.values[pos.x + settings.roomWidthCenter, pos.z + settings.roomHeight - 1] = new Cell(true, CellType.Door, Color.cyan, '0');
                    map.values[pos.x + settings.roomWidthCenter, pos.z + settings.roomHeight - 1] = Cell.CreateCell(CellType.Floor);
                }
                if (roomGrid[x, z].hasEastNeighbour)
                {
                    //map.values[pos.x + settings.roomWidth - 1, pos.z + settings.roomHeightCenter] = new Cell(true, CellType.Door, Color.cyan, '0');
                    map.values[pos.x + settings.roomWidth - 1, pos.z + settings.roomHeightCenter] = Cell.CreateCell(CellType.Floor);
                }
                if (roomGrid[x, z].hasSouthNeighbour)
                {
                    //map.values[pos.x + settings.roomWidthCenter, pos.z] = new Cell(true, CellType.Door, Color.cyan, '0');
                    map.values[pos.x + settings.roomWidthCenter, pos.z] = Cell.CreateCell(CellType.Floor);
                }
                if (roomGrid[x, z].hasWestNeighbour)
                {
                    //map.values[pos.x, pos.z + settings.roomHeightCenter] = new Cell(true, CellType.Door, Color.cyan, '0');
                    map.values[pos.x, pos.z + settings.roomHeightCenter] = Cell.CreateCell(CellType.Floor);
                }
            }
        }

        Random.state = initialState;

        return map;
    }

    private static Direction RandomDirection()
    {
        float chance = Random.value;
        Direction direction = Direction.Nowhere;

        if (chance < 0.25)
        {
            direction = Direction.North;
        }
        else if (chance >= 0.25 && chance < 0.5)
        {
            direction = Direction.East;
        }
        else if (chance >= 0.5 && chance < 0.75)
        {
            direction = Direction.South;
        }
        else
        {
            direction = Direction.West;
        }

        return direction;
    }

}