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
using Afterglow.Core.IO;

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
        
        [DataMember]
        [Required]
        [Display(Name = "Capture Frequency (Hz)",
            Order = 300,
            Description = "Target capture frequency in Hertz",
            GroupName = "General Settings")]
        public int CaptureFrequency
        {
            get { return Get(() => CaptureFrequency, 10); }
            set { Set(() => CaptureFrequency, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Output Frequency (Hz)",
            Order = 300,
            Description = "Target output frequency in Hertz",
            GroupName = "General Settings")]
        public int OutputFrequency
        {
            get { return Get(() => OutputFrequency, 30); }
            set { Set(() => OutputFrequency, value); }
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
        /// Selected Pre-Output Plugins
        /// Optional - may contain zero or more plugins
        /// </summary>
        [DataMember]
        public SerializableInterfaceList<IPreOutputPlugin> PreOutputPlugins
        {
            get { return Get(() => PreOutputPlugins, () => new SerializableInterfaceList<IPreOutputPlugin>()); }
            set { Set(() => PreOutputPlugins, value); }
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
        
        internal void OnDeserialized()
        {
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

            #region PreOutputPlugin Validation
            if (this.PreOutputPlugins == null)
                throw new ValidationException("Pre Output Plugins cannot be null");
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
