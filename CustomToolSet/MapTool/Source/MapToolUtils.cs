using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTool.Source
{
    class MapToolUtils
    {
        private static int RandomSeed = 0;
        private static System.Random RandomInstance = new System.Random(RandomSeed);

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static bool RandomBool()
        {
            // min( inclusive ) max (exclusive)
            int randValue = RandomInstance.Next(0, 2);
            if (randValue == 0) return false;
            return true;
        }
    }
    
}
