using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Configuration
{
    public class ConfigTableAttribute : ConfigAttribute
    {
        /// <summary>
        /// A property or parameterless function that returns an <see cref="IEnumerable{Object}" />. Note: the return object must be assignable to the property.
        /// An enum does not require this property
        /// </summary>
        public string RetrieveValuesFrom { get; set; }
    }
}
