using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Afterglow.Core.UI.Controls
{
    public class BaseControl: UserControl, IAfterglowUIControl
    {
        public event PluginsChangedEventHandler PluginsChanged;

        protected virtual void OnPluginsChanged(PluginsChangedEventArgs e)
        {
            if (PluginsChanged != null)
                PluginsChanged(this, e);
        }

        public int SortIndex { get; set; }
    }
}
