using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core.Configuration;
using Afterglow.Core;
using Afterglow.Core.Storage;

namespace Afterglow.Plugins.ColourExtraction
{
    public class AverageColourExtraction : BasePlugin, IColourExtractionPlugin
    {
        public AverageColourExtraction()
        {
        }

        public AverageColourExtraction(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        #region Read Only Properties
        public override string Name
        {
            get { return "Average Colour Extraction"; }
        }

        public override string Description
        {
            get { return "Average Colour Extraction applied to the raw input"; }
        }

        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion

        [ConfigNumber(DisplayName = "Number of Pixels to skip", Min = 0, Max = 999,
            Description = "Increasing this value speeds up the capture but decreases the accuracy of the colour.")]
        public int? PixelSkip
        {
            get { return Get(() => PixelSkip, () => 3); }
            set { Set(() => PixelSkip, value); }
        }

        public Color Extract(Core.Light led, Core.PixelReader pixelReader)
        {
            //Region might not be set if the whole screen is black
            if (led.Region == null || led.Region == Rectangle.Empty)
            {
                return Color.Black;
            }
            else
            {
                // Average the pixels
                int r = 0, g = 0, b = 0, pixelCount = 0;
                foreach (var pixel in pixelReader.GetEveryNthPixel(this.PixelSkip.Value))
                {
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    pixelCount++;
                }

                int redAvg = r / pixelCount;

                int greenAvg = g / pixelCount;

                int blueAvg = b / pixelCount;

                return Color.FromArgb(redAvg, greenAvg, blueAvg);
            }

        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            
        }
    }
}
