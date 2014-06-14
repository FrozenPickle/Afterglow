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
            get { return Get(() => NumberOfLightsHigh, () => 2); }
            set { Set(() => NumberOfLightsHigh, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Number Of Lights Wide")]
        [Range(1, 999999)]
        public int NumberOfLightsWide
        {
            get { return Get(() => NumberOfLightsWide, () => 2); }
            set { Set(() => NumberOfLightsWide, value); }
        }

        [DataMember]
        [Display(Name = "Lights", AutoGenerateField = false)]
        public List<Core.Light> Lights
        {
            get { return Get(() => Lights, GetDefaultLights()); }
            set { Set(() => Lights, value); }
        }

        public List<Core.Light> GetDefaultLights()
        {
            List<Core.Light> lights = new List<Light>();

            Light light1 = new Light();
            light1.Id = 0;
            light1.Index = 1;
            light1.Left = 0;
            light1.Top = 0;
            lights.Add(light1);

            Light light2 = new Light();
            light2.Id = 1;
            light2.Index = 2;
            light2.Left = 1;
            light2.Top = 0;
            lights.Add(light2);

            Light light3 = new Light();
            light3.Id = 2;
            light3.Index = 3;
            light3.Left = 1;
            light3.Top = 1;
            lights.Add(light3);

            Light light4 = new Light();
            light4.Id = 3;
            light4.Index = 4;
            light4.Left = 0;
            light4.Top = 1;
            lights.Add(light4);

            return lights;
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
