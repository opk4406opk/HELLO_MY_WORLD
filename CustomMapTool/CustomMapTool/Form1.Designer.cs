namespace CustomMapTool
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
            this.SuspendLayout();
            // 
            // btn_StartGenerate
            // 
            this.btn_StartGenerate.Location = new System.Drawing.Point(318, 133);
            this.btn_StartGenerate.Name = "btn_StartGenerate";
            this.btn_StartGenerate.Size = new System.Drawing.Size(185, 74);
            this.btn_StartGenerate.TabIndex = 0;
            this.btn_StartGenerate.Text = "Start";
            this.btn_StartGenerate.UseVisualStyleBackColor = true;
            this.btn_StartGenerate.Click += new System.EventHandler(this.btn_StartGenerate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 521);
            this.Controls.Add(this.btn_StartGenerate);
            this.Name = "MainForm";
            this.Text = "CustomMapTool";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_StartGenerate;
    }
}

