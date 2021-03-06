﻿using HMWGameServer.ServerSoruces.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer.ServerSoruces.DataFiles
{
    public class WorldMapData
    {
        public int WorldAreaRow;
        public int WorldAreaColumn;
        public int WorldAreaLayer;

        public int SubWorldRow;
        public int SubWorldColumn;
        public int SubWorldLayer;

        public List<WorldAreaTerrainData> WorldAreaDatas = new List<WorldAreaTerrainData>();
    }

    public class WorldAreaTerrainData
    {
        public string UniqueID;
        public int OffsetX;
        public int OffsetY;
        public int OffsetZ;
        public string AreaName;
        public List<SubWorldData> SubWorldDatas = new List<SubWorldData>();
    }

    public struct SubWorldData
    {
        public string UniqueID;
        public int OffsetX;
        public int OffsetY;
        public int OffsetZ;
        public string WorldName;
        public bool bSurface;
    }

    class GameWorldMapDataFile
    {
        public WorldMapData MapData { get; private set; } = new WorldMapData();

        private static GameWorldMapDataFile Instance;
        public static GameWorldMapDataFile GetInstance()
        {
            if (Instance == null) Instance = new GameWorldMapDataFile();
            return Instance;
        }

        private GameWorldMapDataFile()
        {
            if (Directory.Exists(GameFilePath.ConfigDirectory) == false)
            {
                Directory.CreateDirectory(GameFilePath.ConfigDirectory);
            }

            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(GameFilePath.WorldMapFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                MapData = (WorldMapData)serializer.Deserialize(file, typeof(WorldMapData));
            }
        }
    }
}
