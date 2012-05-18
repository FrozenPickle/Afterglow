using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core;
using System.Collections.ObjectModel;
using Afterglow.Core.Configuration;

namespace Afterglow.Plugins.LightSetup.BasicLightSetupPlugin
{
    [ConfigCustom(CustomControlName = "Afterglow.Plugins.LightSetup.BasicLightSetupPlugin.BasicLightSetupUserControl")]
    public class BasicLightSetup :BasePlugin, ILightSetupPlugin
    {
        public BasicLightSetup()
        {
        }

         public BasicLightSetup(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        #region Read Only Properties
        public override string Name
        {
            get { return "Basic Light Region Setup"; }
        }

        public override string Description
        {
            get { return "Setup the capture image location that is converted into light"; }
        }

        public override string Author
        {
            get { return "Jono C"; }
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

        [ConfigNumber(DisplayName = "Number Of Lights High", Min = 1, Max = 999999, IsHidden = true)]
        public int? NumberOfLightsHigh
        {
            get { return Get(() => NumberOfLightsHigh, () => 1); }
            set { Set(() => NumberOfLightsHigh, value); }
        }

        [ConfigNumber(DisplayName = "Number Of Lights Wide", Min = 1, Max = 999999, IsHidden = true)]
        public int? NumberOfLightsWide
        {
            get { return Get(() => NumberOfLightsWide, () => 1); }
            set { Set(() => NumberOfLightsWide, value); }
        }

        #region Lights
        [ConfigTable(DisplayName = "Lights", IsHidden = true)]
        public ObservableCollection<Core.Light> Lights
        {
            get { return Get(() => Lights); }
            set { Set(() => Lights, value); }
        }

        public Light AddLight()
        {
            Light light = new Light(Table.Database.AddTable(), this.Logger, this.Runtime);
            this.Lights.Add(light);
            SaveToStorage(() => this.Lights, this.Lights);
            return light;
        }

        public void RemoveLight(Light light)
        {
            this.Lights.Remove(light);
            SaveToStorage(() => this.Lights, this.Lights);
        }
        #endregion


        public override void Start()
        {

        }
        public override void Stop()
        {

        }

        private int _width;

        private int _height;

        public IEnumerable<Light> GetLightsForBounds(int Width, int Height)
        {
            if (_height != Height || _width != Width)
            {
                _width = Width;
                _height = Height;

                int segmentWidth = Width / NumberOfLightsWide.Value;
                int segmentHight = Height / NumberOfLightsHigh.Value;

                foreach (Light light in this.Lights)
                {
                    light.CalculateRegion(segmentWidth, segmentHight);
                }
            }

            return this.Lights;
        }
    }
}
