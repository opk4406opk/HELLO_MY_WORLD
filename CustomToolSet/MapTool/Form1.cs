using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapTool.Source;
namespace MapTool
{
    
    public partial class MainForm : Form
    {
        private SaveSlotFile SaveSlotFile;
        private IFormatter BinFormatter = new BinaryFormatter();
        private MapDataGenerator MapDataGenerator;
        public MainForm()
        {
            InitializeComponent();
            //
            MapDataGenerator = new MapDataGenerator();
            SaveSlotFile = new SaveSlotFile();
        }

        private bool CheckInputDataValidate(ref int row, ref int column, ref int layer)
        {
            return int.TryParse(tbx_row.Text, out row) && int.TryParse(tbx_column.Text, out column) && int.TryParse(tbx_layer.Text, out layer);
        }

        private void btn_StartGenerate_Click(object sender, EventArgs e)
        {
            MapGenerateData mapGenData;
            int row = 0, column = 0, layer = 0;
            if(CheckInputDataValidate(ref row, ref column, ref layer) == true)
            {
                mapGenData.Row = row;
                mapGenData.Column = column;
                mapGenData.Layer = layer;
            }
            else
            {
                mapGenData.Row = MapDataGenerator.DefaultRowValue;
                mapGenData.Column = MapDataGenerator.DefaultColumnValue;
                mapGenData.Layer = MapDataGenerator.DefaultLayerValue;
            }

            if(File.Exists(SaveSlotFile.SaveFilePath) == true && 
                string.Equals(dig_folderBrowser.SelectedPath, string.Empty) == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.Open))
                {
                    SaveSlotFile = BinFormatter.Deserialize(SaveFileStream) as SaveSlotFile;
                    mapGenData.SelectPath = SaveSlotFile.LastestSavePath;
                    SaveFileStream.Close();
                }
            }
            else if (string.Equals(dig_folderBrowser.SelectedPath, string.Empty) == false)
            {
                mapGenData.SelectPath = string.Format("{0}//WorldMapData.json", dig_folderBrowser.SelectedPath);
            }
            else
            {
                mapGenData.SelectPath = ".//WorldMapData.json";
            }

            MapDataGenerator.Init(mapGenData);
            if(MapDataGenerator.Generate())
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.OpenOrCreate))
                {
                    SaveSlotFile.LastestSavePath = mapGenData.SelectPath;
                    BinFormatter.Serialize(SaveFileStream, SaveSlotFile);
                }
                tbx_logBox.AppendText(string.Format("[LOG] Success MapData Generate. \n"));
            }
        }

        private void btn_openSaveFilePathDig_Click(object sender, EventArgs e)
        {
            var ret = dig_folderBrowser.ShowDialog();
            if(ret == DialogResult.OK)
            {
                tbx_logBox.AppendText(string.Format("[LOG] Select folder Path : {0} \n", dig_folderBrowser.SelectedPath));
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(File.Exists(SaveSlotFile.SaveFilePath) == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.Open))
                {
                    SaveSlotFile = BinFormatter.Deserialize(SaveFileStream) as SaveSlotFile;
                    tbx_lastSavePath.Text = SaveSlotFile.LastestSavePath;
                    SaveFileStream.Close();
                }
            }
        }
    }

    //https://www.guru99.com/c-sharp-serialization.html
    //
    [Serializable]
    public class SaveSlotFile
    {
        public static readonly string SaveFilePath = ".//SaveData.bin";
        public string LastestSavePath;
    }

}
