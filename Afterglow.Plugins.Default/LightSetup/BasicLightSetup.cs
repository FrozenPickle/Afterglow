using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core;
using System.Collections.ObjectModel;
using Afterglow.Core.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ComponentModel.Composition;

namespace Afterglow.Plugins.LightSetup.BasicLightSetupPlugin
{
    [DataContract]
    [Export(typeof(ILightSetupPlugin))]
    public class BasicLightSetup : BasePlugin, ILightSetupPlugin
    {
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [DataMember]
        public override string Name
        {
            get { return "Basic Light Region Setup"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        [DataMember]
        public override string Description
        {
            get { return "Setup the capture image location that is converted into light"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [DataMember]
        public override string Author
        {
            get { return "Jono C"; }
        }
        /// <summary>
        /// A website for further information
        /// </summary>
        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 1); }
        }
        #endregion

        [DataMember]
        [Required]
        [Display(Name = "Number Of Lights High")]
        [Range(1, 999999)]
        public int NumberOfLightsHigh
        {
            get { return Get(() => NumberOfLightsHigh, () => 1); }
            set { Set(() => NumberOfLightsHigh, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Number Of Lights Wide")]
        [Range(1, 999999)]
        public int NumberOfLightsWide
        {
            get { return Get(() => NumberOfLightsWide, () => 1); }
            set { Set(() => NumberOfLightsWide, value); }
        }

        [DataMember]
        [Display(Name = "Lights", AutoGenerateField = false)]
        public List<Core.Light> Lights
        {
            get { return Get(() => Lights, () => GetDefaultLights()); }
            set { Set(() => Lights, value); }
        }

        public List<Core.Light> GetDefaultLights()
        {
            return new List<Light>();
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
            try
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

                return this.Lights.OrderBy(l => l.Index);
            }
            catch (Exception ex)
            {
                AfterglowRuntime.Logger.Error(ex, "Basic Light Setup Plugin - GetLightsForBounds");
                return new List<Light>();
            }
        }
    }
}
