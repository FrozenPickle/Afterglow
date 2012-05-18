using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core;
using Afterglow.Core.Configuration;

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
 	        //Change brightness first
            if (this.Brightness != 100)
            {
                //led.LEDColour.B = led.LEDColour.B % Brightness;
            }
            //
        }
    }
}
