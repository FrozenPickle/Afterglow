namespace Afterglow.UserControls
{
    partial class ColourExtractionPluginSelectUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.cboColourExtractions = new System.Windows.Forms.ComboBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Colour Extraction Plugin Selection";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.cboColourExtractions);
            this.groupBox.Location = new System.Drawing.Point(6, 16);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(385, 185);
            this.groupBox.TabIndex = 2;
            this.groupBox.TabStop = false;
            // 
            // cboColourExtractions
            // 
            this.cboColourExtractions.FormattingEnabled = true;
            this.cboColourExtractions.Location = new System.Drawing.Point(6, 19);
            this.cboColourExtractions.Name = "cboColourExtractions";
            this.cboColourExtractions.Size = new System.Drawing.Size(373, 21);
            this.cboColourExtractions.TabIndex = 0;
            // 
            // ColourExtractionPluginSelectUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.label1);
            this.Name = "ColourExtractionPluginSelectUserControl";
            this.Size = new System.Drawing.Size(398, 206);
            this.Load += new System.EventHandler(this.ColourExtractionPluginSelectUserControl_Load);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.ComboBox cboColourExtractions;
    }
}
