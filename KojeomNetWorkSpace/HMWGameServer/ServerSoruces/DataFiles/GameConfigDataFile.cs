using HMWGameServer.ServerSoruces.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer.ServerSoruces.DataFiles
{
    struct ConfigFileStruct
    {
        public int WorldAreaRow;
        public int WorldAreaColumn;
        public int WorldAreaLayer;
        public int SubWorldRow;
        public int SubWorldColumn;
        public int SubWorldLayer;
        public int SubWorldSizeX;
        public int SubWorldSizeY;
        public int SubWorldSizeZ;
        public int ChunkSize;
    }
    class GameConfigDataFile
    {
        private ConfigFileStruct Config;
        private static GameConfigDataFile Instance;
        public static GameConfigDataFile GetInstance()
        {
            if (Instance == null) Instance = new GameConfigDataFile();
            return Instance;
        }

        private GameConfigDataFile()
        {
            if(Directory.Exists(GameFilePath.ConfigDirectory) == false)
            {
                Directory.CreateDirectory(GameFilePath.ConfigDirectory);
            }
            //
            if(File.Exists(GameFilePath.ConfigFilePath) == false)
            {
                using (StreamWriter file = File.CreateText(GameFilePath.ConfigFilePath))
                {
                    ConfigFileStruct defaultConfig = new ConfigFileStruct
                    {
                        WorldAreaRow = 1,
                        WorldAreaColumn = 1,
                        WorldAreaLayer = 1,
                        SubWorldRow = 2,
                        SubWorldColumn = 2,
                        SubWorldLayer = 2,
                        SubWorldSizeX = 32,
                        SubWorldSizeY = 32,
                        SubWorldSizeZ = 32,
                        ChunkSize = 8
                    };
                    //
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(file, defaultConfig);
                }
            }
            // deserialize JSON directly from a file
            using (StreamReader file = File.OpenText(GameFilePath.ConfigFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                Config = (ConfigFileStruct)serializer.Deserialize(file, typeof(ConfigFileStruct));
            }
        }

        public ConfigFileStruct GetConfig()
        {
            return Config;
        }
    }
}
