using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActorGeneratorTool
{
    public partial class Form1 : Form
    {
        private SaveSlotFile SaveSlotFile;
        private IFormatter BinFormatter = new BinaryFormatter();

        public Form1()
        {
            InitializeComponent();
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
