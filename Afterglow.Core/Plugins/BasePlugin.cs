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
using System.Runtime.Serialization;

namespace Afterglow.Core.Plugins
{
    [DataContract]
    public abstract class BasePlugin: BaseModel, IAfterglowPlugin
    {
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Name", Order= -600)]
        [DataMember]
        public abstract string Name { get; }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Author", Order = -500)]
        [DataMember]
        public abstract string Author { get; }

        /// <summary>
        /// A description of this plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Description", Order = -400)]
        [DataMember]
        public abstract string Description { get; }
        /// <summary>
        /// A website for further information
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Website", Order = -300)]
        [DataType(DataType.Url)]
        [DataMember]
        public abstract string Website { get; }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        [ReadOnly(true)]
        [Display(Name = "Version", Order = -200)]
        [DataMember]
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
