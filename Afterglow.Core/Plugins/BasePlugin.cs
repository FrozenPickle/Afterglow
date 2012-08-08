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

        [Display(Name = "Configuration Name", Order = -100)]
        public string DisplayName
        {
            get { return Get(() => DisplayName, () => this.Name); }
            set { Set(() => DisplayName, value); }
        }

        [ReadOnly(true)]
        [Display(Name = "Name", Order= -600)]
        public abstract string Name { get; }

        [ReadOnly(true)]
        [Display(Name = "Author", Order = -500)]
        public abstract string Author { get; }

        [ReadOnly(true)]
        [Display(Name = "Description", Order = -400)]
        public abstract string Description { get; }

        [ReadOnly(true)]
        [Display(Name = "Website", Order = -300)]
        [DataType(DataType.Url)]
        public abstract string Website { get; }

        [ReadOnly(true)]
        [Display(Name = "Version", Order = -200)]
        public abstract Version Version { get; }
        
        public abstract void Start();

        public abstract void Stop();
    }
}
