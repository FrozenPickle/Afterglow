using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Configuration;
using System.Reflection;
using Afterglow.Core.Storage;

namespace Afterglow.Core.Plugins
{
    public abstract class BasePlugin: BaseRecord, IAfterglowPlugin
    {
        public BasePlugin()
        {

        }

        public BasePlugin(ITable table, Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        public override string ToString()
        {
            if (this.Name == this.DisplayName)
            {
                return this.Name;
            }
            else
            {
                return string.Format("{0} ({1})", this.DisplayName, this.Name);
            }
        }

        [ConfigString(DisplayName = "Name", SortIndex = 700)]
        public string DisplayName
        {
            get { return Get(() => DisplayName, () => this.Name); }
            set { Set(() => DisplayName, value); }
        }

        [ConfigReadOnly(DisplayName = "Name", SortIndex= 100)]
        public abstract string Name { get; }

        [ConfigReadOnly(DisplayName = "Author", SortIndex = 200, Description = "These two guys started the Afterglow project")]
        public abstract string Author { get; }

        [ConfigReadOnly(DisplayName = "Description", SortIndex = 300)]
        public abstract string Description { get; }

        [ConfigReadOnly(DisplayName = "Website", SortIndex = 400, IsHyperlink = true)]
        public abstract string Website { get; }

        [ConfigReadOnly(DisplayName = "Version", SortIndex = 500)]
        public abstract Version Version { get; }

        /// <summary>
        /// The logger for this plugin.
        /// </summary>
        new Afterglow.Core.Log.ILogger Logger { get; set; }

        public abstract void Start();

        public abstract void Stop();
    }
}
