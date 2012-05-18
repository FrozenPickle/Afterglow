using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Afterglow.Core.Plugins
{
    public interface IColourExtractionPlugin: IAfterglowPlugin
    {
        Color Extract(Light led, PixelReader pixelReader);
    }
}
