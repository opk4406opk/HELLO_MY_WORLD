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
using ActorGeneratorTool.Sources;

namespace ActorGeneratorTool
{
    public partial class MainForm : Form
    {
        private SaveSlotFile SaveSlotFile;
        private IFormatter BinFormatter = new BinaryFormatter();

        private NPCGenerator NpcGenerator;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NpcGenerator = new NPCGenerator();
            NpcGenerator.Init();
        }

        private void btn_GenerateNPCData_Click(object sender, EventArgs e)
        {
            string selectPath = string.Empty;
            if (File.Exists(SaveSlotFile.SaveFilePath) == true &&
               string.Equals(dig_SelectSavePath.SelectedPath, string.Empty) == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.Open))
                {
                    SaveSlotFile = BinFormatter.Deserialize(SaveFileStream) as SaveSlotFile;
                    selectPath = SaveSlotFile.LastestSavePath;
                }
            }
            else if (string.Equals(dig_SelectSavePath.SelectedPath, string.Empty) == false)
            {
                selectPath = string.Format("{0}//NPCDatas.json", dig_SelectSavePath.SelectedPath);
            }
            else
            {
                selectPath = ".//NPCDatas.json";
            }
            //
            if (NpcGenerator.Generate(cbx_IsDefaultGenerate.Checked, selectPath))
            {          
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.OpenOrCreate))
                {
                    SaveSlotFile = new SaveSlotFile();
                    SaveSlotFile.LastestSavePath = selectPath;
                    BinFormatter.Serialize(SaveFileStream, SaveSlotFile);
                }
            }
        }

        private void btn_GenerateMonsterData_Click(object sender, EventArgs e)
        {

        }

        private void cbx_IsDefaultGenerate_CheckedChanged(object sender, EventArgs e)
        {
            // to do
        }

        private void btn_SelectPath_Click(object sender, EventArgs e)
        {
            dig_SelectSavePath.ShowDialog();
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
