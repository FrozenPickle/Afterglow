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
    public partial class NumberControl : DataControl
    {
        private NumericUpDown _valueNumericUpDown;
        private PropertyInfo _propertyInfo;
        private IAfterglowPlugin _plugin;

        public NumberControl(PropertyInfo prop, IAfterglowPlugin plugin)
            : base(prop)
        {
            ConfigNumberAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigNumberAttribute)) as ConfigNumberAttribute;

            if (configAttribute == null)
            {
                throw new Exception("prop is not a ConfigNumberAttribute");
            }
            this._propertyInfo = prop;
            this._plugin = plugin;

            //Create value Text Box
            _valueNumericUpDown = new NumericUpDown();
            _valueNumericUpDown.Name = Guid.NewGuid().ToString();
            _valueNumericUpDown.Minimum = Convert.ToDecimal(configAttribute.Min);
            _valueNumericUpDown.Maximum = Convert.ToDecimal(configAttribute.Max);


            //Configure decimal Places
            if (prop.PropertyType == typeof(int?))
            {
                _valueNumericUpDown.DecimalPlaces = 0;
            }
            else if (configAttribute.Min != 0 || configAttribute.Max != 0)
            {
                List<int> decimalPlaces = new List<int>{configAttribute.Min.ToString().SkipWhile(c => c != '.').Skip(1).Count(), configAttribute.Max.ToString().SkipWhile(c => c != '.').Skip(1).Count()};
                _valueNumericUpDown.DecimalPlaces = decimalPlaces.OrderBy(d => d).FirstOrDefault();
            }

            decimal value = 0;
            value = Convert.ToDecimal(prop.GetValue(plugin, null));
            _valueNumericUpDown.Value = value;
            _valueNumericUpDown.ValueChanged += new EventHandler(_valueNumericUpDown_ValueChanged);
            
            this.Controls.Add(_valueNumericUpDown);

            InitializeComponent();
        }

        void _valueNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            decimal value = 0;
            value = _valueNumericUpDown.Value;

            if (_propertyInfo.PropertyType == typeof(int?))
            {
                _propertyInfo.SetValue(_plugin, Convert.ToInt32(value), null);
            }
            else if (_propertyInfo.PropertyType == typeof(double?))
            {
                _propertyInfo.SetValue(_plugin, Convert.ToDouble(value), null);
            }
        }

        private void ControlResize(object sender, EventArgs e)
        {
            if (_valueNumericUpDown != null)
            {
                _valueNumericUpDown.Top = 0;
                _valueNumericUpDown.Left = Convert.ToInt32(this.Width * LABEL_WIDTH);
                _valueNumericUpDown.Width = Convert.ToInt32(this.Width * VALUE_WIDTH);
            }
        }
    }
}
