namespace Afterglow.Plugins.LightSetup.BasicLightSetupPlugin
{
    partial class BasicLightSetupUserControl
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
            this.gbRegions = new System.Windows.Forms.GroupBox();
            this.dgvRegions = new System.Windows.Forms.DataGridView();
            this.nudHigh = new System.Windows.Forms.NumericUpDown();
            this.nudWide = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gbCurrentRegion = new System.Windows.Forms.GroupBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.nudIndex = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.nudTop = new System.Windows.Forms.NumericUpDown();
            this.nudLeft = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.nudScreen = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.gbRegions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWide)).BeginInit();
            this.gbCurrentRegion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // gbRegions
            // 
            this.gbRegions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbRegions.Controls.Add(this.dgvRegions);
            this.gbRegions.Location = new System.Drawing.Point(6, 97);
            this.gbRegions.Name = "gbRegions";
            this.gbRegions.Padding = new System.Windows.Forms.Padding(0);
            this.gbRegions.Size = new System.Drawing.Size(401, 232);
            this.gbRegions.TabIndex = 33;
            this.gbRegions.TabStop = false;
            this.gbRegions.Text = "Light Regions";
            // 
            // dgvRegions
            // 
            this.dgvRegions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRegions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRegions.Location = new System.Drawing.Point(3, 16);
            this.dgvRegions.Name = "dgvRegions";
            this.dgvRegions.Size = new System.Drawing.Size(395, 213);
            this.dgvRegions.TabIndex = 1;
            // 
            // nudHigh
            // 
            this.nudHigh.Location = new System.Drawing.Point(138, 33);
            this.nudHigh.Name = "nudHigh";
            this.nudHigh.Size = new System.Drawing.Size(55, 20);
            this.nudHigh.TabIndex = 32;
            // 
            // nudWide
            // 
            this.nudWide.Location = new System.Drawing.Point(138, 7);
            this.nudWide.Name = "nudWide";
            this.nudWide.Size = new System.Drawing.Size(55, 20);
            this.nudWide.TabIndex = 31;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 29;
            this.label4.Text = "Capture Positions High";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 13);
            this.label3.TabIndex = 28;
            this.label3.Text = "Capture Positions Wide";
            // 
            // gbCurrentRegion
            // 
            this.gbCurrentRegion.Controls.Add(this.btnCreate);
            this.gbCurrentRegion.Controls.Add(this.btnDelete);
            this.gbCurrentRegion.Controls.Add(this.nudWidth);
            this.gbCurrentRegion.Controls.Add(this.label7);
            this.gbCurrentRegion.Controls.Add(this.nudHeight);
            this.gbCurrentRegion.Controls.Add(this.nudIndex);
            this.gbCurrentRegion.Controls.Add(this.label5);
            this.gbCurrentRegion.Controls.Add(this.label6);
            this.gbCurrentRegion.Controls.Add(this.nudTop);
            this.gbCurrentRegion.Controls.Add(this.nudLeft);
            this.gbCurrentRegion.Controls.Add(this.label1);
            this.gbCurrentRegion.Controls.Add(this.label2);
            this.gbCurrentRegion.Location = new System.Drawing.Point(199, 3);
            this.gbCurrentRegion.Name = "gbCurrentRegion";
            this.gbCurrentRegion.Size = new System.Drawing.Size(208, 95);
            this.gbCurrentRegion.TabIndex = 34;
            this.gbCurrentRegion.TabStop = false;
            this.gbCurrentRegion.Text = "Current Light Region";
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(125, 13);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(74, 23);
            this.btnCreate.TabIndex = 45;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(125, 13);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 44;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // nudWidth
            // 
            this.nudWidth.Location = new System.Drawing.Point(145, 68);
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(55, 20);
            this.nudWidth.TabIndex = 42;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(106, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 41;
            this.label7.Text = "Width";
            // 
            // nudHeight
            // 
            this.nudHeight.Location = new System.Drawing.Point(145, 42);
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(55, 20);
            this.nudHeight.TabIndex = 40;
            // 
            // nudIndex
            // 
            this.nudIndex.Location = new System.Drawing.Point(45, 16);
            this.nudIndex.Name = "nudIndex";
            this.nudIndex.Size = new System.Drawing.Size(55, 20);
            this.nudIndex.TabIndex = 39;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(106, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Height";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 37;
            this.label6.Text = "Index";
            // 
            // nudTop
            // 
            this.nudTop.Location = new System.Drawing.Point(45, 42);
            this.nudTop.Name = "nudTop";
            this.nudTop.Size = new System.Drawing.Size(55, 20);
            this.nudTop.TabIndex = 36;
            // 
            // nudLeft
            // 
            this.nudLeft.Location = new System.Drawing.Point(45, 68);
            this.nudLeft.Name = "nudLeft";
            this.nudLeft.Size = new System.Drawing.Size(55, 20);
            this.nudLeft.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 34;
            this.label1.Text = "Top";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Left";
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(6, 75);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(85, 23);
            this.btnDeleteAll.TabIndex = 35;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // nudScreen
            // 
            this.nudScreen.Location = new System.Drawing.Point(138, 59);
            this.nudScreen.Name = "nudScreen";
            this.nudScreen.Size = new System.Drawing.Size(55, 20);
            this.nudScreen.TabIndex = 37;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 59);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Screen To Capture";
            // 
            // BasicLightSetupUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudScreen);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.gbCurrentRegion);
            this.Controls.Add(this.gbRegions);
            this.Controls.Add(this.nudHigh);
            this.Controls.Add(this.nudWide);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Name = "BasicLightSetupUserControl";
            this.Size = new System.Drawing.Size(413, 336);
            this.Load += new System.EventHandler(this.BasicLightSetupUserControl_Load);
            this.gbRegions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRegions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHigh)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWide)).EndInit();
            this.gbCurrentRegion.ResumeLayout(false);
            this.gbCurrentRegion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudScreen)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbRegions;
        private System.Windows.Forms.NumericUpDown nudHigh;
        private System.Windows.Forms.NumericUpDown nudWide;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvRegions;
        private System.Windows.Forms.GroupBox gbCurrentRegion;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.NumericUpDown nudIndex;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudTop;
        private System.Windows.Forms.NumericUpDown nudLeft;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.NumericUpDown nudScreen;
        private System.Windows.Forms.Label label8;
    }
}
