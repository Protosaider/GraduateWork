using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities
{
    public static partial class Math
    {
        public static float Remap(float value, float inputFrom, float inputTo, float outputFrom, float outputTo)
        {
            return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
        }

        /// <summary>
        ///   Returns binomial coefficient without explicit use of factorials,
        ///   which can't be used with negative integers. Pascal triangle function.
        /// </summary>
        /// <param name="row">
        ///   Row in Pascal triangle.
        /// </param>
        /// <param name="column">
        ///   Column in Pascal triangle.
        /// </param>
        /// <returns>
        ///   Binomial coefficient.
        /// </returns>
        public static int PascalTriangle(int row, int column)
        {
            var result = 1;
            for (var i = 0; i < column; ++i)
            {
                result *= (row - i) / (i + 1);
            }
            return result;
        }


        public static Vector2 GetRandomPointInCircle(float radius)
        {
            // Using rejection sampling

            // canonical Ellipse equation: (x - h)^2 / Rx^2 + (y - k)^2 / Ry^2 = 1
            //
            // Where: (h,k) - center, 
            //  Rx - semi-major axis, 
            //  Ry - semi-minor axis,
            //  (x,y) - sample point
            //
            // Suppose that:
            //  center = (0, 0) point in Cartesian coord. 
            //  Rx and Ry match x- and y-axis, 
            //  Width > height
            //
            // than point is inside of ellipse if
            //
            // x^2 / width^2 + y^2 / height^2 <= 1 

            bool isInside = false;
            float x = 0.0f;
            float y = 0.0f;
            do
            {
                x = Random.Range(-radius, radius);
                y = Random.Range(-radius, radius);

                if ((x * x) + (y * y) <= (radius * radius))
                {
                    isInside = !isInside;
                }
            }
            while (!isInside);

            return new Vector2(x, y);
        }


        // !FIX!
        // TODO: Implement polygon clipping!!!
        public static bool IsLineIntersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.x * d.y - b.y * d.x;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.x * d.y - c.y * d.x) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.x * b.y - c.y * b.x) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * b;

            return true;
        }

        //4 bits for two-dimensional clipping or 6 bits in the three-dimensional 
        [System.Flags]
        public enum Outcodes
        {
            INSIDE = 0,
            LEFT = 1,
            RIGHT = 2,
            BOTTOM = 4,
            TOP = 8,
        }

        private static Outcodes ComputeOutCode(Vector2 point, Rect rect)
        {
            Outcodes code;
            code = Outcodes.INSIDE; // initialised as being inside of [[clip window]]

            if (point.x < rect.x)                        // to the left of clip window
                code |= Outcodes.LEFT;
            else if (point.x > rect.x + rect.width)      // to the right of clip window
                code |= Outcodes.RIGHT;
            if (point.y < rect.y)                        // below the clip window
                code |= Outcodes.BOTTOM;
            else if (point.y > rect.y + rect.height)      // above the clip window
                code |= Outcodes.TOP;

            return code;
        }

        /// <summary>
        /// Cohen–Sutherland clipping algorithm clips a line from P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with diagonal from (xmin, ymin) to (xmax, ymax).
        /// </summary>
        /// <param name="line"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool PolygonClippingCohenSutherland(Vector2 lineStart, Vector2 lineEnd, Rect rect)
        {
            //Line lineCurrent = line;

            // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
            Outcodes outcode0 = ComputeOutCode(lineStart, rect);
            Outcodes outcode1 = ComputeOutCode(lineEnd, rect);
            bool accept = false;

            while (true)
            {
                if (!((outcode0 | outcode1) == Outcodes.INSIDE))
                {
                    // bitwise OR is 0: both points inside window; trivially accept and exit loop
                    accept = true;
                    break;
                }
                else if ((outcode0 & outcode1) != Outcodes.INSIDE)
                {
                    // bitwise AND is not 0: both points share an outside zone (LEFT, RIGHT, TOP,
                    // or BOTTOM), so both must be outside window; exit loop (accept is false)
                    break;
                }
                else
                {
                    // failed both tests, so calculate the line segment to clip
                    // from an outside point to an intersection with clip edge
                    float x = 0.0f, y = 0.0f;

                    // At least one endpoint is outside the clip rectangle; pick it.
                    Outcodes outcodeOut = outcode0 == Outcodes.INSIDE ? outcode0 : outcode1;

                    // Now find the intersection point;
                    // use formulas:
                    //   slope = (y1 - y0) / (x1 - x0)
                    //   x = x0 + (1 / slope) * (ym - y0), where ym is ymin or ymax
                    //   y = y0 + slope * (xm - x0), where xm is xmin or xmax
                    // No need to worry about divide-by-zero because, in each case, the
                    // outcode bit being tested guarantees the denominator is non-zero
                    if (outcodeOut == Outcodes.TOP)
                    {   // point is above the clip window
                        x = lineStart.x + (lineEnd.x - lineStart.x) * (rect.y + rect.height - lineStart.y) / (lineEnd.y - lineStart.y);
                        y = rect.y + rect.height;
                    }
                    else if (outcodeOut == Outcodes.BOTTOM)
                    { // point is below the clip window
                        x = lineStart.x + (lineEnd.x - lineStart.x) * (rect.y - lineStart.y) / (lineEnd.y - lineStart.y);
                        y = rect.y;
                    }
                    else if (outcodeOut == Outcodes.RIGHT)
                    {  // point is to the right of clip window
                        y = lineStart.y + (lineEnd.y - lineStart.y) * (rect.x + rect.width - lineStart.x) / (lineEnd.x - lineStart.x);
                        x = rect.x + rect.width;
                    }
                    else if (outcodeOut == Outcodes.LEFT)
                    {   // point is to the left of clip window
                        y = lineStart.y + (lineEnd.y - lineStart.y) * (rect.x - lineStart.x) / (lineEnd.x - lineStart.x);
                        x = rect.x;
                    }

                    // Now we move outside point to intersection point to clip
                    // and get ready for next pass.
                    if (outcodeOut == outcode0)
                    {
                        lineStart.x = x;
                        lineStart.y = y;
                        outcode0 = ComputeOutCode(lineStart, rect);
                    }
                    else
                    {
                        lineStart.x = x;
                        lineStart.y = y;
                        outcode1 = ComputeOutCode(lineStart, rect);
                    }
                }
            }

            return accept;
            //if (accept)
            //{
            //    // Following functions are left for implementation by user based on
            //    // their platform (OpenGL/graphics.h etc.)
            //    DrawRectangle(xmin, ymin, xmax, ymax);
            //    LineSegment(x0, y0, x1, y1);
            //}
        }

        public static bool IsLineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersectionPoint)
        {
            bool isIntersecting = false;

            float denominator = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);

            //Make sure the denominator is > 0, if so the lines are parallel
            if (denominator != 0)
            {
                float u_a = ((b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x)) / denominator;
                float u_b = ((a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x)) / denominator;

                //Is intersecting if u_a and u_b are between 0 and 1
                if (u_a >= 0 && u_a <= 1 && u_b >= 0 && u_b <= 1)
                {
                    isIntersecting = true;
                    intersectionPoint = new Vector2(a1.x + a2.x - a1.x * u_a, a1.y + a2.y - a1.y * u_a);
                }
                else
                {
                    intersectionPoint = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
                }
            }
            else
            {
                intersectionPoint = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
            }

            return isIntersecting;
        }
    }
}