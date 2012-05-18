using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core.UI.Controls;
using Afterglow.Core.Plugins;
using System.Collections.ObjectModel;
using Afterglow.Core.UI;
using Afterglow.Plugins.ColourExtraction;

namespace Afterglow.UserControls
{
    public partial class ColourExtractionPluginSelectUserControl : BaseControl
    {
        private Core.Profile _profile;

        public ColourExtractionPluginSelectUserControl()
        {
            InitializeComponent();
        }

        public ColourExtractionPluginSelectUserControl(Core.Profile profile)
        {
            this._profile = profile;
            InitializeComponent();
        }

        private void ColourExtractionPluginSelectUserControl_Load(object sender, EventArgs e)
        {
            ObservableCollection<IColourExtractionPlugin> available = new ObservableCollection<IColourExtractionPlugin>();

            available.Add(new AverageColourExtraction());

            cboColourExtractions.DataSource = available;
            cboColourExtractions.DisplayMember = "DisplayName";
            cboColourExtractions.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboColourExtractions.DropDownStyle = ComboBoxStyle.DropDownList;
            cboColourExtractions.SelectedIndexChanged += new EventHandler(cboColourExtractions_SelectedIndexChanged);
        }

        void cboColourExtractions_SelectedIndexChanged(object sender, EventArgs e)
        {
            _profile.SetCapturePlugin(cboColourExtractions.SelectedItem.GetType());

            PluginsChangedEventArgs args = new PluginsChangedEventArgs();
            args.Plugins = new IAfterglowPlugin[] { _profile.CapturePlugin };
            OnPluginsChanged(args);
        }
    }
}
