using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Afterglow.Core.Plugins;
using Afterglow.Core.Load;

namespace Afterglow.Core
{
    public class AfterglowSetup : BaseModel
    {

        ///Loaded First
        #region Configured Plugins
        public SerializableInterfaceList<ICapturePlugin> ConfiguredCapturePlugins
        {
            get { return Get(() => ConfiguredCapturePlugins);} //, new SerializableInterfaceList<ICapturePlugin>()); }
            set { Set(() => ConfiguredCapturePlugins, value); }
        }

        public SerializableInterfaceList<IColourExtractionPlugin> ConfiguredColourExtractionPlugins
        {
            get { return Get(() => ConfiguredColourExtractionPlugins, new SerializableInterfaceList<IColourExtractionPlugin>()); }
            set { Set(() => ConfiguredColourExtractionPlugins, value); }
        }

        public SerializableInterfaceList<ILightSetupPlugin> ConfiguredLightSetupPlugins
        {
            get { return Get(() => ConfiguredLightSetupPlugins, new SerializableInterfaceList<ILightSetupPlugin>()); }
            set { Set(() => ConfiguredLightSetupPlugins, value); }
        }

        public SerializableInterfaceList<IPostProcessPlugin> ConfiguredPostProcessPlugins
        {
            get { return Get(() => ConfiguredPostProcessPlugins, new SerializableInterfaceList<IPostProcessPlugin>()); }
            set { Set(() => ConfiguredPostProcessPlugins, value); }
        }

        public SerializableInterfaceList<IOutputPlugin> ConfiguredOutputPlugins
        {
            get { return Get(() => ConfiguredOutputPlugins, new SerializableInterfaceList<IOutputPlugin>()); }
            set { Set(() => ConfiguredOutputPlugins, value); }
        }
        #endregion

        public int GetPluginId<T>()
        {
            int result = 0;

            if (typeof(T) == typeof(ICapturePlugin))
            {
                result = (from i in this.ConfiguredCapturePlugins
                          orderby i.Id
                          select i.Id).FirstOrDefault();
            }
            else if (typeof(T) == typeof(IColourExtractionPlugin))
            {
                result = (from i in this.ConfiguredColourExtractionPlugins
                          orderby i.Id
                          select i.Id).FirstOrDefault();
            }
            else if (typeof(T) == typeof(ILightSetupPlugin))
            {
                result = (from i in this.ConfiguredLightSetupPlugins
                          orderby i.Id
                          select i.Id).FirstOrDefault();
            }
            else if (typeof(T) == typeof(IPostProcessPlugin))
            {
                result = (from i in this.ConfiguredPostProcessPlugins
                          orderby i.Id
                          select i.Id).FirstOrDefault();
            }
            else if (typeof(T) == typeof(IOutputPlugin))
            {
                result = (from i in this.ConfiguredOutputPlugins
                          orderby i.Id
                          select i.Id).FirstOrDefault();
            }
            return result;
        }

        //Loaded Second re-using ConfiguredAfterglowPlugins objects to ensure referential integrity
        public List<Profile> Profiles
        {
            get { return Get(() => Profiles, new List<Profile>()); }
            set { Set(() => Profiles, value); }
        }

        //Not loaded as they are loaded from the file system and the project
        #region Available Plugin Types
        [XmlIgnore]
        public Type[] AvailableLightSetupPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<ILightSetupPlugin>(); }
        }
        [XmlIgnore]
        public Type[] AvailableCapturePlugins
        {
            get { return PluginLoader.Loader.GetPlugins<ICapturePlugin>(); }
        }
        [XmlIgnore]
        public Type[] AvailableColourExtractionPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IColourExtractionPlugin>(); }
        }
        [XmlIgnore]
        public Type[] AvailablePostProcessPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IPostProcessPlugin>(); }
        }
        [XmlIgnore]
        public Type[] AvailableOutputPlugins
        {
            get { return PluginLoader.Loader.GetPlugins<IOutputPlugin>(); }
        }
        #endregion

        //Default plugins used when a new plugin is created
        #region Default Plugins
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
                }
                else
                {
                    lightSetupPlugins.Add(Activator.CreateInstance(type) as ILightSetupPlugin);
                }
            }

            return lightSetupPlugins;
        }

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
                    //TODO log error
                }
                else
                {
                    capturePlugins.Add(Activator.CreateInstance(type) as ICapturePlugin);
                }
            }
            return capturePlugins;
        }

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
                }
                else
                {
                    colourExtractionPlugins.Add(Activator.CreateInstance(type) as IColourExtractionPlugin);
                }
            }
            return colourExtractionPlugins;
        }

        //No Default
        public SerializableInterfaceList<IPostProcessPlugin> DefaultPostProcessPlugins()
        {
            return new SerializableInterfaceList<IPostProcessPlugin>();
        }

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
                }
                else
                {
                    outputPlugins.Add(Activator.CreateInstance(type) as IOutputPlugin);
                }
            }
            return outputPlugins;
        }
        #endregion

        //All other general settings go here
        #region General Settings
        public int? Port
        {
            get { return Get(() => Port, 8888); }
            set { Set(() => Port, value); }
        }

        public string UserName
        {
            get { return Get(() => UserName, "Afterglow"); }
            set { Set(() => UserName, value); }
        }

        public string Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value); }
        }
        #endregion

        //Actions to perform when this object has loaded from XML
        public void OnDeserialized()
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
