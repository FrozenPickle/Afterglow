using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Plugins
{
    /// <summary>
    /// This plugin sends the lights to the output device
    /// </summary>
    public interface IOutputPlugin: IAfterglowPlugin
    {
        /// <summary>
        /// Send the lights to the output device
        /// </summary>
        /// <param name="leds">A collection of lights</param>
        void Output(List<Light> lights);

        /// <summary>
        /// Trys to start the output plugin
        /// </summary>
        /// <param name="errorMessage">The error that has occured</param>
        /// <returns>Success or failure of starting this plugin</returns>
        bool TryStart(out string errorMessage);
    }
}
