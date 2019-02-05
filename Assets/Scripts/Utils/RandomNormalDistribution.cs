using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities
{
    public static class RandomNormalDistribution
    {
        private static float secondNormNum;
        private static bool hasSecondNum;

        // Box-Muller
        private static float RandNumGaussianPolar()
        {
            if (hasSecondNum)
            {
                hasSecondNum = false;
                return secondNormNum;
            }
            else
            {
                float u, v, s;

                do
                {
                    u = 2.0f * Random.value - 1.0f;
                    v = 2.0f * Random.value - 1.0f;
                    s = u * u + v * v;
                } while ((s > 1) || (s == 0));

                float r = Mathf.Sqrt(-2.0f * Mathf.Log(s) / s);

                secondNormNum = r * u;
                hasSecondNum = true;

                return r * v;
            }
        }

        public static float RandomGaussian(float mean, float sd)
        {
            return RandNumGaussianPolar() * sd + mean;
        }

        // Table from 68%, 95%, 99.7% rule (welcome to Wikipedia)
        private static float[] standartScore = 
        {
            0.84162123f,
            1.28155156f,
            1.64485363f,
            1.95996399f,
            2.32634787f,
            2.57582931f,
            3.0902323f,
            3.29052673f
        };

        public enum ConfidenceInterval { _60 = 0, _80, _90, _95, _98, _99, _998, _999 };

        public static float RandomRangeGaussian(float min, float max, ConfidenceInterval confidence)
        {
            float sd = (max - min) * 0.5f;
            float mean = min + sd;

            // Maybe it's working - can't understand completely
            sd /= standartScore[(int)confidence];

            float random_normal_num;
            do
            {
                random_normal_num = RandomGaussian(mean, sd);
            } while (random_normal_num > max || random_normal_num < min);

            return random_normal_num;
        }

        static float Erf(float x)
        {
            // constants
            float a1 = 0.254829592f;
            float a2 = -0.284496736f;
            float a3 = 1.421413741f;
            float a4 = -1.453152027f;
            float a5 = 1.061405429f;
            float p = 0.3275911f;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Mathf.Abs(x);

            // A&S formula 7.1.26
            float t = 1.0f / (1.0f + p * x);
            float y = 1.0f - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Mathf.Exp(-x * x);

            return sign * y;
        }
    }
}
