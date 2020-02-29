namespace MapTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_StartGenerate = new System.Windows.Forms.Button();
            this.tbx_subWorldRow = new System.Windows.Forms.TextBox();
            this.tbx_subWorldColumn = new System.Windows.Forms.TextBox();
            this.tbx_subWorldLayer = new System.Windows.Forms.TextBox();
            this.lbl_name = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbx_logBox = new System.Windows.Forms.TextBox();
            this.dig_folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_openSaveFilePathDig = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tbx_lastSavePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel_ControlOption = new System.Windows.Forms.Panel();
            this.tbx_latestServerConfigPath = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_worldAreaRow = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbx_worldAreaColumn = new System.Windows.Forms.TextBox();
            this.tbx_worldAreaLayer = new System.Windows.Forms.TextBox();
            this.panel_MapViewer = new System.Windows.Forms.Panel();
            this.btn_SelectServerConfigPath = new System.Windows.Forms.Button();
            this.dig_serverConfigPath = new System.Windows.Forms.FolderBrowserDialog();
            this.panel_ControlOption.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_StartGenerate
            // 
            this.btn_StartGenerate.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btn_StartGenerate.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btn_StartGenerate.Location = new System.Drawing.Point(555, 282);
            this.btn_StartGenerate.Name = "btn_StartGenerate";
            this.btn_StartGenerate.Size = new System.Drawing.Size(185, 74);
            this.btn_StartGenerate.TabIndex = 0;
            this.btn_StartGenerate.Text = "Generate MapData";
            this.btn_StartGenerate.UseVisualStyleBackColor = false;
            this.btn_StartGenerate.Click += new System.EventHandler(this.btn_StartGenerate_Click);
            // 
            // tbx_subWorldRow
            // 
            this.tbx_subWorldRow.Location = new System.Drawing.Point(125, 131);
            this.tbx_subWorldRow.Name = "tbx_subWorldRow";
            this.tbx_subWorldRow.Size = new System.Drawing.Size(100, 21);
            this.tbx_subWorldRow.TabIndex = 1;
            // 
            // tbx_subWorldColumn
            // 
            this.tbx_subWorldColumn.Location = new System.Drawing.Point(125, 158);
            this.tbx_subWorldColumn.Name = "tbx_subWorldColumn";
            this.tbx_subWorldColumn.Size = new System.Drawing.Size(100, 21);
            this.tbx_subWorldColumn.TabIndex = 2;
            // 
            // tbx_subWorldLayer
            // 
            this.tbx_subWorldLayer.Location = new System.Drawing.Point(125, 185);
            this.tbx_subWorldLayer.Name = "tbx_subWorldLayer";
            this.tbx_subWorldLayer.Size = new System.Drawing.Size(100, 21);
            this.tbx_subWorldLayer.TabIndex = 3;
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Location = new System.Drawing.Point(16, 134);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(83, 12);
            this.lbl_name.TabIndex = 4;
            this.lbl_name.Text = "SubWorldRow";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "SubWorldColumn";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "SubWorldLayer";
            // 
            // tbx_logBox
            // 
            this.tbx_logBox.BackColor = System.Drawing.SystemColors.Info;
            this.tbx_logBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbx_logBox.Font = new System.Drawing.Font("MoeumT R", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbx_logBox.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.tbx_logBox.Location = new System.Drawing.Point(555, 12);
            this.tbx_logBox.Multiline = true;
            this.tbx_logBox.Name = "tbx_logBox";
            this.tbx_logBox.ReadOnly = true;
            this.tbx_logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbx_logBox.Size = new System.Drawing.Size(456, 244);
            this.tbx_logBox.TabIndex = 7;
            // 
            // btn_openSaveFilePathDig
            // 
            this.btn_openSaveFilePathDig.Location = new System.Drawing.Point(555, 368);
            this.btn_openSaveFilePathDig.Name = "btn_openSaveFilePathDig";
            this.btn_openSaveFilePathDig.Size = new System.Drawing.Size(143, 74);
            this.btn_openSaveFilePathDig.TabIndex = 8;
            this.btn_openSaveFilePathDig.Text = "Select SavePath";
            this.btn_openSaveFilePathDig.UseVisualStyleBackColor = true;
            this.btn_openSaveFilePathDig.Click += new System.EventHandler(this.btn_openSaveFilePathDig_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(519, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "LOG";
            // 
            // tbx_lastSavePath
            // 
            this.tbx_lastSavePath.Location = new System.Drawing.Point(154, 260);
            this.tbx_lastSavePath.Multiline = true;
            this.tbx_lastSavePath.Name = "tbx_lastSavePath";
            this.tbx_lastSavePath.ReadOnly = true;
            this.tbx_lastSavePath.Size = new System.Drawing.Size(344, 59);
            this.tbx_lastSavePath.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Lastest SavePath(Client)";
            // 
            // panel_ControlOption
            // 
            this.panel_ControlOption.Controls.Add(this.tbx_latestServerConfigPath);
            this.panel_ControlOption.Controls.Add(this.label8);
            this.panel_ControlOption.Controls.Add(this.label5);
            this.panel_ControlOption.Controls.Add(this.tbx_worldAreaRow);
            this.panel_ControlOption.Controls.Add(this.label6);
            this.panel_ControlOption.Controls.Add(this.label7);
            this.panel_ControlOption.Controls.Add(this.tbx_worldAreaColumn);
            this.panel_ControlOption.Controls.Add(this.lbl_name);
            this.panel_ControlOption.Controls.Add(this.tbx_worldAreaLayer);
            this.panel_ControlOption.Controls.Add(this.label4);
            this.panel_ControlOption.Controls.Add(this.tbx_subWorldRow);
            this.panel_ControlOption.Controls.Add(this.tbx_lastSavePath);
            this.panel_ControlOption.Controls.Add(this.label1);
            this.panel_ControlOption.Controls.Add(this.tbx_subWorldColumn);
            this.panel_ControlOption.Controls.Add(this.tbx_subWorldLayer);
            this.panel_ControlOption.Controls.Add(this.label2);
            this.panel_ControlOption.Location = new System.Drawing.Point(12, 263);
            this.panel_ControlOption.Name = "panel_ControlOption";
            this.panel_ControlOption.Size = new System.Drawing.Size(501, 411);
            this.panel_ControlOption.TabIndex = 12;
            // 
            // tbx_latestServerConfigPath
            // 
            this.tbx_latestServerConfigPath.Location = new System.Drawing.Point(154, 342);
            this.tbx_latestServerConfigPath.Multiline = true;
            this.tbx_latestServerConfigPath.Name = "tbx_latestServerConfigPath";
            this.tbx_latestServerConfigPath.ReadOnly = true;
            this.tbx_latestServerConfigPath.Size = new System.Drawing.Size(344, 57);
            this.tbx_latestServerConfigPath.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(-2, 342);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(149, 12);
            this.label8.TabIndex = 18;
            this.label8.Text = "Lastest SavePath(Server)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "WorldAreaRow";
            // 
            // tbx_worldAreaRow
            // 
            this.tbx_worldAreaRow.Location = new System.Drawing.Point(125, 27);
            this.tbx_worldAreaRow.Name = "tbx_worldAreaRow";
            this.tbx_worldAreaRow.Size = new System.Drawing.Size(100, 21);
            this.tbx_worldAreaRow.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 12);
            this.label6.TabIndex = 16;
            this.label6.Text = "WorldAreaColumn";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 12);
            this.label7.TabIndex = 17;
            this.label7.Text = "WorldAreaLayer";
            // 
            // tbx_worldAreaColumn
            // 
            this.tbx_worldAreaColumn.Location = new System.Drawing.Point(125, 54);
            this.tbx_worldAreaColumn.Name = "tbx_worldAreaColumn";
            this.tbx_worldAreaColumn.Size = new System.Drawing.Size(100, 21);
            this.tbx_worldAreaColumn.TabIndex = 13;
            // 
            // tbx_worldAreaLayer
            // 
            this.tbx_worldAreaLayer.Location = new System.Drawing.Point(125, 81);
            this.tbx_worldAreaLayer.Name = "tbx_worldAreaLayer";
            this.tbx_worldAreaLayer.Size = new System.Drawing.Size(100, 21);
            this.tbx_worldAreaLayer.TabIndex = 14;
            // 
            // panel_MapViewer
            // 
            this.panel_MapViewer.Location = new System.Drawing.Point(12, 12);
            this.panel_MapViewer.Name = "panel_MapViewer";
            this.panel_MapViewer.Size = new System.Drawing.Size(501, 245);
            this.panel_MapViewer.TabIndex = 13;
            this.panel_MapViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel_MapViewer_Paint);
            // 
            // btn_SelectServerConfigPath
            // 
            this.btn_SelectServerConfigPath.Location = new System.Drawing.Point(555, 448);
            this.btn_SelectServerConfigPath.Name = "btn_SelectServerConfigPath";
            this.btn_SelectServerConfigPath.Size = new System.Drawing.Size(153, 74);
            this.btn_SelectServerConfigPath.TabIndex = 14;
            this.btn_SelectServerConfigPath.Text = "Select ServerConfigPath";
            this.btn_SelectServerConfigPath.UseVisualStyleBackColor = true;
            this.btn_SelectServerConfigPath.Click += new System.EventHandler(this.Btn_SelectServerConfigPath_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1023, 686);
            this.Controls.Add(this.btn_SelectServerConfigPath);
            this.Controls.Add(this.panel_MapViewer);
            this.Controls.Add(this.panel_ControlOption);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_openSaveFilePathDig);
            this.Controls.Add(this.tbx_logBox);
            this.Controls.Add(this.btn_StartGenerate);
            this.Name = "MainForm";
            this.Text = "CustomMapTool";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel_ControlOption.ResumeLayout(false);
            this.panel_ControlOption.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_StartGenerate;
        private System.Windows.Forms.TextBox tbx_subWorldRow;
        private System.Windows.Forms.TextBox tbx_subWorldColumn;
        private System.Windows.Forms.TextBox tbx_subWorldLayer;
        private System.Windows.Forms.Label lbl_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbx_logBox;
        private System.Windows.Forms.FolderBrowserDialog dig_folderBrowser;
        private System.Windows.Forms.Button btn_openSaveFilePathDig;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbx_lastSavePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel_ControlOption;
        private System.Windows.Forms.Panel panel_MapViewer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbx_worldAreaRow;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbx_worldAreaColumn;
        private System.Windows.Forms.TextBox tbx_worldAreaLayer;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_SelectServerConfigPath;
        private System.Windows.Forms.FolderBrowserDialog dig_serverConfigPath;
        private System.Windows.Forms.TextBox tbx_latestServerConfigPath;
        private System.Windows.Forms.Label label8;
    }
}

