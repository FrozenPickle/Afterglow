using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core.Configuration;
using Afterglow.Core;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Afterglow.Plugins.ColourExtraction
{
    [DataContract]
    public class AverageColourExtraction : BasePlugin, IColourExtractionPlugin
    {
        #region Read Only Properties
        [DataMember]
        public override string Name
        {
            get { return "Average Colour Extraction"; }
        }

        [DataMember]
        public override string Description
        {
            get { return "Average Colour Extraction applied to the raw input"; }
        }

        [DataMember]
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion

        [DataMember]
        [Required]
        [Display(Name = "Number of Pixels to skip", Description = "Increasing this value speeds up the capture but decreases the accuracy of the colour.")]
        [Range(0, 999)]
        public int PixelSkip
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
                foreach (var pixel in pixelReader.GetEveryNthPixel(this.PixelSkip))
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
