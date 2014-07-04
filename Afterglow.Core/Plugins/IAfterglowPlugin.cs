using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Afterglow.Core.Plugins
{
    /// <summary>
    /// All Plugin types decend from this interface
    /// </summary>
    public interface IAfterglowPlugin : INotifyPropertyChanged
    {
        /// <summary>
        /// The identifier of the plugin
        /// This is unique accross a plugin type
        /// </summary>
        int Id { get; set; }
        
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
        /// Start this Plugin
        /// </summary>
        void Start();

        /// <summary>
        /// Stop this Plugin
        /// </summary>
        void Stop();
    }
}
