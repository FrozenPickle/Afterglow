using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core;
using Afterglow.Core.Configuration;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Afterglow.Plugins.PostProcess
{
    [DataContract]
    public class ColourCorrectionPostProcess : BasePlugin, IPostProcessPlugin
    {
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [DataMember]
        public override string Name
        {
            get { return "Colour Correction Plugin"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        [DataMember]
        public override string Description
        {
            get { return "Adjust the colours to your room"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [DataMember]
        public override string Author
        {
            get { return "Jono C."; }
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
        [Display(Name = "Brightness", Description = "Changes how bright the lights are")]
        [Range(0, 100)]
        public int Brightness
        {
            get { return Get(() => Brightness, () => 100); }
            set { Set(() => Brightness, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Red Saturation")]
        [Range(0, 100)]
        public int RedSaturation
        {
            get { return Get(() => RedSaturation, () => 100); }
            set { Set(() => RedSaturation, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Green Saturation", Description = "Changes how bright the lights are")]
        [Range(0, 100)]
        public int GreenSaturation
        {
            get { return Get(() => GreenSaturation, () => 100); }
            set { Set(() => GreenSaturation, value); }
        }

        [DataMember]
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

        public void Process(List<Core.Light> lights, LightData data)
        {
            if (this.Brightness == 100 && this.RedSaturation == 100 && this.GreenSaturation == 100 && this.BlueSaturation == 100)
                return;

            for (var i = 0; i < data.Length; i++)
            {
                var lightColour = data[i];
                double red = lightColour.R;
                double green = lightColour.G;
                double blue = lightColour.B;

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
                    data[i] = Color.FromArgb((int)red, (int)green, (int)blue);
                }
            }
        }
    }
}
