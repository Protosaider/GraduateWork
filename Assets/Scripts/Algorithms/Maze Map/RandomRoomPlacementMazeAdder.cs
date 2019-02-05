using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoomPlacementMazeAdder : MonoBehaviour {

    private static List<Room> rooms;

    public static T AddRoomsToMaze<T>(T grid, RandomRoomPlacementMazeAdderSettings settings) where T : MazeGrid
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

        rooms = new List<Room>(settings.roomsCount);
        int roomPlacementCount = 0;
        int roomsPlaced = 0;

        while (true)
        {
            bool isPlaced = CreateRoom(ref grid, ref settings);
            if (!isPlaced)
            {
                roomPlacementCount++;
            }
            else
            {
                roomsPlaced++;
                roomPlacementCount = 0;
            }

            if (roomPlacementCount >= settings.roomPlacementTriesCount || roomsPlaced >= settings.roomsCount)
            {
                break;
            }
        }

        foreach (var item in rooms)
        {
            Debug.Log(item.ToString());
        }

        Random.state = initialState;

        return grid;
    }

    private static bool CreateRoom<T>(ref T map, ref RandomRoomPlacementMazeAdderSettings settings) where T : MazeGrid
    {
        int roomWidthSize = Random.Range(settings.roomMinWidth, settings.roomMaxWidth);
        int roomHeightSize = Random.Range(settings.roomMinHeight, settings.roomMaxHeight);

        int startRow = Random.Range(1, map.width - roomWidthSize);
        int startColumn = Random.Range(1, map.height - roomHeightSize);

        int roomWidth = roomWidthSize - 1;
        int roomHeight = roomHeightSize - 1;

        Room room = new Room(startRow, startColumn, roomWidth, roomHeight);
        bool isOverlapping = false;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].IsOverlapping(room))
            {
                isOverlapping = true;
                break;
            }
        }
        if (isOverlapping)
        {
            return false;
        }
        else
        {
            rooms.Add(room);
        }

        float randomEntranceCreationChance = settings.randomEntranceCreationChance;

        List<Coordinate> doors = new List<Coordinate>();

        for (int i = 0; i < settings.maxRandomEntrances; i++)
        {
            if (Random.value < randomEntranceCreationChance)
            {
                randomEntranceCreationChance -= settings.randomEntranceCreationChanceSubtrahend;
            }
            else
            {
                continue;
            }

            int entranceSide = Random.Range(0, 4);

            if (entranceSide % 2 == 0) //even = Top Bottom
            {
                int entranceColumn = Random.Range(0, roomWidthSize);
                if (entranceSide == 0) //Top
                {
                    doors.Add(new Coordinate(startRow + entranceColumn, startColumn + roomHeight));
                }
                else //2
                {
                    doors.Add(new Coordinate(startRow + entranceColumn, startColumn));
                }
            }
            else
            {
                int entranceRow = Random.Range(0, roomHeightSize);
                if (entranceSide == 1) //Right
                {
                    doors.Add(new Coordinate(startRow + roomWidth, startColumn + entranceRow));
                }
                else //3
                {
                    doors.Add(new Coordinate(startRow, startColumn + entranceRow));
                }
            }
        }

        // Top and Bottom
        for (int i = 0; i < roomWidthSize; i++)
        {
            Coordinate current = new Coordinate(startRow + i, startColumn);
            if (!doors.Contains(current))
            {
                //map[startRow + i, startColumn] = '╠';
            }

            current.z += roomHeight;
            if (!doors.Contains(current))
            {
                //map[startRow + i, startColumn + roomHeight] = '╣';
            }
        }

        // Left and Right
        for (int i = 0; i < roomHeightSize; i++)
        {
            Coordinate current = new Coordinate(startRow, startColumn + i);
            if (!doors.Contains(current))
            {
                //map[startRow, startColumn + i] = '╠';
            }

            current.x += roomWidth;
            if (!doors.Contains(current))
            {
                //map[startRow + roomWidth, startColumn + i] = '╣';
            }

        }

        for (int y = 0; y < roomHeightSize; y++)
        {
            for (int x = 0; x < roomWidthSize; x++)
            {
                Coordinate current = new Coordinate(startRow + x, startColumn + y);
                //Coordinate current = new Coordinate(-1, -1);
                if (doors.Contains(current))
                {
                    continue;
                }
                else
                {
                    map.values[startRow + x, startColumn + y] = null;
                }
            }
        }

        return true;
    }
}
