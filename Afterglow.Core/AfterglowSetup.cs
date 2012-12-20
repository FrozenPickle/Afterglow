using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Afterglow.Core.Plugins;
using Afterglow.Core.Load;
using System.ComponentModel.DataAnnotations;

namespace Afterglow.Core
{
    /// <summary>
    /// Contains the setup required to run Afterglow
    /// This is the root node in the XML saved document
    /// </summary>
    public class AfterglowSetup : BaseModel
    {

        /// <summary>
        /// All of the configured plugins exist here and then are referenced in each Profile
        /// </summary>
        /// <remarks>
        /// Loaded first by the XML Deserilization
        /// </remarks>
        #region Configured Plugins

        /// <summary>
        /// A list of configured Capture Plugins
        /// </summary>
        public SerializableInterfaceList<ICapturePlugin> ConfiguredCapturePlugins
        {
            get { return Get(() => ConfiguredCapturePlugins, new SerializableInterfaceList<ICapturePlugin>()); }
            set { Set(() => ConfiguredCapturePlugins, value); }
        }
        /// <summary>
        /// A list of configured Colour Extraction Plugins
        /// </summary>
        public SerializableInterfaceList<IColourExtractionPlugin> ConfiguredColourExtractionPlugins
        {
            get { return Get(() => ConfiguredColourExtractionPlugins, new SerializableInterfaceList<IColourExtractionPlugin>()); }
            set { Set(() => ConfiguredColourExtractionPlugins, value); }
        }
        /// <summary>
        /// A list of configured Light Setup Plugins
        /// </summary>
        public SerializableInterfaceList<ILightSetupPlugin> ConfiguredLightSetupPlugins
        {
            get { return Get(() => ConfiguredLightSetupPlugins, new SerializableInterfaceList<ILightSetupPlugin>()); }
            set { Set(() => ConfiguredLightSetupPlugins, value); }
        }
        /// <summary>
        /// A list of configured Post Process Plugins
        /// </summary>
        public SerializableInterfaceList<IPostProcessPlugin> ConfiguredPostProcessPlugins
        {
            get { return Get(() => ConfiguredPostProcessPlugins, new SerializableInterfaceList<IPostProcessPlugin>()); }
            set { Set(() => ConfiguredPostProcessPlugins, value); }
        }
        /// <summary>
        /// A list of configured Output Plugins
        /// </summary>
        public SerializableInterfaceList<IOutputPlugin> ConfiguredOutputPlugins
        {
            get { return Get(() => ConfiguredOutputPlugins, new SerializableInterfaceList<IOutputPlugin>()); }
            set { Set(() => ConfiguredOutputPlugins, value); }
        }
        #endregion

        /// <summary>
        /// A Generic function to aid in the creation of IPlugin identifers
        /// </summary>
        /// <typeparam name="T">Accepted types are ICapturePlugin, IColourExtractionPlugin, ILightSetupPlugin, IPostProcessPlugin, IOutputPlugin</typeparam>
        /// <returns>Integer Id for a new plugin</returns>
        public int GetNewPluginId<T>()
        {
            int result = 0;

            //Each query gets the last/largest Id used
            if (typeof(T) == typeof(ICapturePlugin))
            {
                result = this.ConfiguredCapturePlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(IColourExtractionPlugin))
            {
                result = this.ConfiguredColourExtractionPlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(ILightSetupPlugin))
            {
                result = this.ConfiguredLightSetupPlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(IPostProcessPlugin))
            {
                result = this.ConfiguredPostProcessPlugins.Max(plugin => plugin.Id);
            }
            else if (typeof(T) == typeof(IOutputPlugin))
            {
                result = this.ConfiguredOutputPlugins.Max(plugin => plugin.Id);
            }
            return result++;
        }

        /// <summary>
        /// A list of all Profiles
        /// </summary>
        /// <remarks>
        /// Loaded Second by the XML Deserilization, to re-using Configured Afterglow Plugins objects to ensure referential integrity
        /// </remarks>
        public List<Profile> Profiles
        {
            get { return Get(() => Profiles, new List<Profile>()); }
            set { Set(() => Profiles, value); }
        }

        /// <summary>
        /// The Current Profile Id
        /// </summary>
        [Required]
        public int CurrentProfileId
        {
            get { return Get(() => CurrentProfileId, 1); }
            set { Set(() => CurrentProfileId, value); }
        }
        
        /// <summary>
        /// Gets the Type of Plugins, used when creating a new configured plugin
        /// </summary>
        /// <remarks>
        /// Not Loaded as they are loaded from the file system and the project
        /// </remarks>
        #region Available Plugin Types
        /// <summary>
        /// Available Light Setup Plugin Types
        /// </summary>
        [XmlIgnore]
        public Type[] AvailableLightSetupPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<ILightSetupPlugin>(); }
        }
        /// <summary>
        /// Available Capture Plugin Types
        /// </summary>
        [XmlIgnore]
        public Type[] AvailableCapturePlugins
        {
            get { return PluginLoader.Loader.GetPlugins<ICapturePlugin>(); }
        }
        /// <summary>
        /// Available Colour Extraction Plugin Types
        /// </summary>
        [XmlIgnore]
        public Type[] AvailableColourExtractionPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IColourExtractionPlugin>(); }
        }
        /// <summary>
        /// Available Post Process Plugin Types
        /// </summary>
        [XmlIgnore]
        public Type[] AvailablePostProcessPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IPostProcessPlugin>(); }
        }
        /// <summary>
        /// Available Available Output Plugin Types
        /// </summary>
        [XmlIgnore]
        public Type[] AvailableOutputPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IOutputPlugin>(); }
        }
        #endregion

        /// <summary>
        /// Gets the default plugins used when a new plugin is created
        /// </summary>
        #region Default Plugins
        /// <summary>
        /// Trys to get a configured Light Setup Plugin (no specific ordering),
        /// failing that it will attempt to create a new Light Setup Plugin from the available types (no specific ordering)
        /// </summary>
        /// <returns>A list with one Light Setup Plugin</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<ILightSetupPlugin> DefaultLightSetupPlugins()
        {
            SerializableInterfaceList<ILightSetupPlugin> lightSetupPlugins = new SerializableInterfaceList<ILightSetupPlugin>();
            if (this.ConfiguredLightSetupPlugins.Any())
            {
                lightSetupPlugins.Add(this.ConfiguredLightSetupPlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableLightSetupPlugins.FirstOrDefault();
                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No ILightSetupPlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    lightSetupPlugins.Add(Activator.CreateInstance(type) as ILightSetupPlugin);
                }
            }

            return lightSetupPlugins;
        }

        /// <summary>
        /// Trys to get a configured Capture Setup Plugin (no specific ordering),
        /// failing that it will attempt to create a new Capture Plugin from the available types (trys CopyScreenCapture first then, no specific ordering)
        /// </summary>
        /// <returns>A list with one Capture Plugin</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<ICapturePlugin> DefaultCapturePlugins()
        {
            SerializableInterfaceList<ICapturePlugin> capturePlugins = new SerializableInterfaceList<ICapturePlugin>();
            if (this.ConfiguredCapturePlugins != null && this.ConfiguredCapturePlugins.Any())
            {
                capturePlugins.Add(this.ConfiguredCapturePlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableCapturePlugins.Where(a => a.Name == "CopyScreenCapture").FirstOrDefault();
                if (type == null)
                {
                    type = AvailableCapturePlugins.FirstOrDefault();
                }

                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No ICapturePlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    capturePlugins.Add(Activator.CreateInstance(type) as ICapturePlugin);
                }
            }
            return capturePlugins;
        }

        /// <summary>
        /// Trys to get a configured Colour Extraction Plugin (no specific ordering),
        /// failing that it will attempt to create a new Colour Extraction Plugin from the available types (no specific ordering)
        /// </summary>
        /// <returns>A list with one Colour Extraction Plugin </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<IColourExtractionPlugin> DefaultColourExtractionPlugins()
        {
            SerializableInterfaceList<IColourExtractionPlugin> colourExtractionPlugins = new SerializableInterfaceList<IColourExtractionPlugin>();
            if (this.ConfiguredColourExtractionPlugins.Any())
            {
                colourExtractionPlugins.Add(this.ConfiguredColourExtractionPlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableColourExtractionPlugins.FirstOrDefault();
                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No IColourExtractionPlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    colourExtractionPlugins.Add(Activator.CreateInstance(type) as IColourExtractionPlugin);
                }
            }
            return colourExtractionPlugins;
        }

        /// <summary>
        /// Returns an empty list as there are no default Post Process Plugins
        /// </summary>
        /// <returns>An empty list object</returns>
        public SerializableInterfaceList<IPostProcessPlugin> DefaultPostProcessPlugins()
        {
            return new SerializableInterfaceList<IPostProcessPlugin>();
        }

        /// <summary>
        /// Trys to get a configured Output Plugin (no specific ordering),
        /// failing that it will attempt to create a new Output Plugin from the available types (no specific ordering)
        /// </summary>
        /// <returns>A list with one Output Plugin</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public SerializableInterfaceList<IOutputPlugin> DefaultOutputPlugins()
        {
            SerializableInterfaceList<IOutputPlugin> outputPlugins = new SerializableInterfaceList<IOutputPlugin>();
            if (this.ConfiguredOutputPlugins.Any())
            {
                outputPlugins.Add(this.ConfiguredOutputPlugins.FirstOrDefault());
            }
            else
            {
                Type type = AvailableOutputPlugins.FirstOrDefault();
                if (type == null)
                {
                    //TODO log error
                    throw new ArgumentNullException("No IOutputPlugin's have been loaded, please check the install and try again");
                }
                else
                {
                    outputPlugins.Add(Activator.CreateInstance(type) as IOutputPlugin);
                }
            }
            return outputPlugins;
        }
        #endregion


        ///<summary>
        /// General settings applied to the Afterglow runtime
        ///</summary>
        #region General Settings

        /// <summary>
        /// Gets and Sets the Port for the web interface, default is 8080
        /// </summary>
        [Required]
        public int Port
        {
            get { return Get(() => Port, 8080); }
            set { Set(() => Port, value); }
        }
        /// <summary>
        /// Gets and Sets the UserName for the web interface, default is Afterglow
        /// </summary>
        [Required]
        public string UserName
        {
            get { return Get(() => UserName, "Afterglow"); }
            set { Set(() => UserName, value); }
        }
        /// <summary>
        /// Gets and Sets the Password for the web interface
        /// </summary>
        public string Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value); }
        }
        #endregion

        /// <summary>
        /// When this object has been deserialized this will get called and set sub object settings
        /// </summary>
        internal void OnDeserialized()
        {
            //Set the parents
            foreach (Profile profile in this.Profiles)
            {
                profile.Setup = this;
                profile.OnDeserialized();
            }
        }
    }
}
