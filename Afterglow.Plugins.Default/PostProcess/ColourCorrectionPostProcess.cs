using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core;
using Afterglow.Core.Configuration;
using System.Drawing;

namespace Afterglow.Plugins.PostProcess
{
    public class ColourCorrectionPostProcess : BasePlugin, IPostProcessPlugin
    {
        public ColourCorrectionPostProcess()
        {
        }

        public ColourCorrectionPostProcess(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        #region Read Only Properties
        public override string Name
        {
            get { return "Colour Correction Plugin"; }
        }

        public override string Description
        {
            get { return "Adjust the colours to your room"; }
        }

        public override string Author
        {
            get { return "Jono C."; }
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


        [ConfigNumber(DisplayName = "Brightness", Min = 0, Max = 100,
            Description = "Changes how bright the lights are")]
        public int? Brightness
        {
            get { return Get(() => Brightness, () => 100); }
            set { Set(() => Brightness, value); }
        }

        [ConfigNumber(DisplayName = "Red Saturation", Min = 0, Max = 100)]
        public int? RedSaturation
        {
            get { return Get(() => RedSaturation, () => 100); }
            set { Set(() => RedSaturation, value); }
        }

        [ConfigNumber(DisplayName = "Green Saturation", Min = 0, Max = 100)]
        public int? GreenSaturation
        {
            get { return Get(() => GreenSaturation, () => 100); }
            set { Set(() => GreenSaturation, value); }
        }

        [ConfigNumber(DisplayName = "Blue Saturation", Min = 0, Max = 100)]
        public int? BlueSaturation
        {
            get { return Get(() => BlueSaturation, () => 100); }
            set { Set(() => BlueSaturation, value); }
        }


        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    
        public void Process(Core.Light led)
        {
            double red = led.LEDColour.R;
            double green = led.LEDColour.G;
            double blue = led.LEDColour.B;

            bool coloursChanged = false;

 	        //Change brightness first
            if (this.Brightness != null && this.Brightness != 100)
            {
                double percent = Brightness.Value / 100.00;
                red = red * percent;
                green = green * percent;
                blue = blue * percent;

                coloursChanged = true;
            }

            if (this.RedSaturation != null && this.RedSaturation != 100)
            {
                double percent = RedSaturation.Value / 100.00;
                red = red * percent;

                coloursChanged = true;
            }

            if (this.GreenSaturation != null && this.GreenSaturation != 100)
            {
                double percent = GreenSaturation.Value / 100.00;
                green = green * percent;

                coloursChanged = true;
            }

            if (this.BlueSaturation != null && this.BlueSaturation != 100)
            {
                double percent = BlueSaturation.Value / 100.00;
                blue = blue * percent;

                coloursChanged = true;
            }


            if (coloursChanged)
            {
                int resultRed = led.LEDColour.R;
                int resultGreen = led.LEDColour.G;
                int resultBlue = led.LEDColour.B;

                resultRed = Convert.ToInt32(red);
                resultGreen = Convert.ToInt32(green);
                resultBlue = Convert.ToInt32(blue);

                led.LEDColour = Color.FromArgb(resultRed, resultGreen, resultBlue);
            }

        }
    }
}
