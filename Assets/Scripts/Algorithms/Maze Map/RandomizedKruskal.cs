using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomizedKruskal  {

    private class State
    {
        public MazeGrid mazeGrid;
        public List<KeyValuePair<MazeCell, MazeCell>> neighbours;
        public Dictionary<MazeCell, int> setsForCells;
        public List<MazeCell> setsKeys;

        public State(MazeGrid grid)
        {
            mazeGrid = grid;
            neighbours = new List<KeyValuePair<MazeCell, MazeCell>>();

            setsForCells = new Dictionary<MazeCell, int>();

            foreach (var cell in mazeGrid.EachCell())
            {
                int set = setsForCells.Count;
                setsForCells.Add(cell, set);

                if (cell.South != null)
                {
                    neighbours.Add(new KeyValuePair<MazeCell, MazeCell>(cell, cell.South));
                }
                if (cell.East != null)
                {
                    neighbours.Add(new KeyValuePair<MazeCell, MazeCell>(cell, cell.East));
                }
            }

            setsKeys = new List<MazeCell>(setsForCells.Keys);
        }

        public bool CanMergeSets(MazeCell lvalue, MazeCell rvalue)
        {
            return setsForCells[lvalue] != setsForCells[rvalue];
        }

        public void MergeSets(MazeCell lvalue, MazeCell rvalue)
        {
            lvalue.Link(rvalue);
            int setToMergeUsing = setsForCells[rvalue];
            int setToMergeInto = setsForCells[lvalue];

            for (int i = 0; i < setsForCells.Count; i++)
            {
                if (setsForCells[setsKeys[i]] == setToMergeUsing)
                {
                    setsForCells[setsKeys[i]] = setToMergeInto;
                }               
            }
        }
    }

    public static T CreateMaze<T>(T grid) where T : MazeGrid
    {
        State state = new State(grid);
        Queue<KeyValuePair<MazeCell, MazeCell>> neighbours = new Queue<KeyValuePair<MazeCell, MazeCell>>(
            Shuffle(state.neighbours, (int)(Random.value * int.MaxValue))
            );

        while (neighbours.Count > 0)
        {
            var values = neighbours.Dequeue();
            if (state.CanMergeSets(values.Key, values.Value))
            {
                state.MergeSets(values.Key, values.Value);
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
   
