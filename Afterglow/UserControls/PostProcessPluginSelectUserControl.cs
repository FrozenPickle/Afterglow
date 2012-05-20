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
using Afterglow.Core.UI.Controls;
using System.Collections.ObjectModel;
using System.Reflection;
using Afterglow.Core.Configuration;

namespace Afterglow.UserControls
{
    public partial class PostProcessPluginSelectUserControl : BaseControl
    {
        private Profile _profile;

        public PostProcessPluginSelectUserControl()
        {
            InitializeComponent();
        }

        public PostProcessPluginSelectUserControl(Profile profile)
        {
            this._profile = profile;
            InitializeComponent();
        }

        private void PostProcessPluginSelectUserControl_Load(object sender, EventArgs e)
        {
            LoadLists(_profile.PostProcessPlugins, GetLookupValues());
        }

        #region Available Selected

        public void LoadLists(ObservableCollection<IPostProcessPlugin> selected, ObservableCollection<IPostProcessPlugin> available)
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
            args.Plugins = _profile.PostProcessPlugins.ToArray();
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
            _profile.AddPostProcessPlugin(lbAvailable.SelectedItem.GetType());

            PluginsChanged();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index = lbSelected.SelectedIndex;

            _profile.RemovePostProcessPlugin(lbSelected.SelectedItem as IPostProcessPlugin);

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
            IPostProcessPlugin itemToMove = lbSelected.SelectedItem as IPostProcessPlugin;
            int selectedIndex = lbSelected.SelectedIndex;

            _profile.PostProcessPlugins.Move(selectedIndex + 1, selectedIndex);

            lbSelected.SelectedIndex = selectedIndex + 1;
            PluginsChanged();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            IPostProcessPlugin itemToMove = lbSelected.SelectedItem as IPostProcessPlugin;
            int selectedIndex = lbSelected.SelectedIndex;

            _profile.PostProcessPlugins.Move(selectedIndex, selectedIndex - 1);

            lbSelected.SelectedIndex = selectedIndex - 1;
            PluginsChanged();
        }
        #endregion

        private ObservableCollection<IPostProcessPlugin> GetLookupValues()
        {
            PropertyInfo prop = _profile.GetType().GetProperties().Where(p => p.Name == "PostProcessPlugins").FirstOrDefault();
            ConfigTableAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigTableAttribute)) as ConfigTableAttribute;

            Type pluginType = _profile.GetType();
            Type propertyType = prop.PropertyType;

            IEnumerable<Type> availableValues = null;
            if (configAttribute.RetrieveValuesFrom != null)
            {
                var member = pluginType.GetMember(configAttribute.RetrieveValuesFrom);
                if (member.Length > 0)
                {
                    if (member[0].MemberType == MemberTypes.Method)
                    {
                        MethodInfo mi = pluginType.GetMethod(configAttribute.RetrieveValuesFrom);

                        var propertyValue = mi.Invoke(_profile, null);

                        availableValues = propertyValue as IEnumerable<Type>;
                    }
                }
            }

            ObservableCollection<IPostProcessPlugin> result = new ObservableCollection<IPostProcessPlugin>();
            foreach (Type item in availableValues)
            {
                IPostProcessPlugin plugin = Activator.CreateInstance(item) as IPostProcessPlugin;
                result.Add(plugin);
            }

            return result;
        }

    }
}
