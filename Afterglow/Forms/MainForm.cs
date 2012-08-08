using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Plugins.Capture;
using Afterglow.Plugins.ColourExtraction;
using Afterglow.Plugins.Output;

namespace Afterglow.Forms
{
    public partial class MainForm : Form
    {
        private AfterglowRuntime _runtime;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(AfterglowRuntime _runtime)
        {
            this._runtime = _runtime;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadListBox();
        }

        private void LoadListBox()
        {
            lbProfiles.DataSource = _runtime.Settings.Profiles;
            lbProfiles.DisplayMember = "Name";
            lbProfiles.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnSettings.Enabled = false;
            this.AcceptButton = btnStop;
            this.SelectNextControl(this, true, true, false, true);
            
            _runtime.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _runtime.Stop();

            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnSettings.Enabled = true;
            this.AcceptButton = btnStart;
            this.SelectNextControl(this, false, true, false, true);
        }

        private void chkShowPreview_CheckedChanged(object sender, EventArgs e)
        {
            this.Height = (_runtime.ShowPreview ? 400 : 123);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            //if (_settingsForm == null)
            //{
            //    _settingsForm = new SettingsForm(_runtime, this);
            //}
            //_settingsForm.ShowDialog(this);
        }

        private void lbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_runtime is null on exit of application
            //lbProfiles.SelectedItem could be null if all profiles have been removed
            if (_runtime != null && lbProfiles.SelectedItem != null)
            {
                _runtime.CurrentProfile = lbProfiles.SelectedItem as Profile;
            }
        }

        internal void RefreshProfiles()
        {
            ((CurrencyManager)lbProfiles.BindingContext[lbProfiles.DataSource]).Refresh();
        }
    }
}
