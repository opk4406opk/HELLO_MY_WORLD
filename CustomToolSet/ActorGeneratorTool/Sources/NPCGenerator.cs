using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources
{
    public enum NPC_TYPE
    {
        MERCHANT,
        GUARD
    }

    public enum NPCResourceID
    {
        N0,
        N1,
        //...
        COUNT
    }

    public enum MonsterResourceID
    {
        M0,
        M1,
        //...
        COUNT
    }
    class NPCGenerator : AGenerator
    {
        private class NPCJsonData
        {
            public List<GenerateData> SpawnDatas = new List<GenerateData>();
        }
        private struct GenerateData
        {
            #region classification and Identification
            public string TYPE;
            public string RESOURCE_ID;
            public string UNIQUE_ID;
            #endregion

            #region stat
            public string AP;
            public string HP;
            public string MP;
            #endregion

            #region info
            public string NAME;
            #endregion
        }
        //
        private readonly Random RandomInstance = new Random();
        private NPCJsonData NpcDatas = new NPCJsonData();
      
        public override bool Generate(bool isDefaultGenerate, string savePath)
        {
            if(isDefaultGenerate == true)
            {
                for(int idx = 0; idx < 10; idx++)
                {
                    GenerateData data;
                    data.HP = RandomInstance.Next(1, 10).ToString();
                    data.MP = RandomInstance.Next(1, 10).ToString();
                    data.AP = RandomInstance.Next(1, 10).ToString();
                    data.NAME = string.Format("Default_NPC_{0}", idx.ToString());
                    data.TYPE = ((NPC_TYPE)RandomInstance.Next(0, 1)).ToString();
                    data.RESOURCE_ID = NPCResourceID.N0.ToString();
                    data.UNIQUE_ID = idx.ToString();
                    NpcDatas.SpawnDatas.Add(data);
                }
            }
            else
            {
                //
            }

            try
            {
                // make json file.
                File.WriteAllText(savePath, JsonConvert.SerializeObject(NpcDatas, Formatting.Indented));
                return true;
            }
            catch (IOException ex)
            {
                return false;
            }
        }

        // ref : https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
        // ref : https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse?redirectedfrom=MSDN&view=netframework-4.7.2#overloads
        public T StringToEnum<T>(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public override void Init()
        {
        }

        public override void Release()
        {
        }
    }
}
