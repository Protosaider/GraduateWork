using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySpacePartitioning : MonoBehaviour
{

    public static TileMapSettings PreprocessMap(TileMapSettings mapSettings, BinarySpacePartitioningSettings settings)
    {
        if (settings.roomsMinHeight < 3)
        {
            settings.roomsMinHeight = 3;
        }
        if (settings.roomsMinWidth < 3)
        {
            settings.roomsMinWidth = 3;
        }

        if (settings.useRoomsMaxSizeValues)
        {
            if (settings.roomsMaxHeight < settings.roomsMinHeight)
            {
                settings.roomsMaxHeight = settings.roomsMinHeight;
            }
            if (settings.roomsMaxWidth < settings.roomsMinWidth)
            {
                settings.roomsMaxWidth = settings.roomsMinWidth;
            }

            settings.roomsMinHeight = settings.roomsMaxHeight;
            settings.roomsMinWidth = settings.roomsMaxWidth;

            if (settings.minSplitSizeHorizontal < settings.roomsMaxHeight)
            {
                settings.minSplitSizeHorizontal = settings.roomsMaxHeight;
            }
            if (settings.minSplitSizeVertical < settings.roomsMaxWidth)
            {
                settings.minSplitSizeVertical = settings.roomsMaxWidth;
            }

            if (settings.minSpaceSizeHorizontal < settings.minSplitSizeHorizontal * 2 - 1)
            {
                settings.minSpaceSizeHorizontal = settings.minSplitSizeHorizontal * 2 - 1;
            }
            if (settings.minSpaceSizeVertical < settings.minSplitSizeVertical * 2 - 1)
            {
                settings.minSpaceSizeVertical = settings.minSplitSizeVertical * 2 - 1;
            }

            if (mapSettings.mapHeight < settings.minSpaceSizeHorizontal)
            {
                mapSettings.mapHeight = settings.minSpaceSizeHorizontal;
            }
            if (mapSettings.mapWidth < settings.minSpaceSizeVertical)
            {
                mapSettings.mapWidth = settings.minSpaceSizeVertical;
            }
        }
        else
        {
            if (settings.minSplitSizeHorizontal < settings.roomsMinHeight)
            {
                settings.minSplitSizeHorizontal = settings.roomsMinHeight;
            }
            if (settings.minSplitSizeVertical < settings.roomsMinWidth)
            {
                settings.minSplitSizeVertical = settings.roomsMinWidth;
            }

            if (settings.minSpaceSizeHorizontal < settings.minSplitSizeHorizontal * 2 - 1)
            {
                settings.minSpaceSizeHorizontal = settings.minSplitSizeHorizontal * 2 - 1;
            }
            if (settings.minSpaceSizeVertical < settings.minSplitSizeVertical * 2 - 1)
            {
                settings.minSpaceSizeVertical = settings.minSplitSizeVertical * 2 - 1;
            }

            if (mapSettings.mapHeight < settings.minSpaceSizeHorizontal)
            {
                mapSettings.mapHeight = settings.minSpaceSizeHorizontal;
            }
            if (mapSettings.mapWidth < settings.minSpaceSizeVertical)
            {
                mapSettings.mapWidth = settings.minSpaceSizeVertical;
            }
        }

        return mapSettings;
    }

    public static EmptyGrid ProcessMap(EmptyGrid map, BinarySpacePartitioningSettings settings)
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

        // Set root
        BSPLeaf root = new BSPLeaf(0, 0, map.width - 1, map.height - 1);

        // Space Partition
        root.Split(ref settings);

        // Get terminal leaves
        List<BSPLeaf> list = new List<BSPLeaf>();
        root.GetLeaves(ref list);

        List<Vector2> midpoints = new List<Vector2>();

        // Recursive Division
        if (settings.useOnlyRecursiveDivision)
        {
            // Fill initial
            for (int y = 0; y < map.height; y++)
            {
                for (int x = 0; x < map.width; x++)
                {
                    map.values[x, y] = Cell.CreateCell(CellType.Floor);
                }
            }
            DrawBSPRoom(ref map.values, ref midpoints, ref settings, root);
        }
        else
        {
            // Room placement on map
            foreach (BSPLeaf leaf in list)
            {
                DrawRectangularRoom(ref map.values, ref midpoints, ref settings, leaf);
            }
        }

        if (settings.hasCleanup)
        {
            CleanUpRoomPlacement(ref map.values, ref map.width, ref map.height);
        }

        Random.state = initialState;

        return map;
    }

    private static void DrawBSPRoom(ref Cell[,] map, ref List<Vector2> midpoints, ref BinarySpacePartitioningSettings settings, BSPLeaf leaf, CellType roomWallsCell = CellType.Wall, CellType roomFloorCell = CellType.Floor)
    {
        if (leaf.Left != null && leaf.Right != null)
        {
            if (leaf.splitOrientationIsVertical)
            {
                int passageAt = Random.Range(leaf.Left.bounds.zMin, leaf.Left.bounds.zMin + leaf.Left.SizeZ + 1);
                int divideCellIndex = leaf.Left.bounds.xMin + leaf.Left.SizeX;

                for (int z = leaf.bounds.zMin; z <= leaf.bounds.zMin + leaf.SizeZ; z++)
                {
                    if (passageAt != z)
                    {
                        map[divideCellIndex, z] = Cell.CreateCell(roomWallsCell);
                    }
                    else
                    {
                        map[divideCellIndex, z] = Cell.CreateCell(roomFloorCell);
                    }
                }
            }
            else
            {
                int passageAt = Random.Range(leaf.Left.bounds.xMin, leaf.Left.bounds.xMin + leaf.Left.SizeX + 1);
                int divideCellIndex = leaf.Left.bounds.zMin + leaf.Left.SizeZ;

                while (passageAt == divideCellIndex)
                {
                    passageAt = Random.Range(leaf.Left.bounds.xMin, leaf.Left.bounds.xMin + leaf.Left.SizeX + 1);
                }

                for (int x = leaf.bounds.xMin; x <= leaf.bounds.xMin + leaf.SizeX; x++)
                {
                    if (passageAt != x)
                    {
                        map[x, divideCellIndex] = Cell.CreateCell(roomWallsCell);
                    }
                    else
                    {
                        map[x, divideCellIndex] = Cell.CreateCell(roomFloorCell);
                    }
                }
            }

            DrawBSPRoom(ref map, ref midpoints, ref settings, leaf.Left, roomWallsCell, roomFloorCell);
            DrawBSPRoom(ref map, ref midpoints, ref settings, leaf.Right, roomWallsCell, roomFloorCell);
        }
        else
        {
            return;
        }
    }

    private static void DrawRectangularRoom(ref Cell[,] map, ref List<Vector2> midpoints, ref BinarySpacePartitioningSettings settings, BSPLeaf leaf, CellType roomWallsCell = CellType.Wall, CellType roomFloorCell = CellType.Floor)
    {
        int roomsMinWidth = settings.roomsMinWidth - 1;
        int roomsMinHeight = settings.roomsMinHeight - 1;

        int roomWidth;
        int roomHeight;

        if (settings.useRoomsMaxSizeValues)
        {
            roomWidth = Random.Range(roomsMinWidth, settings.roomsMaxWidth);
            roomHeight = Random.Range(roomsMinHeight, settings.roomsMaxHeight);
        }
        else
        {
            roomWidth = Random.Range(roomsMinWidth, leaf.SizeX + 1);
            roomHeight = Random.Range(roomsMinHeight, leaf.SizeZ + 1);
        }

        int xOffset = Random.Range(0, leaf.SizeX - roomWidth);
        int zOffset = Random.Range(0, leaf.SizeZ - roomHeight);

        for (int z = leaf.bounds.zMin + zOffset + 1; z < leaf.bounds.zMin + zOffset + roomHeight; z++)
        {
            for (int x = leaf.bounds.xMin + xOffset + 1; x < leaf.bounds.xMin + xOffset + roomWidth; x++)
            {
                map[x, z] = Cell.CreateCell(roomFloorCell);
            }
        }
        for (int x = leaf.bounds.xMin + xOffset; x <= leaf.bounds.xMin + xOffset + roomWidth; x++)
        {
            map[x, leaf.bounds.zMin + zOffset] = Cell.CreateCell(roomWallsCell);
            map[x, leaf.bounds.zMin + zOffset + roomHeight] = Cell.CreateCell(roomWallsCell);
        }
        for (int z = leaf.bounds.zMin + zOffset; z <= leaf.bounds.zMin + zOffset + roomHeight; z++)
        {
            map[leaf.bounds.xMin + xOffset, z] = Cell.CreateCell(roomWallsCell);
            map[leaf.bounds.xMin + xOffset + roomWidth, z] = Cell.CreateCell(roomWallsCell);
        }

        midpoints.Add(new Vector2(leaf.bounds.xMin + xOffset + roomWidth / 2, leaf.bounds.zMin + zOffset + roomHeight / 2));
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

    private static void BresenhamLine(ref Cell[,] map, int x0, int z0, int x1, int z1, CellType floorCell = CellType.Floor)
    {
        bool steep = false;
        if (System.Math.Abs(x0 - x1) < System.Math.Abs(z0 - z1)) // if line is steep => transpose. aka if (dx < dy) => go with for (int z = z0; ... So we finding out, if height > width
        {
            //x0 = x0.Swap(ref y0);
            //x1 = x1.Swap(ref y1);
            Swap(ref x0, ref z0);
            Swap(ref x1, ref z1);
            steep = true;
        }
        if (x0 > x1) // start always lower then end
        {
            Swap(ref x0, ref x1);
            Swap(ref z0, ref z1);
        }

        // line formula: y - y0 = (y1-y0) / (x1-x0) * (x - x0) => y = (y1-y0) / (x1-x0) * (x - x0) + y0 => we know that x = x_previous + 1, so: y = y_prev + steep, where steep = (y1-y0)/(x1-x0)
        // steep = dy / dx = (z1-z0)/(x1-x0)
        //
        // y = y1 * (x-x0)/(x1-x0) - y0 * (x-x0)/(x1-x0) + y0
        // y = y1 * t - y0 * y + y0
        // y = y1 * t + y0 * (1 - t)
        //
        // x = x0; x<=x1; x++
        // float t = (x-x0)/(float)(x1-x0); 
        // int y = y0 * (1.0f - t) + y1 * t;
        //
        //then: make x1-x0 int, same with y
        // int dx = x1 - x0;
        // int dy = y1 - y0;
        // find error: float derr = abs(dy/float(dx));
        // accumulate error: float err = 0;
        // if for loop: err += derr; if (err>0.5) { y += 1; (or if y1 < y0 y += -1);  err -= 1.0}
        //
        // now replace error: derr = abs(dy);
        // err = 0;
        // in for:      err += derr;  if (err > 0.5 * dx) => err -= dx;

        int deltaX = x1 - x0;
        int deltaZ = z1 - z0;
        //  значение ошибки = расстояние между текущим значением y и точным значением y для текущего x
        int deltaError = System.Math.Abs(deltaZ) * 2; //2 times => we don't need to do if (err > 0.5)
        int err = 0;
        int z = z0; // initialize

        for (int x = x0; x <= x1; x++) // мы увеличиваем x = мы увеличиваем значение ошибки на величину наклона s
        {
            if (steep)
            {
                map[z, x] = Cell.CreateCell(floorCell);
            }
            else
            {
                map[x, z] = Cell.CreateCell(floorCell);
            }

            err += deltaError; // accumulate error
            if (err > deltaX) // ошибка превысила порог, линия стала ближе/дальше к следующему y
            {
                //z += (z1 > z0 ? 1 : -1);
                z += System.Math.Sign(deltaZ);
                err -= deltaX * 2; // replace
            }
        }

        // From Wiki:
        // In practice, the algorithm does not keep track of the y coordinate, which increases by m = ∆y /∆x each time the x increases by one; it keeps an error bound at each stage, 
        // which represents the negative of the distance from(a) the point where the line exits the pixel to(b) the top edge of the pixel. This value is first set to m − 0.5
        // (due to using the pixel's center coordinates), and is incremented by m each time the x coordinate is incremented by one. If the error becomes greater than 0.5, 
        // we know that the line has moved upwards one pixel, and that we must increment our y coordinate and readjust the error to represent the distance from the top of the new pixel – 
        // which is done by subtracting one from error.
    }

    private static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    static void MidpointCircle(ref Cell[,] map, int centerX, int centerZ, int radius, CellType floorCell = CellType.Floor)
    {
        //int x = radius - 1;
        //int y = 0;
        //int dx = 1;
        //int dy = 1;
        //int err = dx - (radius << 1);

        //while (x >= y)
        //{
        //    map[centerX + x, centerZ + y] = Cell.CreateCell(floorCell);
        //    map[centerX + y, centerZ + x] = Cell.CreateCell(floorCell);
        //    map[centerX - y, centerZ + x] = Cell.CreateCell(floorCell);
        //    map[centerX - x, centerZ + y] = Cell.CreateCell(floorCell);

        //    map[centerX - x, centerZ - y] = Cell.CreateCell(floorCell);
        //    map[centerX - y, centerZ - x] = Cell.CreateCell(floorCell);
        //    map[centerX + y, centerZ - x] = Cell.CreateCell(floorCell);
        //    map[centerX + x, centerZ - y] = Cell.CreateCell(floorCell);

        //    if (err <= 0)
        //    {
        //        y++;
        //        err += dy;
        //        dy += 2;
        //    }

        //    if (err > 0)
        //    {
        //        x--;
        //        dx += 2;
        //        err += dx - (radius << 1);
        //    }
        //}

        int x = 0;
        int z = radius;
        int delta = 1 - 2 * radius;
        int error = 0;
        while (z >= 0)
        {
            map[centerX + x, centerZ + z] = Cell.CreateCell(floorCell);
            map[centerX + x, centerZ - z] = Cell.CreateCell(floorCell);
            map[centerX - x, centerZ + z] = Cell.CreateCell(floorCell);
            map[centerX - x, centerZ - z] = Cell.CreateCell(floorCell);
            error = 2 * (delta + z) - 1;
            if ((delta < 0) && (error <= 0))
            {
                delta += 2 * ++x + 1;
                continue;
            }
            error = 2 * (delta - x) - 1;
            if ((delta > 0) && (error > 0))
            {
                delta += 1 - 2 * --z;
                continue;
            }
            x++;
            delta += 2 * (x - z);
            z--;
        }
    }
}

//static class SwapExtension
//{
//    public static T Swap<T>(this T x, ref T y)
//    {
//        T t = y;
//        y = x;
//        return t;
//    }
//}

