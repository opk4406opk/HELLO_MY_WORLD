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
