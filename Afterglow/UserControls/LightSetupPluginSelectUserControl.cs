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
using System.Reflection;
using Afterglow.Core.Configuration;

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

            cboLightSetups.DataSource = GetLookupValues();
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

        private IList<ILightSetupPlugin> GetLookupValues()
        {
            PropertyInfo prop = _profile.GetType().GetProperties().Where(p => p.Name == "LightSetupPlugin").FirstOrDefault();
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

            List<ILightSetupPlugin> result = new List<ILightSetupPlugin>();
            foreach (Type item in availableValues)
            {
                ILightSetupPlugin plugin = Activator.CreateInstance(item) as ILightSetupPlugin;
                result.Add(plugin);
            }

            return result;
        }
    }
}
