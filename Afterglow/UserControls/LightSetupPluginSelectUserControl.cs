using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Afterglow.Core.UI.Controls;
using Afterglow.Core.Plugins;
using Afterglow.Core.UI;
using Afterglow.Plugins.LightSetup;
using Afterglow.Plugins.LightSetup.BasicLightSetupPlugin;

namespace Afterglow.UserControls
{
    public partial class LightSetupPluginSelectUserControl : BaseControl
    {
        private Core.Profile _profile;

        public LightSetupPluginSelectUserControl()
        {
            InitializeComponent();
        }

        public LightSetupPluginSelectUserControl(Core.Profile profile)
        {
            this._profile = profile;
            InitializeComponent();
        }

        private void LightSetupPluginSelectUserControl_Load(object sender, EventArgs e)
        {
            ObservableCollection<ILightSetupPlugin> available = new ObservableCollection<ILightSetupPlugin>();

            available.Add(new BasicLightSetup());
            
            cboLightSetups.DataSource = available;
            cboLightSetups.DisplayMember = "DisplayName";
            cboLightSetups.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboLightSetups.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLightSetups.SelectedIndexChanged += new EventHandler(cboLightSetups_SelectedIndexChanged);
        }

        void cboLightSetups_SelectedIndexChanged(object sender, EventArgs e)
        {
            _profile.SetCapturePlugin(cboLightSetups.SelectedItem.GetType());
            
            PluginsChangedEventArgs args = new PluginsChangedEventArgs();
            args.Plugins = new IAfterglowPlugin[]{_profile.CapturePlugin};
            OnPluginsChanged(args);
        }

    }
}
