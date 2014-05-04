using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Plugins
{
    /// <summary>
    /// A Plugin that adjusts the output colours 
    /// </summary>
    public interface IPreOutputPlugin : IAfterglowPlugin
    {
        /// <summary>
        /// Allows adjusting light values immediately prior to output
        /// </summary>
        /// <param name="lights">List of lights to output</param>
        /// <param name="data">light data that will be output</param>
        void PreOutput(List<Core.Light> lights, Core.LightData data);
    }
}
