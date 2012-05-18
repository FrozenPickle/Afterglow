using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Core.UI;
using Afterglow.Plugins.Output;
using Afterglow.Core.UI.Controls;
using System.Collections.ObjectModel;

namespace Afterglow.UserControls
{

    public partial class OutputPluginSelectUserControl : BaseControl
    {
        private Profile _profile;

        public OutputPluginSelectUserControl()
        {
            InitializeComponent();
        }

        public OutputPluginSelectUserControl(Profile profile)
        {
            this._profile = profile;
            InitializeComponent();
        }

        private void OutputPluginSelectUserControl_Load(object sender, EventArgs e)
        {
            ObservableCollection<IOutputPlugin> available = new ObservableCollection<IOutputPlugin>();

            available.Add(new ArduinoOutput());
            available.Add(new DebugOutput());

            LoadLists(_profile.OutputPlugins, available);
        }

        #region Available Selected

        public void LoadLists(ObservableCollection<IOutputPlugin> selected, ObservableCollection<IOutputPlugin> available)
        {
            lbSelected.DataSource = selected;
            lbSelected.DisplayMember = "DisplayName";

            lbAvailable.DataSource = available;
            lbAvailable.DisplayMember = "DisplayName";

            ButtonsEnabledState();

            PluginsChanged();
        }

        private new void PluginsChanged()
        {
            ((CurrencyManager)lbSelected.BindingContext[lbSelected.DataSource]).Refresh();
            PluginsChangedEventArgs args = new PluginsChangedEventArgs();
            args.Plugins = _profile.OutputPlugins.ToArray();
            OnPluginsChanged(args);
        }

        private void ButtonsEnabledState()
        {
            btnSelect.Enabled = (lbAvailable.SelectedItem != null);
            btnRemove.Enabled = (lbSelected.SelectedItem != null);

            btnUp.Enabled = (lbSelected.SelectedItem != null && lbSelected.Items.Count > 1 && lbSelected.SelectedIndex > 0);
            btnDown.Enabled = (lbSelected.SelectedItem != null && lbSelected.Items.Count > 1 && lbSelected.SelectedIndex + 1 < lbSelected.Items.Count);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            _profile.AddOutputPlugin(lbAvailable.SelectedItem.GetType());

            PluginsChanged();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index = lbSelected.SelectedIndex;

            _profile.RemoveOutputPlugin(lbSelected.SelectedItem as IOutputPlugin);

            if (lbSelected.Items.Count == 0)
            {
                //select nothing
            }
            else if (lbSelected.Items.Count - 1 <= index)
            {
                lbSelected.SelectedIndex = index - 1;
            }
            else
            {
                lbSelected.SelectedIndex = index;
            }
            PluginsChanged();
        }

        private void lbAvailable_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonsEnabledState();
        }

        private void lbSelected_SelectedValueChanged(object sender, EventArgs e)
        {
            ButtonsEnabledState();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            IOutputPlugin itemToMove = lbSelected.SelectedItem as IOutputPlugin;
            int selectedIndex = lbSelected.SelectedIndex;

            _profile.OutputPlugins.RemoveAt(selectedIndex);
            _profile.OutputPlugins.Insert(selectedIndex + 1, itemToMove);

            lbSelected.SelectedIndex = selectedIndex + 1;
            PluginsChanged();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            IOutputPlugin itemToMove = lbSelected.SelectedItem as IOutputPlugin;
            int selectedIndex = lbSelected.SelectedIndex;

            _profile.OutputPlugins.RemoveAt(selectedIndex);
            _profile.OutputPlugins.Insert(selectedIndex - 1, itemToMove);

            lbSelected.SelectedIndex = selectedIndex -1;
            PluginsChanged();
        }
        #endregion
    }
}
