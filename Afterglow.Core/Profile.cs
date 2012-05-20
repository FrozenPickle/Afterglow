using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Configuration;
using System.Reflection;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core.Log;
using System.Collections.ObjectModel;

namespace Afterglow.Core
{
    public class Profile : BaseRecord
    {

        public Profile(ITable table, ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        [ConfigString(DisplayName = "Name", SortIndex = 100)]
        public string Name
        {
            get { return Get(() => Name, "Default Profile"); }
            set { Set(() => Name, value); }
        }

        [ConfigString(DisplayName = "Description", SortIndex = 200)]
        public string Description
        {
            get { return Get(() => Description); }
            set { Set(() => Description, value); }
        }
        
        #region Light Setup Plugins
        [ConfigTable(DisplayName = "Light Setup Plugin", IsHidden = true, RetrieveValuesFrom ="GetLightSetupPlugins")]
        public ILightSetupPlugin LightSetupPlugin
        {
            get { return Get(() => LightSetupPlugin, () => DefaultLightSetupPlugin());}
            set { Set(() => LightSetupPlugin, value); }
        }

        public ILightSetupPlugin SetLightSetupPlugin(Type pluginType)
        {
            ILightSetupPlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as ILightSetupPlugin;
            this.LightSetupPlugin = plugin;
            SaveToStorage(() => this.LightSetupPlugin, this.LightSetupPlugin);
            return plugin;
        }

        public Type[] GetLightSetupPlugins()
        {
            //TODO log fatal error if there are no results found
            return this.Runtime.Loader.GetPlugins(typeof(ILightSetupPlugin)); 
        }

        public ILightSetupPlugin DefaultLightSetupPlugin()
        {
            Type type = GetLightSetupPlugins().FirstOrDefault();
            return SetLightSetupPlugin(type);
        }

        #endregion

        #region Capture Plugins
        [ConfigTable(DisplayName = "Capture Plugin", IsHidden = true, RetrieveValuesFrom = "GetCapturePlugins")]
        public ICapturePlugin CapturePlugin
        {
            get { return Get(() => CapturePlugin, () => DefaultCapturePlugin()); }
            set { Set(() => CapturePlugin, value); }
        }

        public ICapturePlugin SetCapturePlugin(Type pluginType)
        {
            ICapturePlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as ICapturePlugin;
            this.CapturePlugin = plugin;
            SaveToStorage(() => this.CapturePlugin, this.CapturePlugin);
            return plugin;
        }

        public Type[] GetCapturePlugins()
        {
            //TODO log fatal error if there are no results found
            return this.Runtime.Loader.GetPlugins(typeof(ICapturePlugin));
        }

        public ICapturePlugin DefaultCapturePlugin()
        {
            Type type = GetCapturePlugins().FirstOrDefault();
            return SetCapturePlugin(type);
        }

        #endregion

        #region Colour Extraction Plugins
        [ConfigTable(DisplayName = "Colour Extraction Plugin", IsHidden = true, RetrieveValuesFrom = "GetColourExtractionPlugins")]
        public IColourExtractionPlugin ColourExtractionPlugin
        {
            get { return Get(() => ColourExtractionPlugin, () => DefaultColourExtractionPlugin()); }
            set { Set(() => ColourExtractionPlugin, value); }
        }

        public IColourExtractionPlugin SetColourExtractionPlugin(Type pluginType)
        {
            IColourExtractionPlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as IColourExtractionPlugin;
            this.ColourExtractionPlugin = plugin;
            SaveToStorage(() => this.ColourExtractionPlugin, this.ColourExtractionPlugin);
            return plugin;
        }

        public Type[] GetColourExtractionPlugins()
        {
            //TODO log fatal error if there are no results found
            return this.Runtime.Loader.GetPlugins(typeof(IColourExtractionPlugin));
        }

        public IColourExtractionPlugin DefaultColourExtractionPlugin()
        {
            Type type = GetColourExtractionPlugins().FirstOrDefault();
            return SetColourExtractionPlugin(type);
        }
        #endregion

        #region Post Process Plugins
        [ConfigTable(DisplayName = "Post Process Plugins", IsHidden = true, RetrieveValuesFrom="GetPostProcessPlugins")]
        public ObservableCollection<IPostProcessPlugin> PostProcessPlugins
        {
            get { return Get(() => PostProcessPlugins); }
            set { Set(() => PostProcessPlugins, value); }
        }

        public IPostProcessPlugin AddPostProcessPlugin(Type pluginType)
        {
            IPostProcessPlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as IPostProcessPlugin;
            this.PostProcessPlugins.Add(plugin);
            SaveToStorage(() => this.PostProcessPlugins, this.PostProcessPlugins);
            return plugin;
        }

        public void RemovePostProcessPlugin(IPostProcessPlugin plugin)
        {
            this.PostProcessPlugins.Remove(plugin);
            SaveToStorage(() => this.PostProcessPlugins, this.PostProcessPlugins);
        }

        public Type[] GetPostProcessPlugins()
        {
            //TODO log fatal error if there are no results found
            return this.Runtime.Loader.GetPlugins(typeof(IPostProcessPlugin));
        }
        #endregion

        #region Output Plugins
        [ConfigTable(DisplayName = "Output Plugins", IsHidden = true, RetrieveValuesFrom="GetOutputPlugins")]
        public ObservableCollection<IOutputPlugin> OutputPlugins
        {
            get { return Get(() => OutputPlugins); }
            set { Set(() => OutputPlugins, value); }
        }

        public IOutputPlugin AddOutputPlugin(Type pluginType)
        {
            IOutputPlugin plugin = Activator.CreateInstance(pluginType, new object[]{ Table.Database.AddTable(),Logger,Runtime}) as IOutputPlugin;
            this.OutputPlugins.Add(plugin);
            SaveToStorage(() => this.OutputPlugins, this.OutputPlugins);
            return plugin;
        }

        public void RemoveOutputPlugin(IOutputPlugin plugin)
        {
            this.OutputPlugins.Remove(plugin);
            SaveToStorage(() => this.OutputPlugins, this.OutputPlugins);
        }

        public Type[] GetOutputPlugins()
        {
            //TODO log fatal error if there are no results found
            return this.Runtime.Loader.GetPlugins(typeof(IOutputPlugin));
        }
        #endregion

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
