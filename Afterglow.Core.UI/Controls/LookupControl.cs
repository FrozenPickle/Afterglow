using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Afterglow.Core.Plugins;
using Afterglow.Core.Configuration;

namespace Afterglow.Core.UI.Controls
{
    public partial class LookupControl : DataControl
    {
        private ComboBox _valueComboBox;
        private PropertyInfo _propertyInfo;
        private IAfterglowPlugin _plugin;

        public LookupControl(PropertyInfo prop, IAfterglowPlugin plugin) : base(prop)
        {
            ConfigLookupAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigLookupAttribute)) as ConfigLookupAttribute;

            if (configAttribute == null)
            {
                throw new Exception("prop is not a ConfigLookupAttribute");
            }
            _propertyInfo = prop;
            _plugin = plugin;

            //Create value Combo Box
            _valueComboBox = new ComboBox();
            _valueComboBox.Name = Guid.NewGuid().ToString();
            
            //Populate Combo and set current item
            IEnumerable<object> availableValues = GetLookupValues(prop, plugin, configAttribute);
            if (availableValues != null)
            {
                _valueComboBox.DataSource = availableValues;
                _valueComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                _valueComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else
            {
                plugin.Logger.Error("{0} could not be set as no lookup values could not be loaded", prop.Name);
            }
            this.Controls.Add(_valueComboBox);

            InitializeComponent();
        }

        private void LookupControl_Load(object sender, EventArgs e)
        {
            object value = _propertyInfo.GetValue(_plugin, null).ToString();
            object selectedItem = (from a in _valueComboBox.DataSource as IEnumerable<object>
                                   where a == value
                                   select a).FirstOrDefault();
            //an item must be selected
            if (selectedItem == null)
            {
                selectedItem = (from a in _valueComboBox.DataSource as IEnumerable<object>
                                   select a).FirstOrDefault();
            }

            //Only set the item if there is one to select
            if (selectedItem != null)
            {
                _valueComboBox.SelectedItem = selectedItem;
            }
            _valueComboBox.SelectionChangeCommitted += new EventHandler(_valueComboBox_SelectionChangeCommitted);
        }

        void _valueComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_propertyInfo.PropertyType.IsEnum)
            {
                string value = _valueComboBox.SelectedItem.ToString();
                object enumValue = Enum.Parse(_propertyInfo.PropertyType, value);
                _propertyInfo.SetValue(_plugin, enumValue, null);

            }
            else
            {
                _propertyInfo.SetValue(_plugin, _valueComboBox.SelectedItem, null);
            }
        }

        private IEnumerable<object> GetLookupValues(PropertyInfo prop, IAfterglowPlugin plugin, ConfigLookupAttribute configAttribute)
        {
            Type pluginType = plugin.GetType();
            Type propertyType = prop.PropertyType;

            string displayName = configAttribute.DisplayName;
            IEnumerable<object> availableValues = null;
            if (propertyType.IsEnum)
            {
                availableValues = propertyType.GetEnumNames() as IEnumerable<object>;
            }
            else if (configAttribute.RetrieveValuesFrom != null)
            {
                var member = pluginType.GetMember(configAttribute.RetrieveValuesFrom);
                if (member.Length > 0)
                {
                    if (member[0].MemberType == MemberTypes.Property)
                    {
                        PropertyInfo pi = pluginType.GetProperty(configAttribute.RetrieveValuesFrom);

                        var propertyValue = pi.GetValue(plugin, null);
                        if (typeof(IEnumerable<>).MakeGenericType(propertyType).IsAssignableFrom(propertyValue.GetType()))
                        {
                            availableValues = propertyValue as IEnumerable<object>;
                        }
                        else
                        {
                            throw new ArgumentException("incorrect type", "RetrieveValuesFrom");
                        }

                    }
                    else if (member[0].MemberType == MemberTypes.Method)
                    {
                        MethodInfo mi = pluginType.GetMethod(configAttribute.RetrieveValuesFrom);

                        var propertyValue = mi.Invoke(plugin, null);
                        if (typeof(IEnumerable<>).MakeGenericType(propertyType).IsAssignableFrom(propertyValue.GetType()))
                        {
                            availableValues = propertyValue as IEnumerable<object>;
                        }
                        else
                        {
                            throw new ArgumentException("incorrect type", "RetrieveValuesFrom");
                        }
                    }
                }
            }
            return availableValues;
        }

        private void ControlResize(object sender, EventArgs e)
        {
            if (_valueComboBox != null)
            {
                _valueComboBox.Top = 0;
                _valueComboBox.Left = Convert.ToInt32(this.Width * LABEL_WIDTH);
                _valueComboBox.Width = Convert.ToInt32(this.Width * VALUE_WIDTH);
            }
        }
    }
}
