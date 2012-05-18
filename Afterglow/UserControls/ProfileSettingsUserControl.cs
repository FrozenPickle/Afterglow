using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core.UI.Controls;

namespace Afterglow.UserControls
{
    public partial class ProfileSettingsUserControl : BaseControl
    {
        private Core.Profile _profile;

        public ProfileSettingsUserControl()
        {
            InitializeComponent();
        }

        public ProfileSettingsUserControl(Core.Profile profile)
        {
            this._profile = profile;
            InitializeComponent();
        }
    }
}
