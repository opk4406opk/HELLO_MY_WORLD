using System;
using System.Collections.Generic;
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

        public static Vector3 DisassembleUniqueID(string uniqueID)
        {
            var sub = uniqueID.Substring(uniqueID.IndexOf("_"));
            var split = sub.Split(':');
            return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        }
    }
}
