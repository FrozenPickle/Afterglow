using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Afterglow.Core.Plugins;
using Afterglow.Core.IO;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Reflection;
using Afterglow.Core.Configuration;
using Microsoft.Win32;

namespace Afterglow.Core
{
    /// <summary>
    /// Contains the setup required to run Afterglow
    /// This is the root node in the XML saved document
    /// </summary>
    [DataContract]
    public class AfterglowSetup : BaseModel
    {
        /// <summary>
        /// A Generic function to aid in the creation of Profile and IPlugin identifers
        /// </summary>
        /// <typeparam name="T">Accepted types are Profile, ICapturePlugin, IColourExtractionPlugin, ILightSetupPlugin, IPostProcessPlugin, IOutputPlugin</typeparam>
        /// <returns>Integer Id for a new object of the given type</returns>
        public int GetNewId<T>()
        {
            int result = 0;

            //Each query gets the last/largest Id used
            try
            {
                if (typeof(T) == typeof(Profile))
                {
                    if (this.Profiles.Any())
                        result = this.Profiles.Max(profile => profile.Id);
                }
                else if (typeof(T) == typeof(ICapturePlugin))
                {
                    result = (from profile in this.Profiles
                              from plugin in profile.CapturePlugins
                              select plugin.Id).Max(p => p);
                }
                else if (typeof(T) == typeof(IColourExtractionPlugin))
                {
                    result = (from profile in this.Profiles
                              from plugin in profile.ColourExtractionPlugins
                              select plugin.Id).Max(p => p);
                }
                else if (typeof(T) == typeof(ILightSetupPlugin))
                {
                    result = (from profile in this.Profiles
                              from plugin in profile.LightSetupPlugins
                              select plugin.Id).Max(p => p);
                }
                else if (typeof(T) == typeof(IPostProcessPlugin))
                {
                    result = (from profile in this.Profiles
                              from plugin in profile.PostProcessPlugins
                              select plugin.Id).Max(p => p);
                }
                else if (typeof(T) == typeof(IPreOutputPlugin))
                {
                    result = (from profile in this.Profiles
                              from plugin in profile.PreOutputPlugins
                              select plugin.Id).Max(p => p);
                }
                else if (typeof(T) == typeof(IOutputPlugin))
                {
                    result = (from profile in this.Profiles
                              from plugin in profile.OutputPlugins
                              select plugin.Id).Max(p => p);
                }
            }
            catch (Exception)
            {
                result = 0;
            }
            return ++result;
        }

        /// <summary>
        /// A list of all Profiles
        /// </summary>
        /// <remarks>
        /// Loaded Second by the XML Deserilization, to re-using Configured Afterglow Plugins objects to ensure referential integrity
        /// </remarks>
        [DataMember]
        public List<Profile> Profiles
        {
            get { return Get(() => Profiles, new List<Profile>()); }
            set { Set(() => Profiles, value); }
        }

        /// <summary>
        /// Adds a new profile
        /// </summary>
        /// <returns>New Profile</returns>
        public Profile AddNewProfile()
        {
            Profile newProfile = new Profile();
            newProfile.Id = GetNewId<Profile>();
            newProfile.Setup = this;
            newProfile.LightSetupPlugins = DefaultLightSetupPlugins();
            newProfile.CapturePlugins = DefaultCapturePlugins();
            newProfile.ColourExtractionPlugins = DefaultColourExtractionPlugins();
            newProfile.PostProcessPlugins = DefaultPostProcessPlugins();
            newProfile.PreOutputPlugins = DefaultPreOutputPlugins();
            newProfile.OutputPlugins = DefaultOutputPlugins();
            this.Profiles.Add(newProfile);

            return newProfile;
        }

        /// <summary>
        /// The Current Profile Id
        /// </summary>
        [DataMember]
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
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableLightSetupPlugins
        {
            get { return PluginLoader.Loader.LightSetupPluginTypes; }
        }
        /// <summary>
        /// Available Capture Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableCapturePlugins
        {
            get { return PluginLoader.Loader.CapturePluginTypes; }
        }
        /// <summary>
        /// Available Colour Extraction Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableColourExtractionPlugins
        {
            get { return PluginLoader.Loader.ColourExtractionPluginTypes; }
        }
        /// <summary>
        /// Available Post Process Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailablePostProcessPlugins
        {
            get { return PluginLoader.Loader.PostProcessPluginTypes; }
        }
        /// <summary>
        /// Available Pre Output Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailablePreOutputPlugins
        {
            get { return PluginLoader.Loader.PreOutputPluginTypes; }
        }
        /// <summary>
        /// Available Available Output Plugin Types
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Type[] AvailableOutputPlugins
        {
            get { return PluginLoader.Loader.OutputPluginTypes; }
        }
        #endregion

        /// <summary>
        /// Gets the details of list of a afterglow plugin types
        /// </summary>
        /// <param name="pluginTypes">An array of IAfterglowPlugin types</param>
        public static IEnumerable<AvailablePluginDetails> GetAvailblePlugins(Type[] pluginTypes)
        {
            List<AvailablePluginDetails> result = new List<AvailablePluginDetails>();

            foreach (Type pluginType in pluginTypes)
            {
                AvailablePluginDetails availablePlugin = new AvailablePluginDetails();

                availablePlugin.Type = pluginType.Name;

                IAfterglowPlugin newObject = Activator.CreateInstance(pluginType) as IAfterglowPlugin;
                if (newObject != null)
                {
                    PropertyInfo nameProperty = pluginType.GetProperty("Name");
                    if (nameProperty != null)
                    {
                        availablePlugin.Name = nameProperty.GetValue(newObject, null) as string;
                    }
                    PropertyInfo descriptionProperty = pluginType.GetProperty("Description");
                    if (descriptionProperty != null)
                    {
                        availablePlugin.Description = descriptionProperty.GetValue(newObject, null) as string;
                    }
                }
                result.Add(availablePlugin);
            }

            return result;
        }

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
            Type type = AvailableLightSetupPlugins.FirstOrDefault();
            if (type == null)
            {
                AfterglowRuntime.Logger.Fatal("No ILightSetupPlugin's have been loaded, please check the install and try again");
            }
            else
            {
                ILightSetupPlugin plugin = null;
                if (this.Profiles.Any())
                {
                    plugin = this.Profiles.Last().LightSetupPlugin;
                }
                else
                {
                    plugin = Activator.CreateInstance(type) as ILightSetupPlugin;
                }

                //Add at least one light
                if (!plugin.Lights.Any())
                {
                    plugin.Lights.Add(new Light() { Id = 1, Index = 0, Height = 1, Width = 1 });
                }

                plugin.Id = this.GetNewId<ILightSetupPlugin>();
                lightSetupPlugins.Add(plugin);
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
            Type type = AvailableCapturePlugins.Where(a => a.Name == "CopyScreenCapture").FirstOrDefault();
            if (type == null)
            {
                type = AvailableCapturePlugins.FirstOrDefault();
            }

            if (type == null)
            {
                AfterglowRuntime.Logger.Fatal("No ICapturePlugin's have been loaded, please check the install and try again");
            }
            else
            {
                ICapturePlugin plugin = Activator.CreateInstance(type) as ICapturePlugin;
                plugin.Id = this.GetNewId<ICapturePlugin>();
                capturePlugins.Add(plugin);
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
            Type type = AvailableColourExtractionPlugins.FirstOrDefault();
            if (type == null)
            {
                AfterglowRuntime.Logger.Fatal("No IColourExtractionPlugin's have been loaded, please check the install and try again");
            }
            else
            {
                IColourExtractionPlugin plugin = Activator.CreateInstance(type) as IColourExtractionPlugin;
                plugin.Id = this.GetNewId<IColourExtractionPlugin>();
                colourExtractionPlugins.Add(plugin);
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
        /// Returns an empty list as there are no default Pre Output Plugins
        /// </summary>
        /// <returns>An empty list object</returns>
        public SerializableInterfaceList<IPreOutputPlugin> DefaultPreOutputPlugins()
        {
            return new SerializableInterfaceList<IPreOutputPlugin>();
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
            Type type = AvailableOutputPlugins.FirstOrDefault();
            if (type == null)
            {
                AfterglowRuntime.Logger.Fatal("No IOutputPlugin's have been loaded, please check the install and try again");
            }
            else
            {
                IOutputPlugin plugin = Activator.CreateInstance(type) as IOutputPlugin;
                plugin.Id = this.GetNewId<IOutputPlugin>();
                outputPlugins.Add(plugin);
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
        [DataMember]
        [Required]
        [Display(Name = "Port", Order = 100)]
        public int Port
        {
            get { return Get(() => Port, 8080); }
            set { Set(() => Port, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Logging Level", Order = 200)]
        [ConfigLookup(RetrieveValuesFrom = "LoggingLevels")]
        public int LogLevel
        {
            get { return Get(() => LogLevel, () => Afterglow.Core.Log.LoggingLevels.LOG_LEVEL_ERROR); }
            set { Set(() => LogLevel, value); }
        }


        [XmlIgnore]
        [Display(Name = "Run On Windows Startup", Order = 300)]
        public bool RunOnWindowsStartup
        {
            get
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                // Check to see the current state (running at startup or not)
                return rkApp.GetValue(AfterglowRuntime.APPLICATION_NAME) != null;
            }
            set
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                {
                    // Add the value in the registry so that the application runs at startup
                    rkApp.SetValue(AfterglowRuntime.APPLICATION_NAME, System.Reflection.Assembly.GetEntryAssembly().Location);
                }
                else
                {
                    // Remove the value from the registry so that the application doesn't start
                    rkApp.DeleteValue(AfterglowRuntime.APPLICATION_NAME, false);

                }
            }
        }

        #region Logging Levels
        [XmlIgnore]
        private LookupItem[] _loggingLevels;
        [XmlIgnore]
        public LookupItem[] LoggingLevels
        {
            get
            {
                if (_loggingLevels == null)
                {
                    List<LookupItem> loggingLevels = new List<LookupItem>();
                    loggingLevels.Add(new LookupItem() { Id = Afterglow.Core.Log.LoggingLevels.LOG_LEVEL_DEBUG, Name = "Debug" });
                    loggingLevels.Add(new LookupItem() { Id = Afterglow.Core.Log.LoggingLevels.LOG_LEVEL_INFORMATION, Name = "Information" });
                    loggingLevels.Add(new LookupItem() { Id = Afterglow.Core.Log.LoggingLevels.LOG_LEVEL_WARNING, Name = "Warning" });
                    loggingLevels.Add(new LookupItem() { Id = Afterglow.Core.Log.LoggingLevels.LOG_LEVEL_ERROR, Name = "Error" });
                    loggingLevels.Add(new LookupItem() { Id = Afterglow.Core.Log.LoggingLevels.LOG_LEVEL_FATAL, Name = "Fatal" });
                    _loggingLevels = loggingLevels.ToArray();
                }
                return _loggingLevels;
            }
        }


        /// <summary>
        /// Gets and Sets the UserName for the web interface, default is Afterglow
        /// </summary>
        [DataMember]
        [Required]
        public string UserName
        {
            get { return Get(() => UserName, "Afterglow"); }
            set { Set(() => UserName, value); }
        }
        /// <summary>
        /// Gets and Sets the Password for the web interface
        /// </summary>
        [DataMember]
        public string Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value); }
        }
        #endregion

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
