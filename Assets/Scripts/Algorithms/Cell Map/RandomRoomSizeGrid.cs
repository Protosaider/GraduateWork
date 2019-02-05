using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomRoomSizeGrid  {

	public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, RandomSizeRoomGridSettings settings)
    {
        if (settings.gridWidth < 1)
        {
            settings.gridWidth = 1;
        }
        if (settings.gridHeight < 1)
        {
            settings.gridHeight = 1;
        }

        int maxRoomsCount = settings.gridWidth * settings.gridHeight;

        if (settings.roomCount > maxRoomsCount)
        {
            settings.roomCount = maxRoomsCount;
        }
        else if (settings.roomCount <= 0)
        {
            settings.roomCount = 1;
        }

        bool isSpawnPointOutsideGrid = (settings.initialRoomSpawnPoint.x < 0) || (settings.initialRoomSpawnPoint.z < 0) || 
        (settings.initialRoomSpawnPoint.x >= settings.gridWidth) || (settings.initialRoomSpawnPoint.z >= settings.gridHeight);

        if (isSpawnPointOutsideGrid || settings.isSpawnPointCentered)
        {
            settings.initialRoomSpawnPoint.x = Mathf.FloorToInt((settings.gridWidth - 1) * 0.5f);
            settings.initialRoomSpawnPoint.z = Mathf.FloorToInt((settings.gridHeight - 1) * 0.5f);
        }

        if (settings.roomMaxWidth < 3)
        {
            settings.roomMaxWidth = 3;
        }
        if (settings.roomMaxHeight < 3)
        {
            settings.roomMaxHeight = 3;
        }

        if (settings.isSquareShaped)
        {
            settings.roomMaxWidth = settings.roomMaxWidth > settings.roomMaxHeight ? settings.roomMaxWidth : settings.roomMaxHeight;
            settings.roomMaxHeight = settings.roomMaxWidth;
        }

        int mapWidth = settings.roomMaxWidth * settings.gridWidth;
        int mapHeight = settings.roomMaxHeight * settings.gridHeight;

        mapSettings.mapWidth = mapWidth;
        mapSettings.mapHeight = mapHeight;

        if (settings.roomMinWidth < 3)
        {
            settings.roomMinWidth = 3;
        }
        if (settings.roomMinHeight < 3)
        {
            settings.roomMinHeight = 3;
        }

        if (settings.roomMinWidth > settings.roomMaxWidth)
        {
            settings.roomMinWidth = settings.roomMaxWidth;
        }
        if (settings.roomMinHeight > settings.roomMaxHeight)
        {
            settings.roomMinHeight = settings.roomMaxHeight;
        }

        if (settings.isSquareShaped)
        {
            settings.roomMinWidth = settings.roomMinWidth < settings.roomMinHeight ? settings.roomMinWidth : settings.roomMinHeight;
            settings.roomMinHeight = settings.roomMinWidth;
        }

        return mapSettings;
    }


    public static EmptyGrid ProcessMap(EmptyGrid map, RandomSizeRoomGridSettings settings)
    {
        RoomWithCenteredDoors[,] roomGrid = new RoomWithCenteredDoors[settings.gridWidth, settings.gridHeight];
        List<Coordinate> rooms = new List<Coordinate>(settings.gridWidth * settings.gridHeight);

        // Random Generator
        Random.State initialState = Random.state;
        Random.InitState(settings.seed);

        // fill the grid
        if (settings.randomizeRoomPositions)
        {
            for (int z = 0; z < settings.gridHeight; z++)
            {
                for (int x = 0; x < settings.gridWidth; x++)
                {
                    roomGrid[x, z] = new RoomWithCenteredDoors(new Coordinate(x, 0, z), RoomType.Empty);
                    rooms.Add(roomGrid[x, z].positionInGrid);
                }
            }

            // set initial room
            roomGrid[settings.initialRoomSpawnPoint.x, settings.initialRoomSpawnPoint.z] = new RoomWithCenteredDoors(settings.initialRoomSpawnPoint, RoomType.Initial);
            int totalRoomCount = 1;
            Coordinate currentCoord = settings.initialRoomSpawnPoint;
            rooms.Remove(roomGrid[settings.initialRoomSpawnPoint.x, settings.initialRoomSpawnPoint.z].positionInGrid);

            while (totalRoomCount < settings.roomCount)
            {
                int roomIndex = Random.Range(0, rooms.Count);
                currentCoord = rooms[roomIndex];
                roomGrid[currentCoord.x, currentCoord.z] = new RoomWithCenteredDoors(currentCoord, RoomType.Ordinary);
                rooms.Remove(currentCoord);
                totalRoomCount++;
            }
        }
        else
        {
            for (int z = 0; z < settings.gridHeight; z++)
            {
                for (int x = 0; x < settings.gridWidth; x++)
                {
                    roomGrid[x, z] = new RoomWithCenteredDoors(new Coordinate(x, 0, z), RoomType.Empty);
                }
            }

            // set initial room
            roomGrid[settings.initialRoomSpawnPoint.x, settings.initialRoomSpawnPoint.z] = new RoomWithCenteredDoors(settings.initialRoomSpawnPoint, RoomType.Initial);
            int totalRoomCount = 1;
            Coordinate currentCoord = settings.initialRoomSpawnPoint;

            rooms.Add(roomGrid[settings.initialRoomSpawnPoint.x, settings.initialRoomSpawnPoint.z].positionInGrid);

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

                // WARNING: check if is inside the range BEFORE accessing room type
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
            }
        }

        //set all rooms
        for (int z = 0; z < settings.gridHeight; z++)
        {
            for (int x = 0; x < settings.gridWidth; x++)
            {
                Coordinate pos = roomGrid[x, z].positionInGrid;

                pos.x = pos.x * settings.roomMaxWidth;
                pos.z = pos.z * settings.roomMaxHeight;

                int currentRoomWidth = settings.roomMaxWidth;
                int currentRoomHeight = settings.roomMaxHeight;

                //Randomize room sizes, accordingly to that fix position
                if (roomGrid[x, z].type != RoomType.Empty)
                {
                    if (settings.isSquareShaped)
                    {
                        currentRoomWidth = Random.Range(settings.roomMinWidth, currentRoomWidth + 1);
                        currentRoomHeight = currentRoomWidth;
                    }
                    else
                    {
                        currentRoomWidth = Random.Range(settings.roomMinWidth, currentRoomWidth + 1);
                        currentRoomHeight = Random.Range(settings.roomMinHeight, currentRoomHeight + 1);
                    }

                    pos.x = pos.x + Random.Range(0, settings.roomMaxWidth - currentRoomWidth + 1);
                    pos.z = pos.z + Random.Range(0, settings.roomMaxHeight - currentRoomHeight + 1);
                }
                
                for (int zz = 0; zz < currentRoomHeight; zz++)
                {
                    for (int xx = 0; xx < currentRoomWidth; xx++)
                    {
                        map.values[pos.x + xx, pos.z + zz] = new Cell(true, CellType.Floor, Color.grey, '=');
                    }
                }

                switch (roomGrid[x, z].type)
                {
                    case RoomType.Initial:
                        for (int zz = 0; zz < currentRoomHeight; zz++)
                        {
                            map.values[pos.x, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + currentRoomWidth - 1, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        for (int xx = 0; xx < currentRoomWidth; xx++)
                        {
                            map.values[pos.x + xx, pos.z] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + xx, pos.z + currentRoomHeight - 1] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        int roomWidthCenter = Mathf.FloorToInt((currentRoomWidth - 1) * 0.5f), roomHeightCenter = Mathf.FloorToInt((currentRoomHeight - 1) * 0.5f);
                        map.values[pos.x + roomWidthCenter, pos.z + roomHeightCenter] = new Cell(false, CellType.OuterWall, Color.blue, '#');
                        break;
                    case RoomType.Ordinary:
                        for (int zz = 0; zz < currentRoomHeight; zz++)
                        {
                            map.values[pos.x, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + currentRoomWidth - 1, pos.z + zz] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        for (int xx = 0; xx < currentRoomWidth; xx++)
                        {
                            map.values[pos.x + xx, pos.z] = new Cell(false, CellType.Wall, Color.red, '#');
                            map.values[pos.x + xx, pos.z + currentRoomHeight - 1] = new Cell(false, CellType.Wall, Color.red, '#');
                        }
                        break;
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
