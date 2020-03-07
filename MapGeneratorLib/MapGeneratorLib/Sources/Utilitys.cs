using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGenLib
{
    public class Utilitys
    {
        // Seed 값은 일정하게 0으로 고정.
        private static System.Random RandomInstance = new System.Random(0);
        public static bool RandomBool()
        {
            // min( inclusive ) max (exclusive)
            int randValue = RandomInstance.Next(0, 2);
            if (randValue == 0) return false;
            return true;
        }
        /// <summary>
        /// 2차원 벡터 외적.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float CrossVector2D(CustomVector2 a, CustomVector2 b)
        {
            return (a.x * b.y - a.y * b.x);
        }

        public static void ChangeSeed()
        {
            RandomInstance = new System.Random(DateTime.Now.Millisecond);
        }
        public static void ChangeSeed(int newSeed)
        {
            RandomInstance = new System.Random(newSeed);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"> inclusive </param>
        /// <param name="max"> exclusive </param>
        /// <returns></returns>
        public static int RandomInteger(int min, int max)
        {
            return RandomInstance.Next(min, max);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">inclusive</param>
        /// <param name="max">inclusive</param>
        /// <returns></returns>
        public static float RandomFloat(float min, float max)
        {
            //return RandomInstance.Next(min, max);
            return 1.0f;
        }

    }
}
