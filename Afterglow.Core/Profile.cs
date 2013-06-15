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
using System.Runtime.Serialization;


namespace Afterglow.Core
{
    /// <summary>
    /// A collection of plugins and settings needed to run afterglow
    /// </summary>
    [DataContract]
    public class Profile : BaseModel
    {
        private AfterglowSetup _setup;
        /// <summary>
        /// A reference to the Setup object
        /// </summary>
        /// <remarks>AfterglowSetup is saved but not from here this object reference is set else where</remarks>
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

        ///<summary>
        /// General settings applied to the whole profile
        ///</summary>
        #region General Settings
        /// <summary>
        /// The display name of this Profile
        /// </summary>
        [DataMember]
        [Required]
        [Display(Name = "Name", Order = 100, GroupName = "General Settings")]
        public string Name
        {
            get { return Get(() => Name, "Default Profile"); }
            set { Set(() => Name, value); }
        }

        /// <summary>
        /// A discription of how/what this Profile does differently to others
        /// </summary>
        [DataMember]
        [Required]
        [Display(Name = "Description", Order = 200, GroupName = "General Settings")]
        public string Description
        {
            get { return Get(() => Description); }
            set { Set(() => Description, value); }
        }
        
        /// <summary>
        /// This sets the minimum time that the program will take to complete each loop
        /// Frame Rate = Frame Rate Limiter + Execution time of program
        /// Default - 1 Millisecond
        /// </summary>
        [DataMember]
        [Required]
        [Display(Name = "Frame Rate Limiter (Milliseconds)", 
            Order = 300, 
            Description = "This sets the minimum time that the program will take to complete each loop. Frame Rate = Frame Rate Limiter + Execution time of program", 
            GroupName = "General Settings")]
        public double FrameRateLimiter
        {
            get { return Get(() => FrameRateLimiter, 1.00); }
            set { Set(() => FrameRateLimiter, value); }
        }
        #endregion

        ///<summary>
        /// References to selected plugins
        /// Changing the configuration of one plugin applies to the configuration of that plugin for all the profiles it is used in
        /// SerializableInterfaceList has been used to store all plugins so that the adding/removing/saving/loading between all plugins works the same
        ///</summary>
        #region Selected Plugins
        /// <summary>
        /// Selected Light Setup Plugin
        /// <strong>Only one item should be stored in this collection</strong>
        /// </summary>
        [DataMember]
        [Required]
        public SerializableInterfaceList<ILightSetupPlugin> LightSetupPlugins
        {
            get { return Get(() => LightSetupPlugins, () => new SerializableInterfaceList<ILightSetupPlugin>()); }
            set { Set(() => LightSetupPlugins, value); }
        }

        /// <summary>
        /// Gets the configured Light Setup Plugin
        /// </summary>
        [XmlIgnore]
        public ILightSetupPlugin LightSetupPlugin
        {
            get
            {
                return LightSetupPlugins.FirstOrDefault();
            }
        }
        /// <summary>
        /// Selected Capture Plugin
        /// <strong>Only one item should be stored in this collection</strong>
        /// </summary>
        [DataMember]
        [Required]
        public SerializableInterfaceList<ICapturePlugin> CapturePlugins
        {
            get { return Get(() => CapturePlugins, () => new SerializableInterfaceList<ICapturePlugin>()); }
            set { Set(() => CapturePlugins, value); }
        }
        /// <summary>
        /// Gets the configured Capture Plugin
        /// </summary>
        [XmlIgnore]
        public ICapturePlugin CapturePlugin
        {
            get
            {
                return CapturePlugins.FirstOrDefault();
            }
        }

        /// <summary>
        /// Selected Colour Extraction Plugin
        /// <strong>Only one item should be stored in this collection</strong>
        /// </summary>
        [DataMember]
        [Required]
        public SerializableInterfaceList<IColourExtractionPlugin> ColourExtractionPlugins
        {
            get { return Get(() => ColourExtractionPlugins, () => new SerializableInterfaceList<IColourExtractionPlugin>()); }
            set { Set(() => ColourExtractionPlugins, value); }
        }
        /// <summary>
        /// Gets the configured Colour Extraction Plugin
        /// </summary>
        [XmlIgnore]
        public IColourExtractionPlugin ColourExtractionPlugin
        {
            get
            {
                return ColourExtractionPlugins.FirstOrDefault();
            }
        }

        /// <summary>
        /// Selected Post Process Plugins
        /// Optional - may contain zero or more plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<IPostProcessPlugin> PostProcessPlugins
        {
            get { return Get(() => PostProcessPlugins, () => new SerializableInterfaceList<IPostProcessPlugin>()); }
            set { Set(() => PostProcessPlugins, value); }
        }

        /// <summary>
        /// Selected Output Plugins
        /// Must contain at least one plugin
        /// </summary>
        [DataMember]
        [Required]
        public SerializableInterfaceList<IOutputPlugin> OutputPlugins
        {
            get { return Get(() => OutputPlugins, new SerializableInterfaceList<IOutputPlugin>()); }
            set { Set(() => OutputPlugins, value); }
        }

        #endregion
        
        /// <summary>
        /// This rebuilds Selected Plugins with objects from AfterglowSetup.Configured*Plugins
        /// So that object references are re-used therefore will also use less RAM
        /// </summary>
        internal void OnDeserialized()
        {
            SerializableInterfaceList<ILightSetupPlugin> lightSetupPlugins = new SerializableInterfaceList<ILightSetupPlugin>();
            foreach (ILightSetupPlugin lightSetupPlugin in LightSetupPlugins)
            {
                ILightSetupPlugin existingPlugin = this.Setup.ConfiguredLightSetupPlugins.Single(p => p.Id == lightSetupPlugin.Id);

                //Add to configured list if not found
                if (existingPlugin == null)
                {
                    existingPlugin = lightSetupPlugin;
                    existingPlugin.Id = this.Setup.GetNewId<ILightSetupPlugin>();
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
                    existingPlugin.Id = this.Setup.GetNewId<ICapturePlugin>();
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
                    existingPlugin.Id = this.Setup.GetNewId<IColourExtractionPlugin>();
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
                    existingPlugin.Id = this.Setup.GetNewId<IPostProcessPlugin>();
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
                    existingPlugin.Id = this.Setup.GetNewId<IOutputPlugin>();
                    this.Setup.ConfiguredOutputPlugins.Add(existingPlugin);
                }
                outputPlugins.Add(existingPlugin);
            }
            this.OutputPlugins = outputPlugins;
        }

        /// <summary>
        /// Extra validation
        /// </summary>
        public void Validate()
        {
            #region LightSetupPlugin Validation
            if (LightSetupPlugins == null)
                throw new ValidationException("Light Setup Plugins cannot be null");
            if (LightSetupPlugins.Count != 1)
                throw new ValidationException("Only 1 Light Setup Plugin can be set");
            #endregion

            #region CapturePlugin Validation
            if (CapturePlugins == null)
                throw new ValidationException("Capture Plugins cannot be null");
            if (CapturePlugins.Count != 1)
                throw new ValidationException("Only 1 Capture Plugin can be set");
            #endregion

            #region ColourExtractionPlugin Validation
            if (ColourExtractionPlugins == null)
                throw new ValidationException("Colour Extraction Plugins cannot be null");
            if (ColourExtractionPlugins.Count != 1)
                throw new ValidationException("Only 1 Colour Extraction Plugin can be set");
            #endregion

            #region PostProcessPlugin Validation
            if (PostProcessPlugins == null)
                throw new ValidationException("Post Process Plugins cannot be null");
            #endregion

            #region OutputPlugin Validation
            if (OutputPlugins == null)
                throw new ValidationException("Output Plugins cannot be null");
            if (ColourExtractionPlugins.Count <= 0)
                throw new ValidationException("At least 1 Output Plugin must be set");
            #endregion
        }
    }
}
