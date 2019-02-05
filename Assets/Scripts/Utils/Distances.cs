using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities
{
    public static class Distances
    {
        //! Manhattan distance
        public static float GetManhattanDistance(Vector2 a, Vector2 b)
        {
            float distanceX = Mathf.Abs(a.x - b.x);
            float distanceZ = Mathf.Abs(a.y - b.y);

            return distanceX > distanceZ ? 4 * distanceZ + 10 * distanceX : 4 * distanceX + 10 * distanceZ;
        }
    }
}
