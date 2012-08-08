using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Afterglow.Core.Configuration
{
    /// <summary>
    /// Currently supported datatypes are string[] and Enum
    /// </summary>
    public class ConfigLookupAttribute : ConfigAttribute
    {
        /// <summary>
        /// A property or parameterless function that returns an <see cref="IEnumerable{Object}" />. Note: the return object must be assignable to the property.
        /// An enum does not require this property
        /// </summary>
        public string RetrieveValuesFrom { get; set; }
    }
}
