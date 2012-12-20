using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Plugins
{
    /// <summary>
    /// A Plugin that converts Light objects created by the Light Setup Plugin into Screen regions
    /// </summary>
    public interface ICapturePlugin : IAfterglowPlugin
    {
        /// <summary>
        /// Perform capture and return the Light -> PixelReader mapping
        /// </summary>
        /// <param name="leds"></param>
        /// <returns></returns>
        IDictionary<Light, PixelReader> Capture(ILightSetupPlugin lightSetup);

        /// <summary>
        /// The Afterglow runtime is finished with the previous capture, any clean up can now be performed.
        /// </summary>
        void ReleaseCapture();
    }
}
