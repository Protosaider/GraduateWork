using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlingRoomsCreator
{
    public List<DelaunayMSTRoom> rooms = new List<DelaunayMSTRoom>();
    public List<Delaunay.Geo.LineSegment> delaunayLines = new List<Delaunay.Geo.LineSegment>();
    public List<Delaunay.Geo.LineSegment> spanningTree = new List<Delaunay.Geo.LineSegment>();

    public List<RoomsConnection> paths = new List<RoomsConnection>();

    private float averageRoomWidth = 0;
    private float averageRoomHeight = 0;

    //public delegate void LevelGenerationComplete();
    //public LevelGenerationComplete OnLevelGenerationComplete;

    //private float minX, minY, maxX, maxY;

    private int minX, minY, maxX, maxY;

    //public Vector2Int GetMinMaxX
    //{
    //    get
    //    {
    //        return new Vector2Int(minX, maxX);
    //    }
    //}
    //public Vector2Int GetMinMaxY
    //{
    //    get
    //    {
    //        return new Vector2Int(minY, maxY);
    //    }
    //}

    public int Width
    {
        get
        {
            return maxX - minX + 1;
        }
    }

    public int Height
    {
        get
        {
            return maxY - minY + 1;
        }
    }

    public SettlingRoomsSettings settings;

    public void GenerateMap(SettlingRoomsSettings settings)
    {
        this.settings = settings;

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

        SpawnOverlappingRooms();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Rooms spawned inside circle");
        }

        SettleOutRooms();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Roows settled out (separated from each other)");
        }

        PickMainRooms();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Main rooms selected");
        }

        TriangulateMainRoomsMidpointsAndGenerateMST();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Delaunay proceeded, minimal spanning tree found");
        }

        CreateAdditionalConnectionsBetweenRooms();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Added some additional paths (that cause creation of branches or loops)");
        }

        FindConnectedRoomsAndPathsBetweenThem();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Hallways created between rooms");
        }

        FindPathRoomsBetweenMainRooms();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Rooms that are overlapping passages added to spawn pool");
        }

        TranslateAllPointsCorrespondingToZeroOriginOfCoordinates();
        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Origin of coordinates set to (0, 0). All points translated accordingly to that.");
        }

        Random.state = initialState;
    }

    //public void GenerateLevel()
    //{
    //    StartCoroutine(Generate());
    //}
    //public IEnumerator Generate()
    //{
    //    // Random Generator
    //    Random.State initialState = Random.state;
    //    if (settings.useFixedSeed)
    //    {
    //        Random.InitState(settings.seed.GetHashCode());
    //    }
    //    else
    //    {
    //        Random.InitState(Time.time.ToString().GetHashCode());
    //    }

    //    SpawnOverlappingRooms();
    //    if (debug)
    //    {
    //        Debug.Log("Cells generated");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    SettleOutRooms();
    //    if (debug)
    //    {
    //        Debug.Log("Cells Separated");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    PickMainRooms();
    //    if (debug)
    //    {
    //        Debug.Log("Main rooms selected");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    TriangulateMainRoomsMidpointsAndGenerateMST();
    //    if (debug)
    //    {
    //        Debug.Log("Delaunay proceeded");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    CreateAdditionalConnectionsBetweenRooms();
    //    if (debug)
    //    {
    //        Debug.Log("Paths Selected");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    //AddRoomsMidpointsToConnectionLines();
    //    //if (debug)
    //    //{
    //    //    Debug.Log("Cell Lines");
    //    //    yield return new WaitForSeconds(1.0f);
    //    //}

    //    FindConnectedRoomsAndPathsBetweenThem();
    //    if (debug)
    //    {
    //        Debug.Log("RoomsConnection between blocks");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    FindPathRoomsBetweenMainRooms();
    //    if (debug)
    //    {
    //        Debug.Log("RoomsConnection between main rooms");
    //        yield return new WaitForSeconds(1.0f);
    //    }

    //    if (OnLevelGenerationComplete != null)
    //    {
    //        OnLevelGenerationComplete();
    //    }

    //    Random.state = initialState;

    //    yield return null;
    //}

    #region GenerationMethods

    private void SpawnOverlappingRooms()
    {
        averageRoomWidth = 0;
        averageRoomHeight = 0;

        for (int i = 0; i < settings.numberOfRoomsToSpawn; i++)
        {
            // place rooms randomly inside a circle
            Vector2 pos = UnityUtilities.Math.GetRandomPointInCircle(settings.roomSpawnCircleRadius);

            // use the normal distribution for generating room sizes
            // Picking different ratios between width/height mean and standard deviation will generally result in different looking dungeons
            // Round to Int - because we use grid
            DelaunayMSTRoom room = new DelaunayMSTRoom
            {
                width = Mathf.RoundToInt(UnityUtilities.RandomNormalDistribution.RandomRangeGaussian(settings.roomMinWidth, settings.roomMaxWidth, UnityUtilities.RandomNormalDistribution.ConfidenceInterval._80)),
                height = Mathf.RoundToInt(UnityUtilities.RandomNormalDistribution.RandomRangeGaussian(settings.roomMinHeight, settings.roomMaxHeight, UnityUtilities.RandomNormalDistribution.ConfidenceInterval._80)),               
                index = i,
                x = Mathf.RoundToInt(pos.x),
                y = Mathf.RoundToInt(pos.y)
            };

            averageRoomWidth += room.width;
            averageRoomHeight += room.height;

            if (settings.isDebugLogEnabled)
            {
                Debug.Log("Room width = " + room.width + " room height = " + room.height + " posX = " + room.x + " posY = " + room.y);
            }

            rooms.Add(room);
        }

        averageRoomWidth /= rooms.Count;
        averageRoomHeight /= rooms.Count;

        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Room Average width = " + averageRoomWidth + " average room height = " + averageRoomHeight);
        }

    }

    //separation steering behavior
    private void SettleOutRooms()
    {
        bool cellCollision = true;
        int loop = 0;

        int count = 0;

        while (cellCollision)
        {
            loop++;
            cellCollision = false;

            if (settings.isDebugLogEnabled)
            {
                Debug.Log("Loop " + loop);
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                DelaunayMSTRoom roomA = rooms[i];

                for (int j = i + 1; j < rooms.Count; j++)
                {
                    count++;

                    DelaunayMSTRoom roomB = rooms[j];

                    if (roomA.CollidesWith(roomB))
                    {                 
                        cellCollision = true;

                        int roomB_x = Mathf.RoundToInt((roomA.x + roomA.width) - roomB.x); //between rightmost point of cellA and leftmost point of cellB
                        int roomA_x = Mathf.RoundToInt((roomB.x + roomB.width) - roomA.x); //between leftmost point of cellA and rightmost point of cellB

                        int roomB_y = Mathf.RoundToInt((roomA.y + roomA.height) - roomB.y);
                        int roomA_y = Mathf.RoundToInt((roomB.y + roomB.height) - roomA.y);

                        if (settings.isDebugLogEnabled)
                        {
                            Debug.Log("cb_x = " + roomB_x + " cell_x = " + roomA_x + " cb_y = " + roomB_y + " cell_y = " + roomA_y);
                        }

                        // cellB inside cellA, cellB intruded from left or cellB intuded from bottom
                        //if (cellA_x < cellB_x) // stretched along x
                        if (roomA_x < roomB_x || roomA_y < roomB_y) // both axis
                        //if (cellA_y < cellB_y) // results: rooms expand along y axis
                        {
                            if (roomA_x < roomA_y) // horizontal or vertical movement
                            {
                                roomA.Shift(roomA_x, 0);
                            }
                            else
                            {
                                roomA.Shift(0, roomA_y);
                            }

                        }
                        else 
                        {
                            if (roomB_x < roomB_y)
                            {
                                roomB.Shift(roomB_x, 0);
                            }
                            else
                            {
                                roomB.Shift(0, roomB_y);
                            }
                        }
                    }
                }
            }
        }

        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Count " + count);
        }
    }

    //pick rooms that are above some width/height threshold
    private void PickMainRooms()
    {
        if (settings.isDebugLogEnabled)
        {
            int i = 0;
            foreach (var room in rooms)
            {
                //threshold * mean, where mean = width_mean or height_mean
                if ((room.width >= averageRoomWidth * settings.roomWidthMeanConversionToMainRoomThreshold && room.height >= averageRoomHeight * settings.roomHeightMeanConversionToMainRoomThreshold))
                {
                    room.isMainRoom = true;
                    Debug.Log("Main room selected. Width = " + room.width + " room height = " + room.height + " posX = " + room.x + " posY = " + room.y);
                    i++;
                }
            }
            Debug.Log("Main rooms count: " + i);
        }
        else
        {
            foreach (var room in rooms)
            {
                //threshold * mean, where mean = width_mean or height_mean
                if ((room.width >= averageRoomWidth * settings.roomWidthMeanConversionToMainRoomThreshold && room.height >= averageRoomHeight * settings.roomHeightMeanConversionToMainRoomThreshold))
                {
                    room.isMainRoom = true;
                }
            }
        }
    }

    // take all the midpoints of the selected rooms and feed that into the Delaunay procedure
    //  generate a graph, also it's good to have ID for rooms
    // and generate minimum spanning tree
    private void TriangulateMainRoomsMidpointsAndGenerateMST()
    {
        List<Vector2> midpoints = new List<Vector2>();
        List<uint> colors = new List<uint>(); // Colors are just necessary part for choosen delaunay library

        Vector2 min = Vector2.positiveInfinity;
        Vector2 max = Vector2.zero;

        for (int i = 0; i < rooms.Count; i++)
        {
            DelaunayMSTRoom room = rooms[i];
            if (room.isMainRoom)
            {
                colors.Add(0);
                midpoints.Add(new Vector2(room.x + room.width / 2, room.y + room.height / 2)); // use division (not multiplication) because we need INT value (or use RoundToInt)
                min.x = Mathf.Min(min.x, room.x);
                min.y = Mathf.Min(min.y, room.y);

                max.x = Mathf.Max(max.x, room.x);
                max.y = Mathf.Max(max.y, room.y);
            }
        }

        Delaunay.Voronoi voronoi = new Delaunay.Voronoi(midpoints, colors, new Rect(min.x, min.y, max.x, max.y));
        delaunayLines = voronoi.DelaunayTriangulation(); // Triangulate main rooms

        spanningTree = voronoi.SpanningTree(Delaunay.KruskalType.MINIMUM); //Find min. span. tree

        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Main passages count: " + spanningTree.Count);
        }
    }

    // To prevent creation of a dungeon that has only one linear path, we can add a few edges back from the Delaunay graph
    private void CreateAdditionalConnectionsBetweenRooms()
    {
        foreach (var pathBetweenRooms in spanningTree)
        {
            delaunayLines.Remove(pathBetweenRooms);
        }

        int countOfPathsToAdd = Mathf.RoundToInt(delaunayLines.Count * settings.percentsOfPathsAmountToAddFromDelaunayGraphToMST);
        int pathsAdded = 0;

        List<Delaunay.Geo.LineSegment> linesToAdd = new List<Delaunay.Geo.LineSegment>();

        for (int i = 0; i < delaunayLines.Count; i++)
        {
            if (pathsAdded >= countOfPathsToAdd)
            {
                break;
            }

            Delaunay.Geo.LineSegment line = delaunayLines[i];

            linesToAdd.Add(line);
            pathsAdded++;
        }

        //for (int i = 0; i < delaunayLines.Count; i++)
        //{
        //    if (pathsAdded >= countOfPathsToAdd)
        //    {
        //        break;
        //    }

        //    Delaunay.Geo.LineSegment line = delaunayLines[i];
        //    bool isLineExist = false;

        //    for (int j = 0; j < spanningTree.Count; j++)
        //    {
        //        Delaunay.Geo.LineSegment spanningTreeLine = spanningTree[j];
        //        if (spanningTreeLine.p0.Value.Equals(line.p0.Value) && spanningTreeLine.p1.Value.Equals(line.p1.Value))
        //        {
        //            isLineExist = true;
        //            break;
        //        }
        //    }

        //    if (!isLineExist)
        //    {
        //        linesToAdd.Add(line);
        //        pathsAdded++;
        //    }
        //}

        if (settings.isDebugLogEnabled)
        {
            Debug.Log("Passages added: " + linesToAdd.Count);
        }

        spanningTree.AddRange(linesToAdd);
        delaunayLines.Clear();
    }

    //private void AddRoomsMidpointsToConnectionLines()
    //{
    //    foreach (Delaunay.Geo.LineSegment line in spanningTree)
    //    {
    //        DelaunayMSTRoom roomStart = GetRoomByPoint(line.p0.Value.x, line.p0.Value.y);

    //        if (roomStart != null)
    //        {
    //            line.cellStart = roomStart;
    //        }
    //        else
    //        {
    //            Debug.LogError("Could not find cell start for " + line.p0.Value);
    //        }

    //        DelaunayMSTRoom roomEnd = GetRoomByPoint(line.p1.Value.x, line.p1.Value.y);

    //        if (roomEnd != null)
    //        {
    //            line.cellEnd = roomEnd;
    //        }
    //        else
    //        {
    //            Debug.LogError("Could not find cell end for " + line.p1.Value);
    //        }
    //    }
    //}

    private void FindConnectedRoomsAndPathsBetweenThem()
    {
        foreach (var line in spanningTree)
        {
            DelaunayMSTRoom roomStart = GetRoomByPoint(line.p0.Value.x, line.p0.Value.y);

            if (roomStart == null)
            {
                Debug.LogError("Could not find cell start for " + line.p0.Value);
            }

            DelaunayMSTRoom roomEnd = GetRoomByPoint(line.p1.Value.x, line.p1.Value.y);

            if (roomEnd == null)
            {
                Debug.LogError("Could not find cell end for " + line.p1.Value);
            }

            Vector2Int start = new Vector2Int((int)line.p0.Value.x, (int)line.p0.Value.y);
            Vector2Int end = new Vector2Int((int)line.p1.Value.x, (int)line.p1.Value.y);
            //Vector2 start = line.p0.Value;
            //Vector2 end = line.p1.Value;

            Line firstHallway = new Line
            {
                start = start,
                //blockPath.end = new Vector2(end.x, start.y);
                end = new Vector2Int(end.x, start.y)
            };

            Line secondHallway = new Line
            {
                start = firstHallway.end,
                end = end
            };

            RoomsConnection path = new RoomsConnection
            {
                from = roomStart,
                to = roomEnd
            };
            path.path.Add(firstHallway);
            path.path.Add(secondHallway);

            paths.Add(path);
        }
        spanningTree.Clear();
    }

    //private void FindPathBetweenBlocks()
    //{
    //    foreach (var line in spanningTree)
    //    {
    //        RoomsConnection path = new RoomsConnection();
    //        path.from = line.cellStart;
    //        path.to = line.cellEnd;

    //        Vector2 start = line.p0.Value;
    //        Vector2 end = line.p1.Value;

    //        Line blockPath = new Line();
    //        blockPath.start = start;
    //        blockPath.end = new Vector2(end.x, start.y);

    //        Line blockPath2 = new Line();
    //        blockPath2.start = blockPath.end;
    //        blockPath2.end = end;

    //        path.path.Add(blockPath);
    //        path.path.Add(blockPath2);
    //        paths.Add(path);
    //    }
    //    spanningTree.Clear();
    //}

    private void FindPathRoomsBetweenMainRooms()
    {
        // Find rooms which is crossed by hallways
        foreach (RoomsConnection connection in paths)
        {
            foreach (DelaunayMSTRoom room in rooms)
            {
                if (!room.isMainRoom && !room.isPathRoom)
                {
                    foreach (Line hallway in connection.path)
                    {
                        if (IsLineIntersectsRectangle(hallway, room) && Random.value < settings.chanceToAddPathRooms)
                        {
                            room.isPathRoom = true;
                            break;
                        }
                    }
                }
            }
        }

        //// Find corners of map and remove unnecessary rooms
        //for (int i = rooms.Count - 1; i >= 0; i--)
        //{
        //    DelaunayMSTRoom room = rooms[i];
        //    if (room.isMainRoom || room.isPathRoom)
        //    {
        //        minX = Mathf.Min(room.x, minX);
        //        minY = Mathf.Min(room.y, minY);
        //        maxX = Mathf.Max(room.x + room.width, maxX);
        //        maxY = Mathf.Max(room.y + room.height, maxY);
        //    }
        //    else
        //    {
        //        rooms.Remove(room);
        //    }
        //}
        //// translate rooms =>
        //foreach (DelaunayMSTRoom room in rooms)
        //{
        //    room.x += Mathf.CeilToInt(Mathf.Abs(minX));
        //    room.y += Mathf.CeilToInt(Mathf.Abs(minY));
        //    maxX = Mathf.Max(room.x + room.width, maxX);
        //    maxY = Mathf.Max(room.y + room.height, maxY);
        //}
        //foreach (RoomsConnection connection in paths)
        //{
        //    foreach (Line hallway in connection.path)
        //    {
        //        hallway.start.x += Mathf.Abs(minX);
        //        hallway.start.y += Mathf.Abs(minY);
        //        hallway.end.x += Mathf.Abs(minX);
        //        hallway.end.y += Mathf.Abs(minY);
        //    }
        //}
    }

    private void TranslateAllPointsCorrespondingToZeroOriginOfCoordinates()
    {
        // Find corners of map and remove unnecessary rooms
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            DelaunayMSTRoom room = rooms[i];

            if (!room.isMainRoom && !room.isPathRoom)
            {
                rooms.Remove(room);
            }
            else
            {
                minX = Mathf.Min(room.x, minX);
                minY = Mathf.Min(room.y, minY);
                maxX = Mathf.Max(room.x + room.width - 1, maxX);
                maxY = Mathf.Max(room.y + room.height - 1, maxY);
            }
        }

        // translate rooms => make starting point == 0, 0
        foreach (DelaunayMSTRoom room in rooms)
        {
            room.x -= minX;
            room.y -= minY;
        }
        // and connections between them
        foreach (RoomsConnection connection in paths)
        {
            foreach (Line hallway in connection.path)
            {
                hallway.start.x -= minX;
                hallway.start.y -= minY;

                hallway.end.x -= minX;
                hallway.end.y -= minY;
            }
        }

        maxX -= minX;
        maxY -= minY;
        minX = 0;
        minY = 0;
    }

    #endregion

    public void FillCellMap(ref Cell[,] map, CellType wallCell = CellType.Empty, CellType floorCell = CellType.Empty)
    {
        if (wallCell == CellType.Empty && floorCell == CellType.Empty)
        {
            Debug.LogError("Both cell types are CellType.Empty.");
            return;
        }

        // Spawn rooms

        for (int i = 0; i < rooms.Count; i++)
        {
            DelaunayMSTRoom room = rooms[i];

            for (int x = room.x; x < room.x + room.width; x++)
            {
                map[x, room.y] = Cell.CreateCell(wallCell);
                map[x, room.y + room.height - 1] = Cell.CreateCell(wallCell);
            }

            for (int z = room.y; z < room.y + room.height; z++)
            {
                map[room.x, z] = Cell.CreateCell(wallCell);
                map[room.x + room.width - 1, z] = Cell.CreateCell(wallCell);
            }

            for (int z = room.y + 1; z < room.y + room.height - 1; z++)
            {
                for (int x = room.x + 1; x < room.x + room.width - 1; x++)
                {
                    map[x, z] = Cell.CreateCell(floorCell);
                }
            }
        }

        // Spawn hallways
        for (int i = 0; i < paths.Count; i++)
        {
            RoomsConnection connection = paths[i];

            for (int j = 0; j < connection.path.Count; j++)
            {
                Line hallway = connection.path[j];

                int fromX = Mathf.Min(hallway.start.x, hallway.end.x);
                int toX = Mathf.Max(hallway.start.x, hallway.end.x);

                int fromZ = Mathf.Min(hallway.start.y, hallway.end.y);
                int toZ = Mathf.Max(hallway.start.y, hallway.end.y);

                for (int x = fromX; x <= toX; x++)
                {
                    for (int z = fromZ; z <= toZ; z++)
                    {
                        map[x, z] = Cell.CreateCell(floorCell);

                        AddPathWalls(x, z, ref map, wallCell, floorCell);
                    }
                }
            }  
        }
    }

    private void AddPathWalls(int x, int y, ref Cell[,] map, CellType wallCell = CellType.Empty, CellType floorCell = CellType.Empty)
    {
        for (int j = y - 1; j <= y + 1; j++)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (IsOutside(i, j) || (i == j && i == x))
                {
                    continue;
                }

                if (map[i, j].cellType == CellType.Empty)
                {
                    map[i, j] = Cell.CreateCell(wallCell);
                }
            }
        }
    }

    private bool IsOutside(int x, int y)
    {
        return (x < 0 || y < 0 || x >= maxX || y >= maxY);
    }

    //private void AddPathWalls(int x, int y, ref Cell[,] map, CellType wallCell = CellType.Empty, CellType floorCell = CellType.Empty)
    //{
    //    if (y < maxY)
    //    {       
    //        if (map[x, y + 1].cellType == CellType.Empty)
    //        {
    //            map[x, y + 1] = Cell.CreateCell(wallCell);
    //        }
    //    }
    //    if (x < maxX)
    //    {
    //        if (map[x + 1, y].cellType == CellType.Empty)
    //        {
    //            map[x + 1, y] = Cell.CreateCell(wallCell);
    //        }
    //    }
    //    if (y > 0)
    //    {
    //        if (map[x, y - 1].cellType == CellType.Empty)
    //        {
    //            map[x, y - 1] = Cell.CreateCell(wallCell);
    //        }
    //    }
    //    if (x > 0)
    //    {
    //        if (map[x - 1, y].cellType == CellType.Empty)
    //        {
    //            map[x - 1, y] = Cell.CreateCell(wallCell);
    //        }
    //    }
    //}

    #region Helpers

    public Vector2 GetMaxPoint()
    {
        return new Vector2(maxX, maxY);
    }

    public Vector2 GetMinPoint()
    {
        return new Vector2(minX, minY);
    }

    private DelaunayMSTRoom GetRoomByPoint(float x, float y)
    {
        DelaunayMSTRoom room = default(DelaunayMSTRoom);

        foreach (var r in rooms)
        {
            if (r.x < x && r.y < y && r.x + r.width > x && r.y + r.height > y)
            {
                return r;
            }
        }

        return room;
    }

    private bool IsLineIntersectsRectangle(Line line, DelaunayMSTRoom room)
    {
        Vector2 intersection;

        Vector2 bottomLeft = new Vector3(room.x, room.y);
        Vector2 bottomRight = new Vector3(room.x + room.width, room.y);
              
        Vector2 topLeft = new Vector3(room.x, room.y + room.height);
        Vector2 topRight = new Vector3(room.x + room.width, room.y + room.height);

        if (UnityUtilities.Math.IsLineIntersects(line.start, line.end, bottomLeft, bottomRight, out intersection))
            return true;
        if (UnityUtilities.Math.IsLineIntersects(line.start, line.end, bottomLeft, topLeft, out intersection))
            return true;
        if (UnityUtilities.Math.IsLineIntersects(line.start, line.end, topRight, topLeft, out intersection))
            return true;
        if (UnityUtilities.Math.IsLineIntersects(line.start, line.end, topRight, bottomRight, out intersection))
            return true;

        return false;
    }

    #endregion
}

public class RoomsConnection
{
    public DelaunayMSTRoom from;
    public DelaunayMSTRoom to;

    public List<Line> path = new List<Line>();
}

public class Line
{
    //public Vector2 start;
    //public Vector2 end;
    public Vector2Int start;
    public Vector2Int end;

    public Line() { }

    public Line(Vector2Int start, Vector2Int end)
    {
        this.start = start;
        this.end = end;
    }

    public Line(Vector2 start, Vector2 end)
    {
        this.start = new Vector2Int(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
        this.end = new Vector2Int(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));
    }
}