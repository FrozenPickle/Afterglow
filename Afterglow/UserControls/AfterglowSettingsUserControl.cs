using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core;
using Afterglow.Core.UI.Controls;
using Afterglow.Core.Plugins;
using System.Reflection;
using Afterglow.Core.UI;

namespace Afterglow.UserControls
{
    public partial class AfterglowSettingsUserControl : BaseControl
    {
        private AfterglowRuntime _runtime;

        public AfterglowSettingsUserControl()
        {
            InitializeComponent();
        }

        public AfterglowSettingsUserControl(AfterglowRuntime _runtime)
        {
            InitializeComponent();
            this._runtime = _runtime;
        }

        private void AfterglowSettingsUserControlLoad(object sender, EventArgs e)
        {
            lbProfiles.DataSource = _runtime.Settings.Profiles;
            lbProfiles.DisplayMember = "Name";

            tbProfileName.DataBindings.Add("Text", _runtime.Settings.Profiles, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void tbProfileName_Validated(object sender, EventArgs e)
        {
            PluginsChanged();
        }

        private new void PluginsChanged()
        {
            ((CurrencyManager)lbProfiles.BindingContext[lbProfiles.DataSource]).Refresh();
            PluginsChangedEventArgs args = new PluginsChangedEventArgs();
            args.RefreshProfiles = true;
            OnPluginsChanged(args);
        }
        
        private void chkShowPreview_CheckedChanged(object sender, EventArgs e)
        {
            _runtime.ShowPreview = chkShowPreview.Checked;
        }

        private void btnAddProfile_Click(object sender, EventArgs e)
        {
            Profile newProfile = _runtime.Settings.AddProfile();
            PluginsChanged();
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            int index = lbProfiles.SelectedIndex;

            Profile profile = lbProfiles.SelectedItem as Profile;
            _runtime.Settings.RemoveProfile(profile);

            if (lbProfiles.Items.Count == 0)
            {
                //select nothing
            }
            else if (lbProfiles.Items.Count - 1 <= index)
            {
                lbProfiles.SelectedIndex = index - 1;
            }
            else
            {
                lbProfiles.SelectedIndex = index;
            }
            PluginsChanged();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            Profile itemToMove = lbProfiles.SelectedItem as Profile;
            int selectedIndex = lbProfiles.SelectedIndex;

            _runtime.Settings.Profiles.RemoveAt(selectedIndex);
            _runtime.Settings.Profiles.Insert(selectedIndex - 1, itemToMove);

            lbProfiles.SelectedIndex = selectedIndex - 1;
            PluginsChanged();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            Profile itemToMove = lbProfiles.SelectedItem as Profile;
            int selectedIndex = lbProfiles.SelectedIndex;

            _runtime.Settings.Profiles.RemoveAt(selectedIndex);
            _runtime.Settings.Profiles.Insert(selectedIndex + 1, itemToMove);

            lbProfiles.SelectedIndex = selectedIndex + 1;
            PluginsChanged();
        }

        private void lbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            ButtonsEnabledState();
        }

        private void ButtonsEnabledState()
        {
            btnDeleteProfile.Enabled = (lbProfiles.SelectedItem != null);

            btnUp.Enabled = (lbProfiles.SelectedItem != null && lbProfiles.Items.Count > 1 && lbProfiles.SelectedIndex > 0);
            btnDown.Enabled = (lbProfiles.SelectedItem != null && lbProfiles.Items.Count > 1 && lbProfiles.SelectedIndex + 1 < lbProfiles.Items.Count);
        }
    }
}
