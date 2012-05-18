using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConfigCustomAttribute : ConfigAttribute
    {
        /// <summary>
        /// The name of the custom control for the class
        /// </summary>
        public string CustomControlName { get; set; }
    }
}
