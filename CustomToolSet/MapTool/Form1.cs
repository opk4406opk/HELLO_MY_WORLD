using MapTool.Source;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
namespace MapTool
{
   
    public partial class MainForm : Form
    {

        private SaveSlotFile SaveSlotFileInstance;
        private IFormatter BinaryFormatterInstance = new BinaryFormatter();
        private MapDataGenerator MapDataGeneratorInstance;
        public MainForm()
        {
            InitializeComponent();
            //
            MapDataGeneratorInstance = new MapDataGenerator();
            SaveSlotFileInstance = new SaveSlotFile();
        }

        private bool CheckInputDataValidate(ref int subWorldRow, ref int subWorldColumn, ref int subWorldLayer, ref int worldAreaRow, ref int worldAreaColumn, ref int worldAreaLayer)
        {
            return int.TryParse(tbx_subWorldRow.Text, out subWorldRow) &&
                   int.TryParse(tbx_subWorldColumn.Text, out subWorldColumn) &&
                   int.TryParse(tbx_subWorldLayer.Text, out subWorldLayer) &&
                   int.TryParse(tbx_worldAreaRow.Text, out worldAreaRow) &&
                   int.TryParse(tbx_worldAreaColumn.Text, out worldAreaColumn) &&
                   int.TryParse(tbx_worldAreaLayer.Text, out worldAreaLayer);
        }

        private void btn_StartGenerate_Click(object sender, EventArgs e)
        {
            WorldMapData mapGenData;
            int worldAreaRow = 0, worldAreaColumn = 0, worldAreaLayer = 0;
            int subWorldRow = 0, subWorldColumn = 0, subWorldLayer = 0;
            if (CheckInputDataValidate(ref subWorldRow, ref subWorldColumn, ref subWorldLayer, ref worldAreaRow, ref worldAreaColumn, ref worldAreaLayer) == true)
            {

                mapGenData.SubWorldRow = MapToolUtils.Clamp<int>(subWorldRow, 0, MapDataGenerator.SubWorld_Count_X_Axis_Per_WorldArea);
                mapGenData.SubWorldColumn = MapToolUtils.Clamp<int>(subWorldColumn, 0, MapDataGenerator.SubWorld_Count_Y_Axis_Per_WorldArea);
                mapGenData.SubWorldLayer = MapToolUtils.Clamp<int>(subWorldLayer, 0, MapDataGenerator.SubWorld_Count_Z_Axis_Per_WorldArea);
                mapGenData.WorldAreaRow = worldAreaRow;
                mapGenData.WorldAreaColumn = worldAreaColumn;
                mapGenData.WorldAreaLayer = worldAreaLayer;
            }
            else
            {
                mapGenData.SubWorldRow = MapDataGenerator.DefaultSubWorldRowValue;
                mapGenData.SubWorldColumn = MapDataGenerator.DefaultSubWorldColumnValue;
                mapGenData.SubWorldLayer = MapDataGenerator.DefaultSubWorldLayerValue;
                mapGenData.WorldAreaRow = MapDataGenerator.DefaultWorldAreaRowValue;
                mapGenData.WorldAreaColumn = MapDataGenerator.DefaultWorldAreaColumnValue;
                mapGenData.WorldAreaLayer = MapDataGenerator.DefaultWorldAreaLayerValue;
            }

            if (File.Exists(SaveSlotFile.SaveFilePath) == true &&
                string.Equals(dig_folderBrowser.SelectedPath, string.Empty) == true &&
                string.Equals(dig_serverConfigPath.SelectedPath, string.Empty) == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.Open))
                {
                    SaveSlotFileInstance = BinaryFormatterInstance.Deserialize(SaveFileStream) as SaveSlotFile;
                    MapToolPath.SubWorldFilePath = SaveSlotFileInstance.LastestClientSubWorldSavePath;
                    MapToolPath.ClientWorldConfigFilePath = SaveSlotFileInstance.LastestClientWorldConfgSavePath;
                    MapToolPath.ServerWorldConfigFilePath = SaveSlotFileInstance.LastestServerWorldConfigSavePath;
                }
            }
            else if (string.Equals(dig_folderBrowser.SelectedPath, string.Empty) == false &&
                     string.Equals(dig_serverConfigPath.SelectedPath, string.Empty) == false)
            {
                MapToolPath.SubWorldFilePath = string.Format("{0}//WorldMapData.json", dig_folderBrowser.SelectedPath);
                MapToolPath.ClientWorldConfigFilePath = string.Format("{0}//WorldConfigData.json", dig_folderBrowser.SelectedPath);
                MapToolPath.ServerWorldConfigFilePath = string.Format("{0}//ServerConfig.json", dig_serverConfigPath.SelectedPath);
            }
            else
            {
                MapToolPath.SubWorldFilePath = ".//WorldMapData.json";
                MapToolPath.ClientWorldConfigFilePath = ".//WorldConfigData.json";
                MapToolPath.ServerWorldConfigFilePath = ".//ServerConfig.json";
            }

            MapDataGeneratorInstance.Init(mapGenData);
            if (MapDataGeneratorInstance.Generate())
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.OpenOrCreate))
                {
                    SaveSlotFileInstance.LastestClientSubWorldSavePath = MapToolPath.SubWorldFilePath;
                    SaveSlotFileInstance.LastestServerWorldConfigSavePath = MapToolPath.ServerWorldConfigFilePath;
                    SaveSlotFileInstance.LastestClientWorldConfgSavePath = MapToolPath.ClientWorldConfigFilePath;
                    BinaryFormatterInstance.Serialize(SaveFileStream, SaveSlotFileInstance);
                }
                tbx_logBox.AppendText(string.Format("[LOG] Success MapData Generate. \n"));
            }
        }

        private void btn_openSaveFilePathDig_Click(object sender, EventArgs e)
        {
            var ret = dig_folderBrowser.ShowDialog();
            if (ret == DialogResult.OK)
            {
                tbx_logBox.AppendText(string.Format("[LOG] Select folder Path : {0} \n", dig_folderBrowser.SelectedPath));
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists(SaveSlotFile.SaveFilePath) == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.Open))
                {
                    SaveSlotFileInstance = BinaryFormatterInstance.Deserialize(SaveFileStream) as SaveSlotFile;
                    tbx_lastSavePath.Text = SaveSlotFileInstance.LastestClientSubWorldSavePath;
                    tbx_latestServerConfigPath.Text = SaveSlotFileInstance.LastestServerWorldConfigSavePath;
                    SaveFileStream.Close();
                }
            }
        }

        private void Panel_MapViewer_Paint(object sender, PaintEventArgs e)
        {
            //using (MapViewer viewer = new MapViewer(800, 600, "LearnOpenTK"))
            //{
            //    //Run takes a double, which is how many frames per second it should strive to reach.
            //    //You can leave that out and it'll just update as fast as the hardware will allow it.
            //    viewer.Run(60.0);
            //}
        }

        private void Btn_SelectServerConfigPath_Click(object sender, EventArgs e)
        {
            var ret = dig_serverConfigPath.ShowDialog();
            if (ret == DialogResult.OK)
            {
                tbx_logBox.AppendText(string.Format("[LOG] Select ServerConfig folder Path : {0} \n", dig_serverConfigPath.SelectedPath));
            }
        }
    }

    //https://www.guru99.com/c-sharp-serialization.html
    //
    [Serializable]
    public class SaveSlotFile
    {
        public static readonly string SaveFilePath = ".//SaveData.bin";
        public string LastestClientSubWorldSavePath;
        public string LastestClientWorldConfgSavePath;
        public string LastestServerWorldConfigSavePath;
    }

    public class MapToolPath
    {
        //
        public static string SubWorldFilePath;
        public static string ClientWorldConfigFilePath;
        public static string ServerWorldConfigFilePath;
        //
    }



}
