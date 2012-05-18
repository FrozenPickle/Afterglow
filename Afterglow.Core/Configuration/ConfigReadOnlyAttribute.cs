using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Configuration
{
    public class ConfigReadOnlyAttribute: ConfigAttribute
    {
        public bool IsHyperlink { get; set; }
    }
}
