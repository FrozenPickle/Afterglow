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
    public partial class StringControl : DataControl
    {
        private TextBox _valueTextBox;
        public StringControl(PropertyInfo prop, IAfterglowPlugin plugin) : base(prop)
        {
            ConfigStringAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigStringAttribute)) as ConfigStringAttribute;

            if (configAttribute == null)
            {
                throw new Exception("prop is not a ConfigStringAttribute");
            }

            //Create value Text Box
            _valueTextBox = new TextBox();
            _valueTextBox.Name = Guid.NewGuid().ToString();
            _valueTextBox.MaxLength = configAttribute.MaxLength;
            _valueTextBox.DataBindings.Add("Text", plugin, prop.Name,false, DataSourceUpdateMode.OnPropertyChanged, null);
            this.Controls.Add(_valueTextBox);
            
            InitializeComponent();
        }

        private void ControlResize(object sender, EventArgs e)
        {
            if (_valueTextBox != null)
            {
                _valueTextBox.Top = 0;
                _valueTextBox.Left = Convert.ToInt32(this.Width * LABEL_WIDTH);
                _valueTextBox.Width = Convert.ToInt32(this.Width * VALUE_WIDTH);
            }
        }
    }
}
