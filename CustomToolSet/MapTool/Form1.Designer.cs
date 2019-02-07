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
            this.tbx_row = new System.Windows.Forms.TextBox();
            this.tbx_column = new System.Windows.Forms.TextBox();
            this.tbx_layer = new System.Windows.Forms.TextBox();
            this.lbl_name = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbx_logBox = new System.Windows.Forms.TextBox();
            this.dig_folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.btn_openSaveFilePathDig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_StartGenerate
            // 
            this.btn_StartGenerate.Location = new System.Drawing.Point(53, 269);
            this.btn_StartGenerate.Name = "btn_StartGenerate";
            this.btn_StartGenerate.Size = new System.Drawing.Size(185, 74);
            this.btn_StartGenerate.TabIndex = 0;
            this.btn_StartGenerate.Text = "Generate MapData";
            this.btn_StartGenerate.UseVisualStyleBackColor = true;
            this.btn_StartGenerate.Click += new System.EventHandler(this.btn_StartGenerate_Click);
            // 
            // tbx_row
            // 
            this.tbx_row.Location = new System.Drawing.Point(100, 35);
            this.tbx_row.Name = "tbx_row";
            this.tbx_row.Size = new System.Drawing.Size(100, 21);
            this.tbx_row.TabIndex = 1;
            // 
            // tbx_column
            // 
            this.tbx_column.Location = new System.Drawing.Point(100, 76);
            this.tbx_column.Name = "tbx_column";
            this.tbx_column.Size = new System.Drawing.Size(100, 21);
            this.tbx_column.TabIndex = 2;
            // 
            // tbx_layer
            // 
            this.tbx_layer.Location = new System.Drawing.Point(100, 118);
            this.tbx_layer.Name = "tbx_layer";
            this.tbx_layer.Size = new System.Drawing.Size(100, 21);
            this.tbx_layer.TabIndex = 3;
            // 
            // lbl_name
            // 
            this.lbl_name.AutoSize = true;
            this.lbl_name.Location = new System.Drawing.Point(58, 35);
            this.lbl_name.Name = "lbl_name";
            this.lbl_name.Size = new System.Drawing.Size(30, 12);
            this.lbl_name.TabIndex = 4;
            this.lbl_name.Text = "Row";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Column";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Layer";
            // 
            // tbx_logBox
            // 
            this.tbx_logBox.BackColor = System.Drawing.SystemColors.Info;
            this.tbx_logBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbx_logBox.Font = new System.Drawing.Font("MoeumT R", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbx_logBox.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.tbx_logBox.Location = new System.Drawing.Point(231, 32);
            this.tbx_logBox.Multiline = true;
            this.tbx_logBox.Name = "tbx_logBox";
            this.tbx_logBox.ReadOnly = true;
            this.tbx_logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbx_logBox.Size = new System.Drawing.Size(366, 208);
            this.tbx_logBox.TabIndex = 7;
            // 
            // btn_openSaveFilePathDig
            // 
            this.btn_openSaveFilePathDig.Location = new System.Drawing.Point(323, 269);
            this.btn_openSaveFilePathDig.Name = "btn_openSaveFilePathDig";
            this.btn_openSaveFilePathDig.Size = new System.Drawing.Size(143, 74);
            this.btn_openSaveFilePathDig.TabIndex = 8;
            this.btn_openSaveFilePathDig.Text = "Select SavePath";
            this.btn_openSaveFilePathDig.UseVisualStyleBackColor = true;
            this.btn_openSaveFilePathDig.Click += new System.EventHandler(this.btn_openSaveFilePathDig_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(609, 521);
            this.Controls.Add(this.btn_openSaveFilePathDig);
            this.Controls.Add(this.tbx_logBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_name);
            this.Controls.Add(this.tbx_layer);
            this.Controls.Add(this.tbx_column);
            this.Controls.Add(this.tbx_row);
            this.Controls.Add(this.btn_StartGenerate);
            this.Name = "MainForm";
            this.Text = "CustomMapTool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_StartGenerate;
        private System.Windows.Forms.TextBox tbx_row;
        private System.Windows.Forms.TextBox tbx_column;
        private System.Windows.Forms.TextBox tbx_layer;
        private System.Windows.Forms.Label lbl_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbx_logBox;
        private System.Windows.Forms.FolderBrowserDialog dig_folderBrowser;
        private System.Windows.Forms.Button btn_openSaveFilePathDig;
    }
}

