namespace ZAxisLevelTool
{
    partial class Form1
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
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "Dave Mirra Freestyle BMX 2",
            "Aggressive Inline",
            "BMXXX"});
			this.comboBox1.Location = new System.Drawing.Point(16, 13);
			this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(345, 22);
			this.comboBox1.TabIndex = 0;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.ForestGreen;
			this.label1.Location = new System.Drawing.Point(16, 39);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(173, 56);
			this.label1.TabIndex = 1;
			this.label1.Text = "0) Load .TXR file (optional)\r\n1) Load .GEO file\r\n2) Load .INS file\r\n3) Ctrl+S sav" +
    "e as .OBJ";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Verdana", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.HotPink;
			this.label2.Location = new System.Drawing.Point(13, 107);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(353, 166);
			this.label2.TabIndex = 2;
			this.label2.Text = "note: don\'t expect anything to work properly!";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(379, 282);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "Form1";
			this.Text = "drop files here";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
    }
}

