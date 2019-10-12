using ActorGeneratorTool.Sources.Share;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources
{
    class NPCGenerator : AGenerator
    {
        private class NPCJsonData
        {
            public List<BaseGenerateData> SpawnDatas = new List<BaseGenerateData>();
        }

        private class NPCGenerateData : BaseGenerateData
        {
            // some add traits..
        }
        private NPCJsonData NpcDatas = new NPCJsonData();
      
        public override bool Generate(bool isDefaultGenerate, string savePath)
        {
            if (isDefaultGenerate == true)
            {
                for(int idx = 0; idx < 10; idx++)
                {
                    NPCGenerateData data = new NPCGenerateData();
                    data.HP = KojeomUtils.GetInstance().GetRandomInstance().Next(1, 10).ToString();
                    data.MP = KojeomUtils.GetInstance().GetRandomInstance().Next(1, 10).ToString();
                    data.AP = KojeomUtils.GetInstance().GetRandomInstance().Next(1, 10).ToString();
                    data.NAME = string.Format("Default_NPC_{0}", idx.ToString());
                    int randVal = KojeomUtils.GetInstance().GetRandomInstance().Next((int)NPC_TYPE.Merchant, (int)NPC_TYPE.COUNT);
                    data.TYPE = ((NPC_TYPE)randVal).ToString();
                    data.RESOURCE_ID = string.Format("{0}{1}", ((NPC_TYPE)randVal).ToString(), 
                        KojeomUtils.GetInstance().GetRandomInstance().Next(0, 2));
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


        public override void Init()
        {
        }

        public override void Release()
        {
            NpcDatas.SpawnDatas.Clear();
        }
    }
}
