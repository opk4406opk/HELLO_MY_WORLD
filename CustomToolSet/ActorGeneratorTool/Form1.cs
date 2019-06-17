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
        private AnimalGenerator AnimalGenerator;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitGenerators();
        }

        private void OnClosedMainForm(object sender, EventArgs e)
        {
            ReleaseGenerators();
        }

        private void InitGenerators()
        {
            NpcGenerator = new NPCGenerator();
            NpcGenerator.Init();

            AnimalGenerator = new AnimalGenerator();
            AnimalGenerator.Init();
        }
        private void ReleaseGenerators()
        {
            NpcGenerator.Release();
            AnimalGenerator.Release();
        }

        private void btn_GenerateNPCData_Click(object sender, EventArgs e)
        {
            SaveDataFile("NPCDatas.json", GenerateActorType.NPC);
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

        private void Btn_GenerateAnimalData_Click(object sender, EventArgs e)
        {
            SaveDataFile("AnimalDatas.json", GenerateActorType.ANIMAL);
        }

        private void SaveDataFile(string fileNameWithExt, GenerateActorType genActorType)
        {
            string savePath = string.Empty;
            if (File.Exists(SaveSlotFile.SaveFilePath) == true &&
               string.Equals(dig_SelectSavePath.SelectedPath, string.Empty) == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.Open))
                {
                    SaveSlotFile = BinFormatter.Deserialize(SaveFileStream) as SaveSlotFile;
                    savePath = string.Format("{0}//{1}",SaveSlotFile.LastestSaveRootDirectory, fileNameWithExt);
                }
            }
            else if (string.Equals(dig_SelectSavePath.SelectedPath, string.Empty) == false)
            {
                savePath = string.Format("{0}//{1}", dig_SelectSavePath.SelectedPath, fileNameWithExt);
            }
            else
            {
                savePath = string.Format(".//{0}", fileNameWithExt);
            }
            //
            bool isGenSuccess = false;
            switch(genActorType)
            {
                case GenerateActorType.NPC:
                    isGenSuccess = NpcGenerator.Generate(cbx_IsDefaultGenerate.Checked, savePath);
                    NpcGenerator.Release();
                    break;
                case GenerateActorType.ANIMAL:
                    isGenSuccess = AnimalGenerator.Generate(cbx_IsDefaultGenerate.Checked, savePath);
                    AnimalGenerator.Release();
                    break;
                case GenerateActorType.MONSTER:
                    break;
            }
            if (isGenSuccess == true)
            {
                using (var SaveFileStream = new FileStream(SaveSlotFile.SaveFilePath, FileMode.OpenOrCreate))
                {
                    SaveSlotFile = new SaveSlotFile();
                    SaveSlotFile.LastestSaveRootDirectory = Path.GetDirectoryName(savePath);
                    BinFormatter.Serialize(SaveFileStream, SaveSlotFile);
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
        public string LastestSaveRootDirectory;
    }

}
