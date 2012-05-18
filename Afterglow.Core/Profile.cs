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
        [ConfigTable(DisplayName = "Light Setup Plugin", IsHidden = true)]
        public ILightSetupPlugin LightSetupPlugin
        {
            get { return Get(() => LightSetupPlugin); }
            set { Set(() => LightSetupPlugin, value); }
        }

        public ILightSetupPlugin SetLightSetupPlugin(Type pluginType)
        {
            ILightSetupPlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as ILightSetupPlugin;
            this.LightSetupPlugin = plugin;
            SaveToStorage(() => this.LightSetupPlugin, this.LightSetupPlugin);
            return plugin;
        }
        #endregion

        #region Capture Plugins
        [ConfigTable(DisplayName = "Capture Plugin", IsHidden = true)]
        public ICapturePlugin CapturePlugin
        {
            get { return Get(() => CapturePlugin); }
            set { Set(() => CapturePlugin, value); }
        }

        public ICapturePlugin SetCapturePlugin(Type pluginType)
        {
            ICapturePlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as ICapturePlugin;
            this.CapturePlugin = plugin;
            SaveToStorage(() => this.CapturePlugin, this.CapturePlugin);
            return plugin;
        }

        #endregion

        #region Colour Extraction Plugins
        [ConfigTable(DisplayName = "Colour Extraction Plugin", IsHidden = true)]
        public IColourExtractionPlugin ColourExtractionPlugin
        {
            get { return Get(() => ColourExtractionPlugin); }
            set { Set(() => ColourExtractionPlugin, value); }
        }

        public IColourExtractionPlugin AddColourExtractionPlugin(Type pluginType)
        {
            IColourExtractionPlugin plugin = Activator.CreateInstance(pluginType, new object[] { Table.Database.AddTable(), Logger, Runtime }) as IColourExtractionPlugin;
            this.ColourExtractionPlugin = plugin;
            SaveToStorage(() => this.ColourExtractionPlugin, this.ColourExtractionPlugin);
            return plugin;
        }
        #endregion

        #region Post Process Plugins
        [ConfigTable(DisplayName = "Post Process Plugins", IsHidden = true)]
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
        #endregion

        #region Output Plugins
        [ConfigTable(DisplayName = "Output Plugins", IsHidden = true)]
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
