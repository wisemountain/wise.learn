using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnOpenTK.Render
{
    class MathUtil
    {
        private static Random rand = new Random();

        public static float DegreeToRadian(float degree)
        {
            return (float)System.Math.PI * degree / 180.0f;
        }

        public static float RadianToDegree(float radian)
        {
            return radian * 180.0f / (float)System.Math.PI;
        }

        public static float NextRand()
        {
            return (float)rand.NextDouble();
        }
    }
}
