using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Plugins
{
    /// <summary>
    /// A Plugin that adjusts the output colours 
    /// </summary>
    public interface IPostProcessPlugin : IAfterglowPlugin
    {
        /// <summary>
        /// Process the Light object and adjust it
        /// </summary>
        /// <param name="light">Light Object</param>
        void Process(Light light);
    }
}
