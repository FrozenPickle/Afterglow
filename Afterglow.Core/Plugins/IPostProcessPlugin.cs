using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Plugins
{
    public interface IPostProcessPlugin : IAfterglowPlugin
    {
        void Process(Light led);
    }
}
