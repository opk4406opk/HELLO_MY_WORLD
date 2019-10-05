using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources.Share
{
    class KojeomUtils
    {
        private readonly Random RandomInstance = new Random(0);
        private static KojeomUtils Instance = null;
        public static KojeomUtils GetInstance()
        {
            if(Instance == null)
            {
                Instance = new KojeomUtils();
            }
            return Instance;
        }

        public Random GetRandomInstance()
        {
            return RandomInstance;
        }

        private KojeomUtils() { }

        // ref : https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
        // ref : https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse?redirectedfrom=MSDN&view=netframework-4.7.2#overloads
        public static T StringToEnum<T>(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

    }
}
