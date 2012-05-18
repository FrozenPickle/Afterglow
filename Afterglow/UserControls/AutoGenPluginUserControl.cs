using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core.Configuration;
using Afterglow.Core.Plugins;
using Afterglow.Plugins;
using System.Reflection;
using Afterglow.Core.UI.Controls;

namespace Afterglow.UserControls
{
    public partial class AutoGenPluginUserControl : BaseControl
    {
        private IAfterglowPlugin _plugin;

        public AutoGenPluginUserControl()
        {
            InitializeComponent();
        }

        public AutoGenPluginUserControl(IAfterglowPlugin plugin)
        {
            this._plugin = plugin;
            InitializeComponent();
        }

        private void AutoGenPluginUserControlLoad(object sender, EventArgs e)
        {
            this.Controls.Clear();

            Type t = _plugin.GetType();

            List<BaseControl> controls = new List<BaseControl>();

            foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Attribute.IsDefined(prop, typeof(ConfigAttribute)) &&
                    !(Attribute.GetCustomAttribute(prop, typeof(ConfigAttribute)) as ConfigAttribute).IsHidden)
                {
                    if (Attribute.IsDefined(prop, typeof(ConfigLookupAttribute)))
                    {
                        controls.Add(new LookupControl(prop, _plugin));
                    }
                    else if (Attribute.IsDefined(prop, typeof(ConfigNumberAttribute)))
                    {
                        controls.Add(new NumberControl(prop, _plugin));
                    }
                    else if (Attribute.IsDefined(prop, typeof(ConfigStringAttribute)))
                    {
                        controls.Add(new StringControl(prop, _plugin));
                    }
                    else if (Attribute.IsDefined(prop, typeof(ConfigReadOnlyAttribute)))
                    {
                        controls.Add(new ReadOnlyControl(prop, _plugin));
                    }
                }
            }
            ConfigCustomAttribute configCustom = (ConfigCustomAttribute)Attribute.GetCustomAttribute(t, typeof(ConfigCustomAttribute));
            if (configCustom != null)
            {
                BaseControl control = CreateCustomControl(configCustom);

                control.Parent = this;
                control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

                controls.Add(control);
            }
            
            //Add Sort to not set values
            foreach (var item in controls.Where(s => s.SortIndex == 0))
            {
                item.SortIndex = (from c in controls
                                  orderby c.SortIndex descending
                                  select c.SortIndex).FirstOrDefault() + 100;
            }

            //Sort controls
            controls = (from c in controls
                        orderby c.SortIndex
                        select c).ToList();

            //Add Controls
            controls.ForEach(c => AddControl(c));

            //TODO add scroll bar control may not need it
        }

        private BaseControl CreateCustomControl(ConfigCustomAttribute configCustom)
        {
            //Get type from local project if possible, if not use other loader
            Type objectType = System.Type.GetType(configCustom.CustomControlName) ?? _plugin.Runtime.Loader.GetObjectType(configCustom.CustomControlName);
                    
            BaseControl createdUserControl = Activator.CreateInstance(objectType, _plugin) as BaseControl;

            return createdUserControl;
        }

        private void AddControl(BaseControl control)
        {
            this.Controls.Add(control);

            control.Width = this.Width;

            //Move the controls down if needed
            int bottom = 0 ;
            for (int i = 0; i < this.Controls.Count - 1; i++)
            {
                if (this.Controls[i].Bounds.Bottom > bottom)
                {
                    bottom = this.Controls[i].Bounds.Bottom;
                }
            }
            control.Top = bottom;
        }

        private void AutoGenPluginUserControlResize(object sender, EventArgs e)
        {
            
            
        }

        //private void AddNumberControl(PropertyInfo prop)
        //{
        //    ConfigNumberAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigNumberAttribute)) as ConfigNumberAttribute;

        //    Type propertyType = prop.PropertyType;
        //    string displayName = configAttribute.DisplayName;
        //    string description = configAttribute.Description;
            
        //    //Create Controls
        //    Panel panel = new Panel();
        //    Label label = new Label();
        //    NumericUpDown numericUpDown = new NumericUpDown();

        //    //Create Names
        //    panel.Name = Guid.NewGuid().ToString();
        //    label.Name = Guid.NewGuid().ToString();
        //    numericUpDown.Name = Guid.NewGuid().ToString();
            
        //    //Move the controls down if needed
        //    Control bottomControl = (this.Controls.Count != 0 ? this.Controls[this.Controls.Count - 1] : null);
        //    if (bottomControl != null)
        //    {
        //        panel.Top = bottomControl.Bounds.Bottom;
        //    }

        //    //Set sizes
        //    panel.Width = this.Width;
        //    label.Width = Convert.ToInt32(this.Width * .4);
        //    numericUpDown.Left = label.Width;
        //    numericUpDown.Width = Convert.ToInt32(this.Width * .6);

        //    //Add Controls
        //    this.Controls.Add(panel);
        //    panel.Controls.Add(label);
        //    panel.Controls.Add(numericUpDown);

        //    //Populate Controls
        //    label.Text = displayName;

        //    //Configure decimal Places
        //    if (propertyType == typeof(int))
        //    {
        //        numericUpDown.DecimalPlaces = 0;
        //    }
        //    else if (configAttribute.Min != 0 || configAttribute.Max != 0)
        //    {
        //        List<int> decimalPlaces = new List<int>{configAttribute.Min.ToString().SkipWhile(c => c != '.').Skip(1).Count(), configAttribute.Max.ToString().SkipWhile(c => c != '.').Skip(1).Count()};
        //        numericUpDown.DecimalPlaces = decimalPlaces.OrderBy(d => d).FirstOrDefault();
        //    }
        //    numericUpDown.Minimum = Convert.ToDecimal(configAttribute.Min);
        //    numericUpDown.Maximum = Convert.ToDecimal(configAttribute.Max);
        //    //numericUpDown.Value = Convert.ToDecimal(prop.GetValue(_plugin, null));
        //    numericUpDown.DataBindings.Add("Value", _plugin, prop.Name);
        //}

        //private void AddStringControl(PropertyInfo prop)
        //{
        //    ConfigStringAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigStringAttribute)) as ConfigStringAttribute;

        //    Type propertyType = prop.PropertyType;
        //    string displayName = configAttribute.DisplayName;
        //    string description = configAttribute.Description;

        //    //Create Controls
        //    Panel panel = new Panel();
        //    Label label = new Label();
        //    Label descriptionLabel = new Label();
        //    TextBox textBox = new TextBox();

        //    //Create Names
        //    panel.Name = Guid.NewGuid().ToString();
        //    descriptionLabel.Name = Guid.NewGuid().ToString();
        //    label.Name = Guid.NewGuid().ToString();
        //    textBox.Name = Guid.NewGuid().ToString();

        //    //Move the controls down if needed
        //    Control bottomControl = (this.Controls.Count != 0 ? this.Controls[this.Controls.Count - 1] : null);
        //    if (bottomControl != null)
        //    {
        //        panel.Top = bottomControl.Bounds.Bottom;
        //    }

        //    //Set sizes
        //    panel.Width = this.Width;
        //    label.Width = Convert.ToInt32(this.Width * .4);
        //    textBox.Left = label.Width;
        //    textBox.Width = Convert.ToInt32(this.Width * .6);
        //    descriptionLabel.Width = this.Width;
        //    descriptionLabel.Top = label.Bounds.Bottom;
        //    panel.Height = label.Height + descriptionLabel.Height;

        //    //Add Controls
        //    this.Controls.Add(panel);
        //    panel.Controls.Add(label);
        //    panel.Controls.Add(textBox);
        //    panel.Controls.Add(descriptionLabel);

        //    //Populate Controls
        //    label.Text = displayName;
        //    descriptionLabel.Text = description;
        //    textBox.Enabled = prop.CanWrite;

        //    textBox.Text = prop.GetValue(_plugin, null).ToString();

            
        //}

        //private void AddLookupControl(PropertyInfo prop)
        //{
        //    ConfigLookupAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigLookupAttribute)) as ConfigLookupAttribute;
        //    Type pluginType = _plugin.GetType();
        //    Type propertyType = prop.PropertyType;

        //    string displayName = configAttribute.DisplayName;
        //    IEnumerable<object> availableValues = null;
        //    if (propertyType.IsEnum)
        //    {
        //        availableValues = propertyType.GetEnumNames();
        //    }
        //    else if (configAttribute.RetrieveValuesFrom != null)
        //    {
        //        var member = pluginType.GetMember(configAttribute.RetrieveValuesFrom);
        //        if (member.Length > 0)
        //        {
        //            if (member[0].MemberType == MemberTypes.Property)
        //            {
        //                PropertyInfo pi = pluginType.GetProperty(configAttribute.RetrieveValuesFrom);

        //                var propertyValue = pi.GetValue(_plugin, null);
        //                if (typeof(IEnumerable<>).MakeGenericType(propertyType).IsAssignableFrom(propertyValue.GetType()))
        //                {
        //                    availableValues = propertyValue as IEnumerable<object>;
        //                }
        //                else
        //                {
        //                    throw new ArgumentException("incorrect type", "RetrieveValuesFrom");
        //                }

        //            }
        //            else if (member[0].MemberType == MemberTypes.Method)
        //            {
        //                MethodInfo mi = pluginType.GetMethod(configAttribute.RetrieveValuesFrom);

        //                var propertyValue = mi.Invoke(_plugin, null);
        //                if (typeof(IEnumerable<>).MakeGenericType(propertyType).IsAssignableFrom(propertyValue.GetType()))
        //                {
        //                    availableValues = propertyValue as IEnumerable<object>;
        //                }
        //                else
        //                {
        //                    throw new ArgumentException("incorrect type", "RetrieveValuesFrom");
        //                }
        //            }
        //        }
        //    }

        //    string value = prop.GetValue(_plugin, null) as string;

        //    //Create Controls
        //    Panel panel = new Panel();
        //    Label label = new Label();
        //    Label descriptionLabel = new Label();
        //    ComboBox comboBox = new ComboBox();

        //    //Create Names
        //    panel.Name = Guid.NewGuid().ToString();
        //    label.Name = Guid.NewGuid().ToString();
        //    descriptionLabel.Name = Guid.NewGuid().ToString();
        //    comboBox.Name = Guid.NewGuid().ToString();

        //    //Move the controls down if needed
        //    Control bottomControl = (this.Controls.Count != 0 ? this.Controls[this.Controls.Count - 1] : null);
        //    if (bottomControl != null)
        //    {
        //        panel.Top = bottomControl.Bounds.Bottom;
        //    }

        //    //Set Widths
        //    panel.Width = this.Width;
        //    label.Width = Convert.ToInt32(this.Width * .4);
        //    comboBox.Left = label.Width;
        //    comboBox.Width = Convert.ToInt32(this.Width * .6);
        //    descriptionLabel.Width = this.Width;
        //    descriptionLabel.Top = label.Bounds.Bottom;
        //    panel.Height = label.Height + descriptionLabel.Height;

        //    //Add Controls
        //    this.Controls.Add(panel);
        //    panel.Controls.Add(label);
        //    panel.Controls.Add(comboBox);
        //    panel.Controls.Add(descriptionLabel);

        //    //Populate Controls
        //    label.Text = displayName;
        //    descriptionLabel.Text = configAttribute.Description;

        //    //Populate Combo and set current item
        //    if (availableValues != null)
        //    {
        //        comboBox.Items.AddRange(availableValues.ToArray());
        //        if (value == null)
        //        {
        //            comboBox.SelectedIndex = 0;
        //        }
        //        else
        //        {
        //            comboBox.SelectedIndex = comboBox.Items.IndexOf(value);
        //        }
        //    }

        //}
    }
}
