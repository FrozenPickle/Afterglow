using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Plugins.Capture;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Core.UI.Controls;
using Afterglow.Core.UI;
using System.Collections.ObjectModel;

namespace Afterglow.UserControls
{
    public partial class CapturePluginSelectUserControl : BaseControl
    {
        private Profile _profile;

        public CapturePluginSelectUserControl()
        {
            InitializeComponent();
        }

        public CapturePluginSelectUserControl(Profile profile)
        {
            this._profile = profile;
            InitializeComponent();
        }
        
        private void CapturePluginSelectUserControl_Load(object sender, EventArgs e)
        {
            ObservableCollection<ICapturePlugin> available = new ObservableCollection<ICapturePlugin>();

            available.Add(new CopyScreenCapture());

            cmbCapturePlugins.DataSource = available;
            cmbCapturePlugins.DisplayMember = "DisplayName";
            cmbCapturePlugins.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCapturePlugins.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCapturePlugins.SelectedIndexChanged += new EventHandler(cmbCapturePlugins_SelectedIndexChanged);
        }

        void cmbCapturePlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            _profile.SetCapturePlugin(cmbCapturePlugins.SelectedItem.GetType());
            
            PluginsChangedEventArgs args = new PluginsChangedEventArgs();
            args.Plugins = new IAfterglowPlugin[]{_profile.CapturePlugin};
            OnPluginsChanged(args);
        }
    }
}
