using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettlingRooms
{
    private static SettlingRoomsCreator settlingRooms;

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, SettlingRoomsSettings settings)
    {
        settlingRooms = new SettlingRoomsCreator();

        settlingRooms.GenerateMap(settings);

        mapSettings.mapWidth = settlingRooms.Width;      //+1 for zero tile
        mapSettings.mapHeight = settlingRooms.Height;    //+1 for zero tile

        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, SettlingRoomsSettings settings)
    {
        settlingRooms.FillCellMap(ref map.values, CellType.Wall, CellType.Floor);

        return map;
    }
}
