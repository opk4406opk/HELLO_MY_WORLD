﻿using ActorGeneratorTool.Sources.Share;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources
{
    class AnimalGenerator : AGenerator
    {
        private AnimalJsonData AnimalDatas = new AnimalJsonData();
        private class AnimalJsonData
        {
            public List<BaseGenerateData> SpawnDatas = new List<BaseGenerateData>();
        }

        private class AnimalGenerateData : BaseGenerateData
        {
            // some add traits.
        }
        public override bool Generate(bool isDefaultGenerate, string savePath)
        {
            if (isDefaultGenerate == true)
            {
                for (int idx = 0; idx < 10; idx++)
                {
                    AnimalGenerateData data = new AnimalGenerateData();
                    data.HP = KojeomUtils.GetInstance().GetRandomInstance().Next(1, 10).ToString();
                    data.MP = KojeomUtils.GetInstance().GetRandomInstance().Next(1, 10).ToString();
                    data.AP = KojeomUtils.GetInstance().GetRandomInstance().Next(1, 10).ToString();
                    data.NAME = string.Format("Default_Animal_{0}", idx.ToString());
                    int randVal = KojeomUtils.GetInstance().GetRandomInstance().Next((int)ANIMAL_TYPE.Pig, (int)ANIMAL_TYPE.COUNT);
                    data.TYPE = ((ANIMAL_TYPE)randVal).ToString();
                    data.RESOURCE_ID = ((AnimalResourceID)randVal).ToString();
                    data.UNIQUE_ID = idx.ToString();
                    AnimalDatas.SpawnDatas.Add(data);
                }
            }
            else
            {
                //
            }

            try
            {
                // make json file.
                File.WriteAllText(savePath, JsonConvert.SerializeObject(AnimalDatas, Formatting.Indented));
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
            AnimalDatas.SpawnDatas.Clear();
        }
    }
}
