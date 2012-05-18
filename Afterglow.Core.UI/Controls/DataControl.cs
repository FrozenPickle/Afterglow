using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core.Configuration;
using System.Reflection;

namespace Afterglow.Core.UI.Controls
{
    public partial class DataControl : BaseControl
    {
        public DataControl()
        {
            InitializeComponent();
        }

        public const double LABEL_WIDTH = .4; //40%
        public const double VALUE_WIDTH = .6; //60%

        private Label _displayNameLabel;
        private Label _descriptionLabel;

        public DataControl(PropertyInfo prop, float? fontSize = null)
        {
            ConfigAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigAttribute)) as ConfigAttribute;

            if (configAttribute == null)
            {
                throw new Exception("prop is not a ConfigAttribute");
            }

            //Sort Index
            this.SortIndex = configAttribute.SortIndex;

            //Create display name Label
            _displayNameLabel = new Label();
            _displayNameLabel.Name = Guid.NewGuid().ToString();
            _displayNameLabel.Text = configAttribute.DisplayName;
            if (fontSize != null)
            {
                Font font = new Font(_displayNameLabel.Font.FontFamily, fontSize.Value);
                _displayNameLabel.Font = font;
                _displayNameLabel.Height = _displayNameLabel.Font.Height + 4;
            }
            this.Controls.Add(_displayNameLabel);

            //Create description if there is one
            if (!string.IsNullOrEmpty(configAttribute.Description))
            {
                _descriptionLabel = new Label();
                _descriptionLabel.Name = Guid.NewGuid().ToString();
                _descriptionLabel.Text = configAttribute.Description;
                if (fontSize != null)
                {
                    Font font = new Font(_descriptionLabel.Font.FontFamily, fontSize.Value);
                    _descriptionLabel.Font = font;
                    _descriptionLabel.Height = _descriptionLabel.Font.Height + 4;
                }
                this.Controls.Add(_descriptionLabel);
                this.Height = _displayNameLabel.Height +_descriptionLabel.Height;
            }
            else
            {
                this.Height = _displayNameLabel.Height;
            }


            InitializeComponent();
        }

        private void DataControlResize(object sender, EventArgs e)
        {
            //display name is required
            if (_displayNameLabel != null)
            {
                _displayNameLabel.Top = 0;
                _displayNameLabel.Left = 0;
                _displayNameLabel.Width = Convert.ToInt32(this.Width * LABEL_WIDTH);

                //Description is optional
                if (_descriptionLabel != null)
                {
                    _descriptionLabel.Top = _displayNameLabel.Bounds.Bottom;
                    _descriptionLabel.Left = 0;
                    _descriptionLabel.Width = this.Width;
                }
            }
            
        }
    }
}
