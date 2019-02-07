using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MapTool.Source
{
    public struct MapGenerateData
    {
        public int Row;
        public int Column;
        public int Layer;
        public string SelectPath;
    }
        
    class MapDataGenerator
    {
        #region defualt values.
        public static readonly int  DefaultRowValue = 2;
        public static readonly int  DefaultColumnValue = 2;
        public static readonly int  DefaultLayerValue = 2;
        //
        #endregion

        private struct Properties
        {
            public int ROW;
            public int COLUMN;
            public int LAYER;
        }
       
        private struct SubWorldData
        {
            public string UNIQUE_ID;
            public string X;
            public string Y;
            public string Z;
            public string WORLD_NAME;
            public string IS_SURFACE;
        }

        private MapGenerateData GenData;

        class MapData
        {
            public Properties Properties;
            public List<SubWorldData> SubWorldDatas = new List<SubWorldData>();
        }
        
        public void Init(MapGenerateData genData)
        {
            GenData = genData;
        }

        public void Generate()
        {
            MapData mapData = new MapData();
            mapData.Properties.ROW = GenData.Row;
            mapData.Properties.COLUMN = GenData.Column;
            mapData.Properties.LAYER = GenData.Layer;

            int worldIndex = 0;
            for (int x = 0; x < GenData.Row; x++)
            {
                for (int y = 0; y < GenData.Layer; y++)
                {
                    for (int z = 0; z < GenData.Column; z++)
                    {
                        SubWorldData subWorldData = new SubWorldData
                        {
                            UNIQUE_ID = string.Format("unique_{0}:{1}:{2}", x.ToString(), y.ToString(), z.ToString()),
                            X = x.ToString(),
                            Y = y.ToString(),
                            Z = z.ToString(),
                            WORLD_NAME = string.Format("SUB_WORLD_{0}", worldIndex)
                        };
                        if(y == (GenData.Layer - 1))
                        {
                            subWorldData.IS_SURFACE = true.ToString();
                        }
                        else
                        {
                            subWorldData.IS_SURFACE = false.ToString();
                        }
                        mapData.SubWorldDatas.Add(subWorldData);
                        worldIndex++;
                    }
                }
            }
            File.WriteAllText(GenData.SelectPath, JsonConvert.SerializeObject(mapData, Formatting.Indented));
        }
    }
}
