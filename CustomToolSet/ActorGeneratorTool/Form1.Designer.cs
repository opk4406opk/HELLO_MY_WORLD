namespace ActorGeneratorTool
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
            this.btn_GenerateMonsterData = new System.Windows.Forms.Button();
            this.btn_GenerateNPCData = new System.Windows.Forms.Button();
            this.cbx_IsDefaultGenerate = new System.Windows.Forms.CheckBox();
            this.dig_SelectSavePath = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_SelectPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_GenerateMonsterData
            // 
            this.btn_GenerateMonsterData.Location = new System.Drawing.Point(46, 600);
            this.btn_GenerateMonsterData.Name = "btn_GenerateMonsterData";
            this.btn_GenerateMonsterData.Size = new System.Drawing.Size(178, 59);
            this.btn_GenerateMonsterData.TabIndex = 1;
            this.btn_GenerateMonsterData.Text = "Generate MonData";
            this.btn_GenerateMonsterData.UseVisualStyleBackColor = true;
            this.btn_GenerateMonsterData.Click += new System.EventHandler(this.btn_GenerateMonsterData_Click);
            // 
            // btn_GenerateNPCData
            // 
            this.btn_GenerateNPCData.Location = new System.Drawing.Point(335, 600);
            this.btn_GenerateNPCData.Name = "btn_GenerateNPCData";
            this.btn_GenerateNPCData.Size = new System.Drawing.Size(182, 59);
            this.btn_GenerateNPCData.TabIndex = 0;
            this.btn_GenerateNPCData.Text = "Generate NPCData";
            this.btn_GenerateNPCData.UseVisualStyleBackColor = true;
            this.btn_GenerateNPCData.Click += new System.EventHandler(this.btn_GenerateNPCData_Click);
            // 
            // cbx_IsDefaultGenerate
            // 
            this.cbx_IsDefaultGenerate.AutoSize = true;
            this.cbx_IsDefaultGenerate.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.cbx_IsDefaultGenerate.Location = new System.Drawing.Point(335, 466);
            this.cbx_IsDefaultGenerate.Name = "cbx_IsDefaultGenerate";
            this.cbx_IsDefaultGenerate.Size = new System.Drawing.Size(146, 16);
            this.cbx_IsDefaultGenerate.TabIndex = 2;
            this.cbx_IsDefaultGenerate.Text = "Default Generate Data";
            this.cbx_IsDefaultGenerate.UseVisualStyleBackColor = true;
            this.cbx_IsDefaultGenerate.CheckedChanged += new System.EventHandler(this.cbx_IsDefaultGenerate_CheckedChanged);
            // 
            // btn_SelectPath
            // 
            this.btn_SelectPath.Location = new System.Drawing.Point(209, 330);
            this.btn_SelectPath.Name = "btn_SelectPath";
            this.btn_SelectPath.Size = new System.Drawing.Size(127, 85);
            this.btn_SelectPath.TabIndex = 3;
            this.btn_SelectPath.Text = "Select SavePath";
            this.btn_SelectPath.UseVisualStyleBackColor = true;
            this.btn_SelectPath.Click += new System.EventHandler(this.btn_SelectPath_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 701);
            this.Controls.Add(this.btn_SelectPath);
            this.Controls.Add(this.cbx_IsDefaultGenerate);
            this.Controls.Add(this.btn_GenerateMonsterData);
            this.Controls.Add(this.btn_GenerateNPCData);
            this.Name = "MainForm";
            this.Text = "ActorGenerator";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_GenerateMonsterData;
        private System.Windows.Forms.Button btn_GenerateNPCData;
        private System.Windows.Forms.CheckBox cbx_IsDefaultGenerate;
        private System.Windows.Forms.FolderBrowserDialog dig_SelectSavePath;
        private System.Windows.Forms.Button btn_SelectPath;
    }
}

