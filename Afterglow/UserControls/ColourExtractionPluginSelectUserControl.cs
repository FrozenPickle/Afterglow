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
using System.Reflection;
using Afterglow.Core.Configuration;

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
            cboColourExtractions.DataSource = GetLookupValues();
            cboColourExtractions.DisplayMember = "DisplayName";
            cboColourExtractions.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboColourExtractions.DropDownStyle = ComboBoxStyle.DropDownList;
            cboColourExtractions.SelectedIndexChanged += new EventHandler(cboColourExtractions_SelectedIndexChanged);
        }

        void cboColourExtractions_SelectedIndexChanged(object sender, EventArgs e)
        {
            _profile.SetColourExtractionPlugin(cboColourExtractions.SelectedItem.GetType());

            PluginsChangedEventArgs args = new PluginsChangedEventArgs();
            args.Plugins = new IAfterglowPlugin[] { _profile.CapturePlugin };
            OnPluginsChanged(args);
        }

        private IList<IColourExtractionPlugin> GetLookupValues()
        {
            PropertyInfo prop = _profile.GetType().GetProperties().Where(p => p.Name == "ColourExtractionPlugin").FirstOrDefault();
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

            List<IColourExtractionPlugin> result = new List<IColourExtractionPlugin>();
            foreach (Type item in availableValues)
            {
                IColourExtractionPlugin plugin = Activator.CreateInstance(item) as IColourExtractionPlugin;
                result.Add(plugin);
            }

            return result;
        }
    }
}
