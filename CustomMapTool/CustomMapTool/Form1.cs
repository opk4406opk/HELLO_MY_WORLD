using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomMapTool.Source;
namespace CustomMapTool
{
    public partial class MainForm : Form
    {
        private MapDataGenerator MapDataGenerator;
        public MainForm()
        {
            InitializeComponent();
            //
            MapDataGenerator = new MapDataGenerator();
        }

        private void btn_StartGenerate_Click(object sender, EventArgs e)
        {
            MapGenerateData mapGenData;
            mapGenData.Row = 2;
            mapGenData.Column = 2;
            mapGenData.Layer = 2;
            MapDataGenerator.Init(mapGenData);
            MapDataGenerator.Generate();
        }
    }
}
