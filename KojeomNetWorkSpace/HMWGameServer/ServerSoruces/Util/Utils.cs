using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    }
    class Utils
    {
        // Seed 값은 일정하게 0으로 고정.
        private static int RandomSeed = 0;
        private static System.Random RandomInstance = new System.Random(RandomSeed);

        public static void ChangeSeed()
        {
            RandomInstance = new System.Random(DateTime.Now.Millisecond);
        }
        public static void ChangeSeed(int newSeed)
        {
            RandomInstance = new System.Random(newSeed);
        }

        public static int GetSeed() { return RandomSeed; }
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

        // ref : https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
        // ref : https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse?redirectedfrom=MSDN&view=netframework-4.7.2#overloads
        public static T StringToEnum<T>(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string EnumToString<T>(T value)
        {
            return value.ToString();
        }

        /// <summary>
        /// UniqueID를 생성합니다.
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public static string MakeUniqueID(Vector3 pos)
        {
            // basic form : unique_0:0:0  ( x:y:z )
            return string.Format("unique_{0}:{1}:{2}", pos.x, pos.y, pos.z);
        }

        /// <summary>
        /// UniqueID를 생성합니다.
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public static string MakeUniqueID(int x, int y, int z)
        {
            // basic form : unique_0:0:0  ( x:y:z )
            return string.Format("unique_{0}:{1}:{2}", x, y, z);
        }

        /// <summary>
        /// UniqueID를 FileName으로 변환.
        /// </summary>
        /// <returns></returns>
        public static string ConvertUniqueIDToFileName(string uniqueID)
        {
            Vector3 vec = DisassembleUniqueID(uniqueID);
            return string.Format("unique_{0}_{1}_{2}", vec.x, vec.y, vec.z);
        }

        public static Vector3 DisassembleUniqueID(string uniqueID)
        {
            var sub = uniqueID.Substring(uniqueID.IndexOf("_") + 1);
            var split = sub.Split(':');
            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
