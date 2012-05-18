namespace Afterglow.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tvSettings = new System.Windows.Forms.TreeView();
            this.panelPlaceHolder = new System.Windows.Forms.Panel();
            this.lblCurrentSetting = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tvSettings
            // 
            this.tvSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvSettings.Location = new System.Drawing.Point(12, 12);
            this.tvSettings.Name = "tvSettings";
            this.tvSettings.Size = new System.Drawing.Size(237, 438);
            this.tvSettings.TabIndex = 0;
            this.tvSettings.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSettings_AfterSelect);
            // 
            // panelPlaceHolder
            // 
            this.panelPlaceHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPlaceHolder.Location = new System.Drawing.Point(255, 45);
            this.panelPlaceHolder.Name = "panelPlaceHolder";
            this.panelPlaceHolder.Size = new System.Drawing.Size(434, 405);
            this.panelPlaceHolder.TabIndex = 1;
            // 
            // lblCurrentSetting
            // 
            this.lblCurrentSetting.AutoSize = true;
            this.lblCurrentSetting.Location = new System.Drawing.Point(255, 12);
            this.lblCurrentSetting.Name = "lblCurrentSetting";
            this.lblCurrentSetting.Size = new System.Drawing.Size(84, 13);
            this.lblCurrentSetting.TabIndex = 2;
            this.lblCurrentSetting.Text = "Currently Setting";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 462);
            this.Controls.Add(this.lblCurrentSetting);
            this.Controls.Add(this.panelPlaceHolder);
            this.Controls.Add(this.tvSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Afterglow Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvSettings;
        private System.Windows.Forms.Panel panelPlaceHolder;
        private System.Windows.Forms.Label lblCurrentSetting;
    }
}