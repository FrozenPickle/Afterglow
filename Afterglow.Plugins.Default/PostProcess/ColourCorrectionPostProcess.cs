using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core;
using Afterglow.Core.Configuration;
using System.Drawing;
using System.ComponentModel.DataAnnotations;

namespace Afterglow.Plugins.PostProcess
{
    public class ColourCorrectionPostProcess : BasePlugin, IPostProcessPlugin
    {
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        public override string Name
        {
            get { return "Colour Correction Plugin"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        public override string Description
        {
            get { return "Adjust the colours to your room"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
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

        [Required]
        [Display(Name = "Brightness", Description = "Changes how bright the lights are")]
        [Range(0, 100)]
        public int Brightness
        {
            get { return Get(() => Brightness, () => 100); }
            set { Set(() => Brightness, value); }
        }

        [Required]
        [Display(Name = "Red Saturation")]
        [Range(0, 100)]
        public int RedSaturation
        {
            get { return Get(() => RedSaturation, () => 100); }
            set { Set(() => RedSaturation, value); }
        }

        [Required]
        [Display(Name = "Green Saturation", Description = "Changes how bright the lights are")]
        [Range(0, 100)]
        public int GreenSaturation
        {
            get { return Get(() => GreenSaturation, () => 100); }
            set { Set(() => GreenSaturation, value); }
        }

        [Required]
        [Display(Name = "Blue Saturation")]
        [Range(0, 100)]
        public int BlueSaturation
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
            double red = led.LightColour.R;
            double green = led.LightColour.G;
            double blue = led.LightColour.B;

            bool coloursChanged = false;

 	        //Change brightness first
            if (this.Brightness != 100)
            {
                double percent = Brightness / 100.00;
                red = red * percent;
                green = green * percent;
                blue = blue * percent;

                coloursChanged = true;
            }

            if (this.RedSaturation != 100)
            {
                double percent = RedSaturation / 100.00;
                red = red * percent;

                coloursChanged = true;
            }

            if (this.GreenSaturation != 100)
            {
                double percent = GreenSaturation / 100.00;
                green = green * percent;

                coloursChanged = true;
            }

            if (this.BlueSaturation != 100)
            {
                double percent = BlueSaturation / 100.00;
                blue = blue * percent;

                coloursChanged = true;
            }


            if (coloursChanged)
            {
                int resultRed = led.LightColour.R;
                int resultGreen = led.LightColour.G;
                int resultBlue = led.LightColour.B;

                resultRed = Convert.ToInt32(red);
                resultGreen = Convert.ToInt32(green);
                resultBlue = Convert.ToInt32(blue);

                led.LightColour = Color.FromArgb(resultRed, resultGreen, resultBlue);
            }

        }
    }
}
