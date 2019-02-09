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

    class NPCGenerator : AGenerator
    {
        private class NPCJsonData
        {
            public List<GenerateData> SpawnDatas = new List<GenerateData>();
        }
        private struct GenerateData
        {
            #region classification
            public string TYPE;
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

        public override void Init()
        {
        }

        public override void Release()
        {
        }
    }
}
