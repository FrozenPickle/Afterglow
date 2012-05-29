using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Afterglow.Core.Plugins
{
    public interface ILightSetupPlugin : IAfterglowPlugin
    {
        ObservableCollection<Light> Lights { get; set; }

        int? NumberOfLightsHigh { get; set; }

        int? NumberOfLightsWide { get; set; }

        IEnumerable<Light> GetLightsForBounds(int CaptureWidth, int CaptureHeight, int LeftOffset, int TopOffset);
    }
}
