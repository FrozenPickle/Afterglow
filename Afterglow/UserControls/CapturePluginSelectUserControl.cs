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
using System.Reflection;
using Afterglow.Core.Configuration;

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
            cmbCapturePlugins.DataSource = GetLookupValues();
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

        private IList<ICapturePlugin> GetLookupValues()
        {
            PropertyInfo prop = _profile.GetType().GetProperties().Where(p => p.Name == "CapturePlugin").FirstOrDefault();
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

            List<ICapturePlugin> result = new List<ICapturePlugin>();
            foreach (Type item in availableValues)
            {
                ICapturePlugin plugin = Activator.CreateInstance(item) as ICapturePlugin;
                result.Add(plugin);
            }

            return result;
        }
    }
}
