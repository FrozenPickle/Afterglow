using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Configuration
{
    /// <summary>
    /// Currently supported datatypes are int? and double?
    /// </summary>
    public class ConfigNumberAttribute: ConfigAttribute
    {
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
