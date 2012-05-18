using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Configuration
{
    public class ConfigStringAttribute: ConfigAttribute
    {
        public int MaxLength { get; set; }
    }
}
