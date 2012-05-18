using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Afterglow.Core.Plugins
{
    public interface IAfterglowPlugin : INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the plugin given by a user.
        /// </summary>
        string DisplayName { get; set; }
        
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The author(s) of the plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// A description of the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// A website for information regarding the plugin.
        /// </summary>
        string Website { get; }

        /// <summary>
        /// The version of the plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// The logging level to log for this plugin.
        /// </summary>
        Afterglow.Core.Log.Level LogLevel { get; set; }

        /// <summary>
        /// The logger for this plugin.
        /// </summary>
        Afterglow.Core.Log.ILogger Logger { get; set; }

        /// <summary>
        /// The storage for this plugin.
        /// </summary>
        Afterglow.Core.Storage.ITable Table { get; set; }
        
        /// <summary>
        /// A reference to the runtime.
        /// </summary>
        Afterglow.Core.AfterglowRuntime Runtime { get; set; }

        void Start();

        void Stop();
    }
}
