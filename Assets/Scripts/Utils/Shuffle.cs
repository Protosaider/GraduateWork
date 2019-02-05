using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities
{
    public static class ShuffleData
    {
        public static void ShuffleFisherYates<T>(this IList<T> list, int seed)
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
        }

        public static T[] ShuffleFisherYates<T>(T[] array, int seed)
        {
            Random.State initialState = Random.state;
            Random.InitState(seed);
            for (int i = 0; i < array.Length - 1; i++)
            {
                int randomIndex = Random.Range(i, array.Length);
                T exchange = array[randomIndex];
                array[randomIndex] = array[i];
                array[i] = exchange;
            }
            Random.state = initialState;
            return array;
        }

        public static T[] ShuffleWithKeyValue<T>(T[] array, int seed)
        {
            Random.State initialState = Random.state;
            Random.InitState(seed);

            List<KeyValuePair<int, T>> list = new List<KeyValuePair<int, T>>();
            /// Add all T from array.
            /// ... Add new random int each time.
            foreach (T item in array)
            {
                list.Add(new KeyValuePair<int, T>(Random.Range(int.MinValue, int.MaxValue), item));
            }

            /// Sort the list by the random number.
            list.Sort((x, y) => x.Key.CompareTo(y.Key));
            /// Allocate new string array.           
            //T[] result = new T[array.Length];
            /// Copy values to array.
            int index = 0;
            foreach (KeyValuePair<int, T> pair in list)
            {
                //result[index] = pair.Value;
                array[index] = pair.Value;
                index++;
            }

            Random.state = initialState;
            // Return copied array.
            //return result;
            return array;
        }

    }
}
