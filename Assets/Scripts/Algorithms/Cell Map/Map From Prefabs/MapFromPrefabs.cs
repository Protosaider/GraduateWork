using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum ERooms
//{
//    X = 'X',
//    O = 'O',
//    RL = '─',
//    UD = '│',
//    RD = '┌',
//    DL = '┐',
//    UR = '└',
//    UL = '┘',
//    URD = '├',
//    UDL = '┤',
//    RDL = '┬',
//    URL = '┴',
//    URDL = '┼',  
//};


//▲►▼◄ ■        – dead ends
//─│┌┐└┘├┤┬┴┼   - corridors
//╒╓╔           - room corner LeftTop, opening left, top, none
//╕╖╗           - room corner RightTop, opening right, top, none
//╘╙╚           - room corner LeftBottom, opening left, bottom, none
//╛╜╝           - room corner RightBottom, opening right, bottom, none
//╟╠            - room left wall, opening left, none
//╢╣            - room right wall, opening right, none
//╤╦            - room top wall, opening top, none
//╧╩            - room bottom wall, opening bottom, none
//═║ ╪ ╫ ╬      - inner room walls, opening TopBottom, LeftRight, none

//█ ▓ □         - room floor


    /// 
    // TODO: Redo all for RoomWithCenteredDoors class (instead of characters). It's important!
    /// 

public class MapFromPrefabs {

   // private GridMapFromPrefabsSettings settings;
   // RoomWithCenteredDoors[,] roomGrid = new RoomWithCenteredDoors[settings.gridWidth, settings.gridHeight];

    public int mapRows;
    public int mapColumns;

    public char[,] map;

    private string boxCharacters;
    private string prefabCharacters;
    private string[] boxCharacterUpFriends;
    private string[] boxCharacterDownFriends;
    private string[] boxCharacterLeftFriends;
    private string[] boxCharacterRightFriends;

    private List<Room> rooms;

    private List<char> hasTopLeftWall;
    private List<char> hasTopCenterWall;
    private List<char> hasTopRightWall;
    private List<char> hasMiddleLeftWall;
    private List<char> hasMiddleCenterWall;
    private List<char> hasMiddleRightWall;
    private List<char> hasBottomLeftWall;
    private List<char> hasBottomCenterWall;
    private List<char> hasBottomRightWall;

    public void LogOutputMap()
    {
        string output = "";
        for (int c = mapColumns - 1; c >= 0; c--)
        //for (int r = 0; r < mapRows; r++)
        {
            for (int r = 0; r < mapRows; r++)
            //for (int c = 0; c < mapColumns; c++)
            {
                //output += map[r, c];
                output += map[r, c];
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    public void InitializeMap(ref GridMapFromPrefabsSettings settings)
    {
        InitializeCharacters();
        settings.prefabCharacters = prefabCharacters;

        mapRows = settings.gridWidth + 2;
        mapColumns = settings.gridHeight + 2;

        map = new char[mapRows, mapColumns];

        // Put 'X' (walls) in top and bottom rows.
        for (int c = 0; c < mapColumns; c++)
        {
            map[0, c] = 'X';
            map[mapRows - 1, c] = 'X';
        }

        // Put 'X' (walls) in the left and right columns.
        for (int r = 0; r < mapRows; r++)
        {
            map[r, 0] = 'X';
            map[r, mapColumns - 1] = 'X';
        }

        // Set 'O' (empty) for the other map cells.
        for (int r = 1; r < mapRows - 1; r++)
        {
            for (int c = 1; c < mapColumns - 1; c++)
            {
                map[r, c] = 'O';
            }
        }

        TryToSpawnRooms(ref settings);

        for (int c = 1; c < mapColumns - 1; c++)
        {
            for (int r = 1; r < mapRows - 1; r++)
            {
                if (
                    map[r, c] == '╒' || map[r, c] == '╕' || map[r, c] == '╘' || map[r, c] == '╛' ||
                    map[r, c] == '╔' || map[r, c] == '╗' || map[r, c] == '╚' || map[r, c] == '╝' ||
                    map[r, c] == '╓' || map[r, c] == '╖' || map[r, c] == '╙' || map[r, c] == '╜' ||
                    map[r, c] == '╧' || map[r, c] == '╤' || map[r, c] == '╟' || map[r, c] == '╢' ||
                    map[r, c] == '╩' || map[r, c] == '╦' || map[r, c] == '╠' || map[r, c] == '╣' || map[r, c] == '▓'
                    )
                {
                    continue;
                }

                string validCharacters = GetValidBoxCharacters(r, c);
                map[r, c] = validCharacters[Random.Range(0, validCharacters.Length)];
            }
        }

        PlaceDeadEnds();

    }

    private void TryToSpawnRooms(ref GridMapFromPrefabsSettings settings)
    {
        rooms = new List<Room>(settings.roomsCount);
        int roomPlacementCount = 0;
        int roomsPlaced = 0;

        while (true)
        {
            bool isPlaced = CreateRoom(ref settings);
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
    }

    public bool[,] WalkThroughMap()
    {
        bool[,] visitedCells = new bool[mapRows, mapColumns];

        int curRow = Random.Range(1, mapRows - 1);
        int curCol = Random.Range(1, mapColumns - 1);
        //int curRow = Random.Range(1, mapRows - 2);
        //int curCol = Random.Range(1, mapColumns - 2);

        VisitCell(visitedCells, curRow, curCol);

        OutputVisitedCellsMap(visitedCells);

        return visitedCells;
    }

    private void OutputVisitedCellsMap(bool[,] visitedCells)
    {
        string output = "";

        for (int c = mapColumns - 1; c >= 0; c--)
        {
            for (int r = 0; r < mapRows; r++)
            {
                output += visitedCells[r, c] ? 'T' : 'F';
            }
            output += "\n";
        }

        //for (int c = 0; c < mapColumns; c++)
        //{
        //    output += 'X';
        //}
        //output += "\n";
        //for (int x = 1; x < mapRows - 1; x++)
        //{
        //    output += 'X';
        //    for (int y = 1; y < mapColumns - 1; y++)
        //    {
        //        output += visitedCells[x, y] ? 'T' : 'F';
        //    }
        //    output += "X\n";
        //}
        //for (int c = 0; c < mapColumns; c++)
        //{
        //    output += 'X';
        //}

        Debug.Log(output);
    }

    public void VisitCell(bool[,] visitedCells, int row, int col)
    {
        if (visitedCells[row, col])
        {
            return;
        }

        visitedCells[row, col] = !visitedCells[row, col];

        switch (map[row, col])
        {
            case '─':
            case '╟':
            case '╢':
                //VisitCell(visitedCells, row, col - 1);
                VisitCell(visitedCells, row - 1, col);
                //VisitCell(visitedCells, row, col + 1);
                VisitCell(visitedCells, row + 1, col);
                break;
            case '│':
            case '╤':
            case '╧':
                VisitCell(visitedCells, row, col - 1);
                //VisitCell(visitedCells, row - 1, col);
                VisitCell(visitedCells, row, col + 1);
                //VisitCell(visitedCells, row + 1, col);
                break;
            case '┌':
            case '╔':
                VisitCell(visitedCells, row, col - 1);
                //VisitCell(visitedCells, row, col + 1);
                VisitCell(visitedCells, row + 1, col);
                break;
            case '┐':
            case '╗':
                VisitCell(visitedCells, row, col - 1);
                //VisitCell(visitedCells, row + 1, col);
                VisitCell(visitedCells, row - 1, col);
                break;
            case '└':
            case '╚':
                VisitCell(visitedCells, row, col + 1);
                //VisitCell(visitedCells, row - 1, col);
                VisitCell(visitedCells, row + 1, col);
                break;
            case '┘':
            case '╝':
                VisitCell(visitedCells, row, col + 1);
                //VisitCell(visitedCells, row, col - 1);
                VisitCell(visitedCells, row - 1, col);
                break;
            case '├':
            case '╠':
            case '╓':
            case '╙':
                VisitCell(visitedCells, row, col + 1);
                //VisitCell(visitedCells, row - 1, col);
                VisitCell(visitedCells, row, col + 1);
                VisitCell(visitedCells, row + 1, col);
                break;
            case '┤':
            case '╣':
            case '╖':
            case '╜':
                VisitCell(visitedCells, row - 1, col);
                VisitCell(visitedCells, row, col - 1);
                VisitCell(visitedCells, row, col + 1);
                //VisitCell(visitedCells, row + 1, col);
                break;
            case '┬':
            case '╦':
            case '╕':
            case '╒':
                VisitCell(visitedCells, row - 1, col);
                //VisitCell(visitedCells, row, col + 1);
                VisitCell(visitedCells, row + 1, col);
                VisitCell(visitedCells, row, col - 1);
                break;
            case '┴':
            case '╩':
            case '╛':
            case '╘':
                VisitCell(visitedCells, row, col + 1);
                VisitCell(visitedCells, row - 1, col);
                VisitCell(visitedCells, row + 1, col);
                //VisitCell(visitedCells, row, col - 1);
                break;
            case '┼':

            case '▓':
                VisitCell(visitedCells, row - 1, col);
                VisitCell(visitedCells, row, col + 1);
                VisitCell(visitedCells, row + 1, col);
                VisitCell(visitedCells, row, col - 1);
                break;
            case 'O':
            case '▲':
            case '►':
            case '▼':
            case '◄':
                return; //deadend
            default:
                Debug.Log("Wrong cell: " + map[row, col] + " at " + row + "; " + col);
                return;
        }
    }


    private void PlaceDeadEnds()
    {
        for (int c = 1; c < mapColumns - 1; c++)
        {
            for (int r = 1; r < mapRows - 1; r++)
            {
                if (map[r, c] == 'O')
                {
                    if ("─┐┘┤┬┴┼╒╘╢".Contains(map[r + 1, c].ToString()))
                    //if ("─┐┘┤┬┴┼╒╘╢".Contains(map[r, c + 1].ToString()))
                    {
                        //Debug.Log("Right");
                        map[r, c] = '►';
                    }
                    else if ("─┌└├┬┴┼╕╛╟".Contains(map[r - 1, c].ToString()))
                    //else if ("─┌└├┬┴┼╕╛╟".Contains(map[r, c - 1].ToString()))
                    {
                        //Debug.Log("Left");
                        map[r, c] = '◄';
                    }
                    else if ("│┌┐├┤┬┼╤╓╖".Contains(map[r, c + 1].ToString()))
                    //else if ("│┌┐├┤┬┼╤╓╖".Contains(map[r - 1, c].ToString()))
                    {
                        //Debug.Log("Top");
                        map[r, c] = '▲';
                    }
                    else if ("│└┘├┤┴┼╧╙╜".Contains(map[r, c - 1].ToString()))
                    //else if ("│└┘├┤┴┼╧╙╜".Contains(map[r + 1, c].ToString()))
                    {
                        //Debug.Log("Bottom");
                        map[r, c] = '▼';
                    }
                    else
                    {
                        //Debug.Log("Nothing!");
                    }
                }
            }
        }
    }


    private string GetValidBoxCharacters(int row, int column)
    {
        string validCharacters = "";

        for (int i = 0; i < boxCharacters.Length; i++)
        {
            char ch = boxCharacters[i];

            if (
                //boxCharacterLeftFriends[i].Contains(map[row, column - 1].ToString()) &&
                //boxCharacterRightFriends[i].Contains(map[row, column + 1].ToString()) &&
                //boxCharacterUpFriends[i].Contains(map[row - 1, column].ToString()) &&
                //boxCharacterDownFriends[i].Contains(map[row + 1, column].ToString())
                boxCharacterUpFriends[i].Contains(map[row, column + 1].ToString()) &&
                boxCharacterDownFriends[i].Contains(map[row, column - 1].ToString()) &&
                boxCharacterLeftFriends[i].Contains(map[row - 1, column].ToString()) &&
                boxCharacterRightFriends[i].Contains(map[row + 1, column].ToString())
                )
            {
                validCharacters += ch.ToString();
            }
        }

        if (validCharacters.Length == 0)
        {
            validCharacters = "O";
        }

        return validCharacters;
    }

    private void InitializeCharacters()
    {
        boxCharacters = "─│┌┐└┘├┤┬┴┼";
        boxCharacterUpFriends = new string[boxCharacters.Length];
        boxCharacterDownFriends = new string[boxCharacters.Length];
        boxCharacterLeftFriends = new string[boxCharacters.Length];
        boxCharacterRightFriends = new string[boxCharacters.Length];

        prefabCharacters = boxCharacters + "╒╓╔" + "╕╖╗" + "╘╙╚" + "╛╜╝" + "╢╠" + "╟╣" + "╧╦" + "╤╩" + "▓" + "►◄▼▲";

        //  ─
        boxCharacterLeftFriends[0] = "O─└┌┴├┬┼╕╛╟";
        boxCharacterRightFriends[0] = "O─┐┘┴┬┤┼╒╘╢";
        boxCharacterUpFriends[0] = "OX─└┘┴╩╚╝╘╛";
        boxCharacterDownFriends[0] = "OX─┌┐┬╦╔╗╒╕";

        //  │
        boxCharacterLeftFriends[1] = "OX│┐┘┤╣╗╝╖╜";
        boxCharacterRightFriends[1] = "OX│┌└├╠╔╚╓╙";
        boxCharacterUpFriends[1] = "O│┌┐├┬┤┼╤╙╜";
        boxCharacterDownFriends[1] = "O│└┘├┴┤┼╧╓╖";
        // --==--

        //  ┌
        boxCharacterLeftFriends[2] = boxCharacterLeftFriends[1];
        boxCharacterRightFriends[2] = boxCharacterRightFriends[0];
        boxCharacterUpFriends[2] = boxCharacterUpFriends[0];
        boxCharacterDownFriends[2] = boxCharacterDownFriends[1];

        //  ┐
        boxCharacterLeftFriends[3] = boxCharacterLeftFriends[0];
        boxCharacterRightFriends[3] = boxCharacterRightFriends[1];
        boxCharacterUpFriends[3] = boxCharacterUpFriends[0];
        boxCharacterDownFriends[3] = boxCharacterDownFriends[1];

        //  └
        boxCharacterLeftFriends[4] = boxCharacterLeftFriends[1];
        boxCharacterRightFriends[4] = boxCharacterRightFriends[0];
        boxCharacterUpFriends[4] = boxCharacterUpFriends[1];
        boxCharacterDownFriends[4] = boxCharacterDownFriends[0];

        //  ┘
        boxCharacterLeftFriends[5] = boxCharacterLeftFriends[0];
        boxCharacterRightFriends[5] = boxCharacterRightFriends[1];
        boxCharacterUpFriends[5] = boxCharacterUpFriends[1];
        boxCharacterDownFriends[5] = boxCharacterDownFriends[0];
        // --==--

        //  ├
        boxCharacterLeftFriends[6] = boxCharacterLeftFriends[1];
        boxCharacterRightFriends[6] = boxCharacterRightFriends[0];
        boxCharacterUpFriends[6] = boxCharacterUpFriends[1];
        boxCharacterDownFriends[6] = boxCharacterDownFriends[1];

        //  ┤
        boxCharacterLeftFriends[7] = boxCharacterLeftFriends[0];
        boxCharacterRightFriends[7] = boxCharacterRightFriends[1];
        boxCharacterUpFriends[7] = boxCharacterUpFriends[1];
        boxCharacterDownFriends[7] = boxCharacterDownFriends[1];

        //  ┬
        boxCharacterLeftFriends[8] = boxCharacterLeftFriends[0];
        boxCharacterRightFriends[8] = boxCharacterRightFriends[0];
        boxCharacterUpFriends[8] = boxCharacterUpFriends[0];
        boxCharacterDownFriends[8] = boxCharacterDownFriends[1];

        //  ┴
        boxCharacterLeftFriends[9] = boxCharacterLeftFriends[0];
        boxCharacterRightFriends[9] = boxCharacterRightFriends[0];
        boxCharacterUpFriends[9] = boxCharacterUpFriends[1];
        boxCharacterDownFriends[9] = boxCharacterDownFriends[0];

        //  ┼
        boxCharacterLeftFriends[10] = boxCharacterLeftFriends[0];
        boxCharacterRightFriends[10] = boxCharacterRightFriends[0];
        boxCharacterUpFriends[10] = boxCharacterUpFriends[1];
        boxCharacterDownFriends[10] = boxCharacterDownFriends[1];

       

        hasTopLeftWall = new List<char>();
        hasTopLeftWall.AddRange(boxCharacters.ToCharArray());
        hasTopLeftWall.Add('╔');
        hasTopLeftWall.Add('╒');
        hasTopLeftWall.Add('╓');

        hasTopLeftWall.Add('╗');
        hasTopLeftWall.Add('╕');
        hasTopLeftWall.Add('╖');

        hasTopLeftWall.Add('╘');
        hasTopLeftWall.Add('╙');
        hasTopLeftWall.Add('╚');

        hasTopLeftWall.Add('╢');
        hasTopLeftWall.Add('╠');
        hasTopLeftWall.Add('╧');
        hasTopLeftWall.Add('╦');

        hasTopLeftWall.Add('►');
        hasTopLeftWall.Add('◄');
        hasTopLeftWall.Add('▼');
        hasTopLeftWall.Add('▲');
        hasTopLeftWall.Add('O');


        hasTopCenterWall = new List<char>();
        hasTopCenterWall.Add('─');
        hasTopCenterWall.Add('┌');
        hasTopCenterWall.Add('┐');
        hasTopCenterWall.Add('┬');

        hasTopCenterWall.Add('╔');
        hasTopCenterWall.Add('╒');

        hasTopCenterWall.Add('╗');
        hasTopCenterWall.Add('╕');

        hasTopCenterWall.Add('╦');

        hasTopCenterWall.Add('►');
        hasTopCenterWall.Add('◄');
        hasTopCenterWall.Add('▼');

        hasTopCenterWall.Add('O');

        hasTopRightWall = new List<char>();
        hasTopRightWall.AddRange(boxCharacters.ToCharArray());

        hasTopRightWall.Add('╒');
        hasTopRightWall.Add('╓');
        hasTopRightWall.Add('╔');

        hasTopRightWall.Add('╕');
        hasTopRightWall.Add('╖');
        hasTopRightWall.Add('╗');

        hasTopRightWall.Add('╛');
        hasTopRightWall.Add('╜');
        hasTopRightWall.Add('╝');

        hasTopRightWall.Add('╟');
        hasTopRightWall.Add('╣');
        hasTopRightWall.Add('╧');
        hasTopRightWall.Add('╦');

        hasTopRightWall.Add('►');
        hasTopRightWall.Add('◄');
        hasTopRightWall.Add('▼');
        hasTopRightWall.Add('▲');

        hasTopRightWall.Add('O');



        hasMiddleLeftWall = new List<char>();
        hasMiddleLeftWall.Add('│');
        hasMiddleLeftWall.Add('┌');
        hasMiddleLeftWall.Add('└');
        hasMiddleLeftWall.Add('├');

        hasMiddleLeftWall.Add('╓');
        hasMiddleLeftWall.Add('╔');

        hasMiddleLeftWall.Add('╙');
        hasMiddleLeftWall.Add('╚');

        hasMiddleLeftWall.Add('╠');

        hasMiddleLeftWall.Add('►');
        hasMiddleLeftWall.Add('▼');
        hasMiddleLeftWall.Add('▲');

        hasMiddleLeftWall.Add('O');


        hasMiddleCenterWall = new List<char>('O');


        hasMiddleRightWall = new List<char>();
        hasMiddleRightWall.Add('│');
        hasMiddleRightWall.Add('┐');
        hasMiddleRightWall.Add('┘');
        hasMiddleRightWall.Add('┤');

        hasMiddleRightWall.Add('╖');
        hasMiddleRightWall.Add('╗');

        hasMiddleRightWall.Add('╜');
        hasMiddleRightWall.Add('╝');

        hasMiddleRightWall.Add('╣');

        hasMiddleRightWall.Add('◄');
        hasMiddleRightWall.Add('▼');
        hasMiddleRightWall.Add('▲');


        hasMiddleRightWall.Add('O');



        hasBottomLeftWall = new List<char>();

        hasBottomLeftWall.AddRange(boxCharacters.ToCharArray());

        hasBottomLeftWall.Add('╒');
        hasBottomLeftWall.Add('╓');
        hasBottomLeftWall.Add('╔');

        hasBottomLeftWall.Add('╘');
        hasBottomLeftWall.Add('╙');
        hasBottomLeftWall.Add('╚');

        hasBottomLeftWall.Add('╛');
        hasBottomLeftWall.Add('╜');
        hasBottomLeftWall.Add('╝');

        hasBottomLeftWall.Add('╢');
        hasBottomLeftWall.Add('╠');
        hasBottomLeftWall.Add('╤');
        hasBottomLeftWall.Add('╩');

        hasBottomLeftWall.Add('►');
        hasBottomLeftWall.Add('◄');
        hasBottomLeftWall.Add('▼');
        hasBottomLeftWall.Add('▲');


        hasBottomLeftWall.Add('O');



        hasBottomCenterWall = new List<char>();
        hasBottomCenterWall.Add('─');
        hasBottomCenterWall.Add('└');
        hasBottomCenterWall.Add('┘');
        hasBottomCenterWall.Add('┴');

        hasBottomCenterWall.Add('╘');
        hasBottomCenterWall.Add('╚');

        hasBottomCenterWall.Add('╛');
        hasBottomCenterWall.Add('╝');

        hasBottomCenterWall.Add('╩');

        hasBottomCenterWall.Add('►');
        hasBottomCenterWall.Add('◄');
        hasBottomCenterWall.Add('▲');

        hasBottomCenterWall.Add('O');


        hasBottomRightWall = new List<char>();
        hasBottomRightWall.AddRange(boxCharacters.ToCharArray());

        hasBottomRightWall.Add('╕');
        hasBottomRightWall.Add('╖');
        hasBottomRightWall.Add('╗');

        hasBottomRightWall.Add('╛');
        hasBottomRightWall.Add('╜');
        hasBottomRightWall.Add('╝');

        hasBottomRightWall.Add('╘');
        hasBottomRightWall.Add('╙');
        hasBottomRightWall.Add('╚');

        hasBottomRightWall.Add('╟');
        hasBottomRightWall.Add('╣');
        hasBottomRightWall.Add('╤');
        hasBottomRightWall.Add('╩');

        hasBottomRightWall.Add('►');
        hasBottomRightWall.Add('◄');
        hasBottomRightWall.Add('▼');
        hasBottomRightWall.Add('▲');

        hasBottomRightWall.Add('O');

    }

    public bool DoesPrefabHaveAWall(ref char prefab, CleanUpCells pos)
    {
        switch (pos)
        {
            case CleanUpCells.TopLeft:
                return hasTopLeftWall.Contains(prefab);

            case CleanUpCells.TopCenter:
                return hasTopCenterWall.Contains(prefab);
                //return false;

            case CleanUpCells.TopRight:
                return hasTopRightWall.Contains(prefab);
                //return false;

            case CleanUpCells.MiddleLeft:
                return hasMiddleLeftWall.Contains(prefab);
                //return false;

            case CleanUpCells.MiddleCenter:
                return hasMiddleCenterWall.Contains(prefab);
                //return false;

            case CleanUpCells.MiddleRight:
                return hasMiddleRightWall.Contains(prefab);
                //return false;

            case CleanUpCells.BottomLeft:
                return hasBottomLeftWall.Contains(prefab);
                //return false;

            case CleanUpCells.BottomCenter:
                return hasBottomCenterWall.Contains(prefab);
                //return false;

            case CleanUpCells.BottomRight:
                return hasBottomRightWall.Contains(prefab);
                //return false;

            case CleanUpCells.None:
                return false;

            default:
                Debug.LogError("Wrong CleanUpCell type");
                return false;
        }
    }

    private bool CreateRoom(ref GridMapFromPrefabsSettings settings)
    {
        int roomWidthSize = Random.Range(settings.roomMinWidth, settings.roomMaxWidth);
        //int roomWidthSize = Random.Range(2, 5);
        int roomHeightSize = Random.Range(settings.roomMinHeight, settings.roomMaxHeight);
        //int roomHeightSize = Random.Range(2, 5);

        int startRow = Random.Range(2, mapRows - 1 - roomWidthSize);
        int startColumn = Random.Range(2, mapColumns - 1 - roomHeightSize);

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

        // Corners
        map[startRow, startColumn] = '╚';
        map[startRow + roomWidth, startColumn] = '╝';
        map[startRow, startColumn + roomHeight] = '╔';
        map[startRow + roomWidth, startColumn + roomHeight] = '╗';

        // Top and Bottom
        for (int i = 1; i < roomWidth; i++)
        {
            map[startRow + i, startColumn + roomHeight] = '╦';
            map[startRow + i, startColumn] = '╩';
        }

        // Left and Right
        for (int i = 1; i < roomHeight; i++)
        {
            map[startRow, startColumn + i] = '╠';
            map[startRow + roomWidth, startColumn + i] = '╣';
        }

        // Set '▓' for the other spaces (which means 'floor').
        for (int i = 1; i < roomHeight; i++)
        {
            for (int j = 1; j < roomWidth; j++)
            {
                map[startRow + j, startColumn + i] = '▓';
            }
        }

        for (int i = 0; i < settings.maxRandomEntrances; i++)
        {
            if (Random.value < randomEntranceCreationChance)
            {
                randomEntranceCreationChance -= settings.randomEntranceCreationChanceSubtrahend;
                continue;
            }

            int entranceSide = Random.Range(0, 4);

            if (entranceSide % 2 == 0) //even = Top Bottom
            {
                int entranceColumn = Random.Range(0, roomWidthSize);
                if (entranceSide == 0) //Top
                {
                    if (entranceColumn == 0)
                    {
                        map[startRow + entranceColumn, startColumn + roomHeight] = '╓';
                    }
                    else if (entranceColumn == roomWidth)
                    {
                        map[startRow + entranceColumn, startColumn + roomHeight] = '╖';
                    }
                    else
                    {
                        map[startRow + entranceColumn, startColumn + roomHeight] = '╧';
                    }
                }
                else //2
                {
                    if (entranceColumn == 0)
                    {
                        map[startRow + entranceColumn, startColumn] = '╙';
                    }
                    else if (entranceColumn == roomWidth)
                    {
                        map[startRow + entranceColumn, startColumn] = '╜';
                    }
                    else
                    {
                        map[startRow + entranceColumn, startColumn] = '╤';
                    }
                }
            }
            else
            {
                int entranceRow = Random.Range(0, roomHeightSize);
                if (entranceSide == 1) //Right
                {
                    if (entranceRow == 0)
                    {
                        map[startRow + roomWidth, startColumn + entranceRow] = '╛';
                    }
                    else if (entranceRow == roomHeight)
                    {
                        map[startRow + roomWidth, startColumn + entranceRow] = '╕';
                    }
                    else
                    {
                        map[startRow + roomWidth, startColumn + entranceRow] = '╟';
                    }
                }
                else //3
                {
                    if (entranceRow == 0)
                    {
                        map[startRow, startColumn + entranceRow] = '╘';
                    }
                    else if (entranceRow == roomHeight)
                    {
                        map[startRow, startColumn + entranceRow] = '╒';
                    }
                    else
                    {
                        map[startRow, startColumn + entranceRow] = '╢';
                    }
                }
            }
        }

        return true;
    }
}
