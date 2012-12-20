using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core;
using System.Collections.ObjectModel;
using Afterglow.Core.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Afterglow.Plugins.LightSetup.BasicLightSetupPlugin
{
    //This Class requires a custom UI
    [ConfigCustom(CustomControlName = "Afterglow.Plugins.LightSetup.BasicLightSetupPlugin.BasicLightSetupUserControl")]
    public class BasicLightSetup : BasePlugin, ILightSetupPlugin
    {
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        public override string Name
        {
            get { return "Basic Light Region Setup"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        public override string Description
        {
            get { return "Setup the capture image location that is converted into light"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        public override string Author
        {
            get { return "Jono C"; }
        }
        /// <summary>
        /// A website for further information
        /// </summary>
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion

        [Required]
        [Display(Name = "Number Of Lights High")]
        [Range(1, 999999)]
        public int NumberOfLightsHigh
        {
            get { return Get(() => NumberOfLightsHigh, () => 1); }
            set { Set(() => NumberOfLightsHigh, value); }
        }

        [Required]
        [Display(Name = "Number Of Lights Wide")]
        [Range(1, 999999)]
        public int NumberOfLightsWide
        {
            get { return Get(() => NumberOfLightsWide, () => 1); }
            set { Set(() => NumberOfLightsWide, value); }
        }

        [Display(Name = "Lights", AutoGenerateField=false)]
        public List<Core.Light> Lights
        {
            get { return Get(() => Lights); }
            set { Set(() => Lights, value); }
        }

        /// <summary>
        /// Not used for this plugin
        /// </summary>
        public override void Start()
        {

        }
        /// <summary>
        /// Not used for this plugin
        /// </summary>
        public override void Stop()
        {

        }

        private int _width;

        private int _height;

        public IEnumerable<Light> GetLightsForBounds(int CaptureWidth, int CaptureHeight, int LeftOffset, int TopOffset)
        {
            if (_height != CaptureHeight || _width != CaptureWidth)
            {
                _width = CaptureWidth;
                _height = CaptureHeight;

                int segmentWidth = CaptureWidth / NumberOfLightsWide;
                int segmentHight = CaptureHeight / NumberOfLightsHigh;
                
                foreach (Light light in this.Lights)
                {
                    light.CalculateRegion(segmentWidth, segmentHight, LeftOffset, TopOffset);
                }
            }

            return this.Lights;
        }
    }
}
