using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Afterglow.Core.Plugins
{
    /// <summary>
    /// A plugin that describe the positioning of the lights and usualy directly correlates to the screen capture positions
    /// </summary>
    public interface ILightSetupPlugin : IAfterglowPlugin
    {
        List<Light> Lights { get; set; }

        int NumberOfLightsHigh { get; set; }

        int NumberOfLightsWide { get; set; }

        IEnumerable<Light> GetLightsForBounds(int CaptureWidth, int CaptureHeight, int LeftOffset, int TopOffset);
    }
}
