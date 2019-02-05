using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidewinder  {

    public static T CreateMaze<T>(T grid, MazeSidewinderSettings settings) where T : MazeGrid
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

        Direction vertical = settings.verticalCarveDirectionNorth ? (Direction.North) : (Direction.South);
        Direction horizontal = settings.horizontalCarveDirectionEast ? (Direction.East) : (Direction.West);

        foreach (MazeCell[] cellRow in grid.EachRow(vertical, horizontal))
        {
            List<MazeCell> row = new List<MazeCell>();

            foreach (MazeCell cell in cellRow)
            {
                if (cell == null)
                {
                    continue;
                }

                row.Add(cell);

                //check if is out of width / height to avoid carve walls
                bool atVerticalBoundary = settings.verticalCarveDirectionNorth ? (cell.North == null) : (cell.South == null);
                bool atHorizontalBoundary = settings.horizontalCarveDirectionEast ? (cell.East == null) : (cell.West == null);
                bool carveMainDirection;

                if (settings.isVerticalDirectionMain)
                {
                    carveMainDirection = atHorizontalBoundary || (!atVerticalBoundary && Random.value < settings.chanceToCarveOutMain);
                }
                else
                {
                    carveMainDirection = atVerticalBoundary || (!atHorizontalBoundary && Random.value < settings.chanceToCarveOutMain);
                }

                if (carveMainDirection)
                {
                    MazeCell member = row[Random.Range(0, row.Count)]; //get random cell in a row

                    if (settings.isVerticalDirectionMain)
                    {
                        if (settings.verticalCarveDirectionNorth)
                        {
                            if (member.North != null) //if has neighbour at North => carve out wall
                            {
                                member.Link(member.North);
                            }
                        }
                        else
                        {
                            if (member.South != null)
                            {
                                member.Link(member.South);
                            }
                        }
                    }
                    else
                    {
                        if (settings.horizontalCarveDirectionEast)
                        {
                            if (member.East != null)
                            {
                                member.Link(member.East);
                            }
                        }
                        else
                        {
                            if (member.West != null)
                            {
                                member.Link(member.West);
                            }
                        }
                    }
                    
                    row.Clear(); // Clear row
                }
                else // if not carve north - break walls in horizontal axis between cells on row
                {
                    if (settings.isVerticalDirectionMain)
                    {
                        if (settings.horizontalCarveDirectionEast)
                        {
                            cell.Link(cell.East);
                        }
                        else
                        {
                            cell.Link(cell.West);
                        }
                    }
                    else
                    {
                        if (settings.verticalCarveDirectionNorth)
                        {
                            cell.Link(cell.North);
                        }
                        else
                        {
                            cell.Link(cell.South);
                        }
                    }
                }
            }
        }

        Random.state = initialState;

        return grid;
    }
}
