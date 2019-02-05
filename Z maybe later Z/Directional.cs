using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Directional : MonoBehaviour {

    private int[,] map;
    private bool goBackwards = false;
    private DirectionalSettings settings;
    public Coordinate startTile = new Coordinate(40, 5);

    void GenerateMap(DirectionalSettings settings)
    {
        if (settings.mapWidth < 6)
        {
            settings.mapWidth = 6;
        }

        map = new int[settings.mapWidth, settings.mapHeight];

        if (settings.canGoBackwards)
        {
            goBackwards = false;
        }

        if (settings.useDefaultWidthRange)
        {
            settings.minWidth = 3;
            settings.maxWidth = settings.mapWidth - 2;
        }
        else
        {
            if (settings.minWidth < 3)
            {
                settings.minWidth = 3;
            }

            if (settings.maxWidth > settings.mapWidth - 2)
            {
                settings.maxWidth = settings.mapWidth - 2;
            }

            if (settings.maxWidth < 3)
            {
                settings.maxWidth = 3;
            }

            if (settings.minWidth > settings.maxWidth)
            {
                settings.minWidth = settings.maxWidth;
            }
        }

        if (!settings.useAutoLineWidthRange)
        {
            if (settings.lineWidthRange < 1)
            {
                settings.lineWidthRange = 1;
            }

            if (settings.lineWidthRange > settings.maxWidth - settings.minWidth)
            {
                settings.lineWidthRange = settings.maxWidth - settings.minWidth;
            }
        }

        if (!settings.useAutoPositioningRange)
        {
            if (settings.positioningRange < 1)
            {
                settings.positioningRange = 1;
            }

            if (settings.positioningRange > settings.mapWidth / 2)
            {
                settings.positioningRange = settings.mapWidth / 2;
            }
        }

        InitialFillMap();
 
        DirectionalAlgorithm(startTile.x, startTile.z);
    }

    void ChangeGoBackwards()
    {
        goBackwards = !goBackwards;
    }

    void InitialFillMap()
    {
        //for (int x = 0; x < mapWidth; x++)
        //{
        //    for (int y = 0; y < mapHeight; y++)
        //    {
        //        map[x, y] = 1; //wall
        //    }
        //}
    }

    void DirectionalAlgorithm(int startX, int startY)
    {
        if (settings.directionHorizontal)
        {
            DirectionalAlgorithmHorizontal(startX, startY);
        }
        else
        {
            DirectionalAlgorithmVertical(startX, startY);
        }
    }

    void DirectionalAlgorithmVertical(int startX, int startY)
    {
        int x = startX;
        int y = startY;

        int roughWidth;
        int windy;
        int xCarve;

        int width = Random.Range(settings.minWidth, settings.maxWidth);
        int lastWidth = width;
        //place a room, if you like

        for (int i = 0; i < settings.pathLength; i++)
        {
            lastWidth = width;

            for (int j = 0; j < width; j++)
            {
                xCarve = x + j;
                if (xCarve > settings.mapWidth - 1)
                {
                    break;
                }
                map[xCarve, y] = 0;
            }

            if (!goBackwards)
            {
                y++;
                if (y > settings.mapHeight - 1)
                {
                    y = settings.mapHeight - 1;
                    break;
                }
            }
            else
            {
                y--;
                if (y < 1)
                {
                    y = 1;
                    break;
                }
            }

            if (Random.Range(1, 100) < settings.changeCellLineWidthChance)
            {

                do
                {
                    if (!settings.useAutoLineWidthRange)
                    {
                        roughWidth = Random.Range(-settings.lineWidthRange, settings.lineWidthRange);
                    }
                    else
                    {
                        roughWidth = Random.Range(-width, width);
                    }
                } while (roughWidth == 0);

                width += roughWidth;

                width = Mathf.Clamp(width, settings.minWidth, settings.maxWidth);
            }

            if (Random.Range(1, 100) < settings.chanceToChangePositioning)
            {
                do
                {
                    if (!settings.useAutoPositioningRange)
                    {
                        windy = Random.Range(-settings.positioningRange, settings.positioningRange);
                    }
                    else
                    {
                        windy = Random.Range(-width + 1, lastWidth - 1);
                    }
                } while (windy == 0);

                x += windy;
                x = Mathf.Clamp(x, 0, settings.mapWidth - width);
            }

            //smth else

        }

        if (settings.canGoBackwards && !goBackwards)
        {
            ChangeGoBackwards();
            DirectionalAlgorithm(x, y);
            return;
        }

        if (goBackwards)
        {
            ChangeGoBackwards();
            return;
        }
    }

    void DirectionalAlgorithmHorizontal(int startX, int startY)
    {
        int x = startX;
        int y = startY;

        int roughWidth;
        int windy;
        int yCarve;

        int width = Random.Range(settings.minWidth, settings.maxWidth);
        int lastWidth = width;
        //place a room, if you like

        for (int i = 0; i < settings.pathLength; i++)
        {
            lastWidth = width;

            for (int j = 0; j < width; j++)
            {
                yCarve = y + j;
                if (yCarve > settings.mapHeight - 1)
                {
                    break;
                }
                map[x, yCarve] = 0;
            }

            if (!goBackwards)
            {
                x++;
                if (x > settings.mapWidth - 1)
                {
                    x = settings.mapWidth - 1;
                    break;
                }
            }
            else
            {
                x--;
                if (x < 1)
                {
                    x = 1;
                    break;
                }
            }

            if (Random.Range(1, 100) < settings.changeCellLineWidthChance)
            {

                do
                {
                    if (!settings.useAutoLineWidthRange)
                    {
                        roughWidth = Random.Range(-settings.lineWidthRange, settings.lineWidthRange);
                    }
                    else
                    {
                        roughWidth = Random.Range(-width, width);
                    }
                } while (roughWidth == 0);

                width += roughWidth;

                width = Mathf.Clamp(width, settings.minWidth, settings.maxWidth);
            }

            if (Random.Range(1, 100) < settings.chanceToChangePositioning)
            {
                do
                {
                    if (!settings.useAutoPositioningRange)
                    {
                        windy = Random.Range(-settings.positioningRange, settings.positioningRange);
                    }
                    else
                    {
                        windy = Random.Range(-width + 1, lastWidth - 1);
                    }
                } while (windy == 0);

                y += windy;
                y = Mathf.Clamp(y, 0, settings.mapHeight - width);
            }

            //smth else

        }

        if (settings.canGoBackwards && !goBackwards)
        {
            ChangeGoBackwards();
            DirectionalAlgorithm(x, y);
            return;
        }

        if (goBackwards)
        {
            ChangeGoBackwards();
            return;
        }
    }

}
