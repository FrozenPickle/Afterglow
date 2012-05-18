using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using Afterglow.Core.Configuration;
using Afterglow.Core.Plugins;

namespace Afterglow.Core.UI.Controls
{
    public partial class ReadOnlyControl : DataControl
    {
        private const float FONT_SIZE = 7.0f;
        private Label _valueLabel;
        private LinkLabel _valueLinkLabel;
        public ReadOnlyControl(PropertyInfo prop, IAfterglowPlugin plugin)
            : base(prop, FONT_SIZE)
        {
            ConfigReadOnlyAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigReadOnlyAttribute)) as ConfigReadOnlyAttribute;

            if (configAttribute == null)
            {
                throw new Exception("prop is not a ConfigReadOnlyAttribute");
            }
            //Create value Label
            if (configAttribute.IsHyperlink)
            {
                string link = prop.GetValue(plugin, null).ToString();
                _valueLinkLabel = new LinkLabel();
                _valueLinkLabel.Name = Guid.NewGuid().ToString();
                _valueLinkLabel.Text = link;
                Font font = new Font(_valueLinkLabel.Font.FontFamily, FONT_SIZE);
                _valueLinkLabel.Font = font;
                _valueLinkLabel.Links.Add(0,link.Length,link);
                _valueLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkClicked);
                this.Controls.Add(_valueLinkLabel);
            }
            else
            {
                _valueLabel = new Label();
                _valueLabel.Name = Guid.NewGuid().ToString();
                _valueLabel.Text = prop.GetValue(plugin, null).ToString();
                Font font = new Font(_valueLabel.Font.FontFamily, FONT_SIZE);
                _valueLabel.Font = font;
                this.Controls.Add(_valueLabel);
            }

            InitializeComponent();
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void ControlResize(object sender, EventArgs e)
        {
            if (_valueLabel != null)
            {
                _valueLabel.Top = 0;
                _valueLabel.Left = Convert.ToInt32(this.Width * LABEL_WIDTH);
                _valueLabel.Width = Convert.ToInt32(this.Width * VALUE_WIDTH);
            }
            if (_valueLinkLabel != null)
            {
                _valueLinkLabel.Top = 0;
                _valueLinkLabel.Left = Convert.ToInt32(this.Width * LABEL_WIDTH);
                _valueLinkLabel.Width = Convert.ToInt32(this.Width * VALUE_WIDTH);
            }
        }
    }
}
