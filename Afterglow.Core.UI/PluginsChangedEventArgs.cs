using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;

namespace Afterglow.Core.UI
{
    public delegate void PluginsChangedEventHandler(object sender, PluginsChangedEventArgs e);

    public class PluginsChangedEventArgs : EventArgs
    {
        public IAfterglowPlugin[] Plugins { get; set; }

        public bool RefreshProfiles { get; set; }
    }
}
