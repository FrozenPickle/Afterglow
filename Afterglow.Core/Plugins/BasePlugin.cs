using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Afterglow.Core.Plugins
{
    public abstract class BasePlugin: BaseModel, IAfterglowPlugin
    {

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

        /// <summary>
        /// A Display Name for this Plugin 
        /// </summary>
        [Display(Name = "Display Name", Order = -100)]
        public string DisplayName
        {
            get { return Get(() => DisplayName, () => this.Name); }
            set { Set(() => DisplayName, value); }
        }

        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Name", Order= -600)]
        public abstract string Name { get; }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Author", Order = -500)]
        public abstract string Author { get; }

        /// <summary>
        /// A description of this plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Description", Order = -400)]
        public abstract string Description { get; }
        /// <summary>
        /// A website for further information
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Website", Order = -300)]
        [DataType(DataType.Url)]
        public abstract string Website { get; }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Version", Order = -200)]
        public abstract Version Version { get; }
        
        /// <summary>
        /// Start this Plugin
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stop this Plugin
        /// </summary>
        public abstract void Stop();
    }
}
