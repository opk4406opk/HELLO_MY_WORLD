using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

            if(string.Equals(dig_folderBrowser.SelectedPath, string.Empty) == true)
            {
                //
                mapGenData.SelectPath = ".//WorldMapData.json";
            }
            else
            {
                mapGenData.SelectPath = string.Format("{0}//WorldMapData.json", dig_folderBrowser.SelectedPath);
            }
           
            MapDataGenerator.Init(mapGenData);
            MapDataGenerator.Generate();

            tbx_logBox.AppendText(string.Format("[LOG] Success MapData Generate. \n"));
        }

        private void btn_openSaveFilePathDig_Click(object sender, EventArgs e)
        {
            var ret = dig_folderBrowser.ShowDialog();
            if(ret == DialogResult.OK)
            {
                tbx_logBox.AppendText(string.Format("[LOG] Select folder Path : {0} \n", dig_folderBrowser.SelectedPath));
            }
        }
    }
}
