using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Configuration;
using Afterglow.Core.Plugins;
using Afterglow.Core.Log;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;


namespace Afterglow.Core
{
    public class Profile : BaseModel
    {
        private AfterglowSetup _setup;
        [XmlIgnore]
        public AfterglowSetup Setup
        {
            get
            {
                return _setup;
            }
            set
            {
                this._setup = value;
            }
        }

        [Required]
        [Display(Name = "Name", Order = 100)]
        public string Name
        {
            get { return Get(() => Name, "Default Profile"); }
            set { Set(() => Name, value); }
        }

        [Required]
        [Display(Name = "Description", Order = 200)]
        public string Description
        {
            get { return Get(() => Description); }
            set { Set(() => Description, value); }
        }

        #region Selected Plugins
        [Required]
        public SerializableInterfaceList<ILightSetupPlugin> LightSetupPlugins
        {
            get { return Get(() => LightSetupPlugins, () => new SerializableInterfaceList<ILightSetupPlugin>()); }
            set { Set(() => LightSetupPlugins, value); }
        }
        [XmlIgnore]
        public ILightSetupPlugin LightSetupPlugin
        {
            get
            {
                return LightSetupPlugins.FirstOrDefault();
            }
        }

        [Required]
        public SerializableInterfaceList<ICapturePlugin> CapturePlugins
        {
            get { return Get(() => CapturePlugins, () => new SerializableInterfaceList<ICapturePlugin>()); }
            set { Set(() => CapturePlugins, value); }
        }
        [XmlIgnore]
        public ICapturePlugin CapturePlugin
        {
            get
            {
                return CapturePlugins.FirstOrDefault();
            }
        }

        [Required]
        public SerializableInterfaceList<IColourExtractionPlugin> ColourExtractionPlugins
        {
            get { return Get(() => ColourExtractionPlugins, () => new SerializableInterfaceList<IColourExtractionPlugin>()); }
            set { Set(() => ColourExtractionPlugins, value); }
        }
        [XmlIgnore]
        public IColourExtractionPlugin ColourExtractionPlugin
        {
            get
            {
                return ColourExtractionPlugins.FirstOrDefault();
            }
        }

        public SerializableInterfaceList<IPostProcessPlugin> PostProcessPlugins
        {
            get { return Get(() => PostProcessPlugins, () => new SerializableInterfaceList<IPostProcessPlugin>()); }
            set { Set(() => PostProcessPlugins, value); }
        }

        [Required]
        public SerializableInterfaceList<IOutputPlugin> OutputPlugins
        {
            get { return Get(() => OutputPlugins, new SerializableInterfaceList<IOutputPlugin>()); }
            set { Set(() => OutputPlugins, value); }
        }

        #endregion
        
        //Rebuild Selected Plugins with objects from AfterglowSetup.Configured*Plugins
        //So that object references are all correct
        //Will also use less RAM
        public void OnDeserialized()
        {
            SerializableInterfaceList<ILightSetupPlugin> lightSetupPlugins = new SerializableInterfaceList<ILightSetupPlugin>();
            foreach (ILightSetupPlugin lightSetupPlugin in LightSetupPlugins)
            {
                ILightSetupPlugin existingPlugin = this.Setup.ConfiguredLightSetupPlugins.Single(p => p.Id == lightSetupPlugin.Id);

                //Add to configured list if not found
                if (existingPlugin == null)
                {
                    existingPlugin = lightSetupPlugin;
                    existingPlugin.Id = this.Setup.GetPluginId<ILightSetupPlugin>();
                    this.Setup.ConfiguredLightSetupPlugins.Add(existingPlugin);
                }
                lightSetupPlugins.Add(existingPlugin);
            }
            this.LightSetupPlugins = lightSetupPlugins;

            SerializableInterfaceList<ICapturePlugin> capturePlugins = new SerializableInterfaceList<ICapturePlugin>();
            foreach (ICapturePlugin capturePlugin in CapturePlugins)
            {
                ICapturePlugin existingPlugin = this.Setup.ConfiguredCapturePlugins.Single(p => p.Id == capturePlugin.Id);
                //Add to configured list if not found
                if (existingPlugin == null)
                {
                    existingPlugin = capturePlugin;
                    existingPlugin.Id = this.Setup.GetPluginId<ICapturePlugin>();
                    this.Setup.ConfiguredCapturePlugins.Add(existingPlugin);
                }
                capturePlugins.Add(existingPlugin);
            }
            this.CapturePlugins = capturePlugins;

            SerializableInterfaceList<IColourExtractionPlugin> colourExtractionPlugins = new SerializableInterfaceList<IColourExtractionPlugin>();
            foreach (IColourExtractionPlugin colourExtractionPlugin in ColourExtractionPlugins)
            {
                IColourExtractionPlugin existingPlugin = this.Setup.ConfiguredColourExtractionPlugins.Single(p => p.Id == colourExtractionPlugin.Id);
                //Add to configured list if not found
                if (existingPlugin == null)
                {
                    existingPlugin = colourExtractionPlugin;
                    existingPlugin.Id = this.Setup.GetPluginId<IColourExtractionPlugin>();
                    this.Setup.ConfiguredColourExtractionPlugins.Add(existingPlugin);
                }
                colourExtractionPlugins.Add(existingPlugin);
            }
            this.ColourExtractionPlugins = colourExtractionPlugins;

            SerializableInterfaceList<IPostProcessPlugin> postProcessPlugins = new SerializableInterfaceList<IPostProcessPlugin>();
            foreach (IPostProcessPlugin postProcessPlugin in PostProcessPlugins)
            {
                IPostProcessPlugin existingPlugin = this.Setup.ConfiguredPostProcessPlugins.Single(p => p.Id == postProcessPlugin.Id);
                //Add to configured list if not found
                if (existingPlugin == null)
                {
                    existingPlugin = postProcessPlugin;
                    existingPlugin.Id = this.Setup.GetPluginId<IPostProcessPlugin>();
                    this.Setup.ConfiguredPostProcessPlugins.Add(existingPlugin);
                }
                postProcessPlugins.Add(existingPlugin);
            }
            this.PostProcessPlugins = postProcessPlugins;

            SerializableInterfaceList<IOutputPlugin> outputPlugins = new SerializableInterfaceList<IOutputPlugin>();
            foreach (IOutputPlugin outputPlugin in OutputPlugins)
            {
                IOutputPlugin existingPlugin = this.Setup.ConfiguredOutputPlugins.FirstOrDefault(p => p.Id == outputPlugin.Id);
                //Add to configured list if not found
                if (existingPlugin == null)
                {
                    existingPlugin = outputPlugin;
                    existingPlugin.Id = this.Setup.GetPluginId<IOutputPlugin>();
                    this.Setup.ConfiguredOutputPlugins.Add(existingPlugin);
                }
                outputPlugins.Add(existingPlugin);
            }
            this.OutputPlugins = outputPlugins;
        }

        public void Validate()
        {
            if (LightSetupPlugin == null)
                throw new InvalidOperationException("LightSetupPlugin is not set");
            if (CapturePlugin == null)
                throw new InvalidOperationException("CapturePlugin is not set");
            if (ColourExtractionPlugin == null)
                throw new InvalidOperationException("ColourExtractionPlugin is not set");
            if (PostProcessPlugins == null)
                throw new InvalidOperationException("PostProcessPlugins is not set");
            if (OutputPlugins == null)
                throw new InvalidOperationException("OutputPlugins is not set");
        }
    }
}
