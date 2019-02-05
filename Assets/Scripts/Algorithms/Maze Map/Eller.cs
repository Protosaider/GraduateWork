using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Eller {


    private class RowState
    {
        public List<Dictionary<MazeCell, int>> setsForCells;
        public List<List<MazeCell>> setsKeys;
        public int nextSet;

        public RowState(MazeGrid grid, int startingSet = 0)
        {
            nextSet = startingSet;

            setsForCells = new List<Dictionary<MazeCell, int>>();
            setsKeys = new List<List<MazeCell>>();
            foreach (var row in grid.EachRow())
            {
                Dictionary<MazeCell, int> dictionary = new Dictionary<MazeCell, int>();
                foreach (var cell in row)
                {
                    int set = -1;
                    dictionary.Add(cell, set);
                }
                setsForCells.Add(dictionary);
                setsKeys.Add(new List<MazeCell>(dictionary.Keys));
            }
        }

        public void Record(int row, int set, MazeCell cell)
        {
            setsForCells[row][cell] = set;
        }

        public int SetFor(int row, MazeCell cell)
        {
            if (setsForCells[row][cell] == -1)
            {
                Record(row, nextSet, cell);
                nextSet++;
            }
            return setsForCells[row][cell];
        }

        public List<int> SetsInRow(int row)
        {
            List<int> setsInRow = new List<int>();
            foreach (var set in setsForCells[row].Values)
            {
                if (!setsInRow.Contains(set))
                {
                    setsInRow.Add(set);
                }
            }
            return setsInRow;
        }

        public List<MazeCell> CellsInSetInRow(int row, int set)
        {
            List<MazeCell> cellsInSetInRow = new List<MazeCell>();
            foreach (var cell in setsForCells[row].Keys)
            {
                if (setsForCells[row][cell] == set)
                {
                    cellsInSetInRow.Add(cell);
                }
            }
            return cellsInSetInRow;
        }

        public void MergeSets(int row, MazeCell lvalue, MazeCell rvalue)
        {
            int setToMergeUsing = setsForCells[row][rvalue];
            int setToMergeInto = setsForCells[row][lvalue];

            for (int i = 0; i < setsForCells[row].Count; i++)
            {
                if (setsForCells[row][setsKeys[row][i]] == setToMergeUsing)
                {
                    setsForCells[row][setsKeys[row][i]] = setToMergeInto;
                }
            }
        }
    }

    public static T CreateMaze<T>(T grid) where T : MazeGrid
    {
        RowState state = new RowState(grid);
        MazeCell cell = grid.GetRandomCell();
        float chanceToLinkInRow = 0.5f;
        float chanceToLinkBetweenRows = 0.35f;

        for (int z = 0; z < grid.height; z++)
        {
            for (int x = 1; x < grid.width; x++)
            {
                cell = grid.GetCell(x, grid.height - 1 - z);

                int currentSet = state.SetFor(z, cell);
                int currentPriorSet = state.SetFor(z, cell.West);               

                bool shouldLink = currentSet != currentPriorSet && (cell.South == null || Random.value < chanceToLinkInRow);
                if (shouldLink)
                {
                    cell.Link(cell.West);
                    state.MergeSets(z, cell.West, cell);
                }
            }

            if (cell.South != null)
            {
                var sets = state.SetsInRow(z);
                for (int i = 0; i < sets.Count; i++)
                {
                    var cells = Shuffle(state.CellsInSetInRow(z, sets[i]), (int)(Random.value * int.MaxValue));
                    for (int j = 0; j < cells.Count; j++)
                    {
                        if (j == 0 || Random.value < chanceToLinkBetweenRows)
                        {
                            cell = cells[j];
                            cell.Link(cell.South);
                            state.Record(z + 1, state.SetFor(z, cell), cell.South);
                        }
                    }
                }              
            }
        }

        return grid;
    }

    private static List<T> Shuffle<T>(List<T> list, int seed)
    {
        Random.State initialState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < list.Count - 1; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T exchange = list[randomIndex];
            list[randomIndex] = list[i];
            list[i] = exchange;
        }

        Random.state = initialState;
        return list;
    }
}
