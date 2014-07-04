using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections;
using Afterglow.Core.Plugins;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Afterglow.Core.Log;

namespace Afterglow.Core.IO
{
    /// <summary>
    /// Holds the a Afterglow Plugin Loader object
    /// </summary>
    public static class PluginLoader
    {
        private static AfterglowPluginLoader _loader;

        /// <summary>
        /// Loads/Reloads all plugins types
        /// </summary>
        public static void Load()
        {
            Loader.Load();
        }

        /// <summary>
        /// Gets a AfterglowPluginLoader object
        /// </summary>
        public static AfterglowPluginLoader Loader
        {
            get
            {
                if (_loader == null)
                {
                    _loader = new AfterglowPluginLoader();
                }
                return _loader;
            }
        }
    }

    public class AfterglowPluginLoader
    {
        public const string PLUGINS_DIRECTORY = "Plugins";

        [ImportMany]
        public ILightSetupPlugin[] LightSetupPlugins { get; set; }
        public Type[] LightSetupPluginTypes
        {
            get
            {
                if (this.LightSetupPlugins == null)
                {
                    return new Type[0];
                }
                else
                {
                    return (from p in this.LightSetupPlugins
                            select p.GetType()).ToArray();
                }
            }
        }

        [ImportMany]
        public ICapturePlugin[] CapturePlugins { get; set; }
        public Type[] CapturePluginTypes
        {
            get
            {
                if (this.CapturePlugins == null)
                {
                    return new Type[0];
                }
                else
                {
                    return (from p in this.CapturePlugins
                            select p.GetType()).ToArray();
                }
            }
        }
        
        [ImportMany]
        public IColourExtractionPlugin[] ColourExtractionPlugins { get; set; }
        public Type[] ColourExtractionPluginTypes
        {
            get
            {
                if (this.ColourExtractionPlugins == null)
                {
                    return new Type[0];
                }
                else
                {
                    return (from p in this.ColourExtractionPlugins
                            select p.GetType()).ToArray();
                }
            }
        }
        
        [ImportMany]
        public IPostProcessPlugin[] PostProcessPlugins { get; set; }
        public Type[] PostProcessPluginTypes
        {
            get
            {
                if (this.PostProcessPlugins == null)
                {
                    return new Type[0];
                }
                else
                {
                    return (from p in this.PostProcessPlugins
                            select p.GetType()).ToArray();
                }
            }
        }
        
        [ImportMany]
        public IPreOutputPlugin[] PreOutputPlugins { get; set; }
        public Type[] PreOutputPluginTypes
        {
            get
            {
                if (this.PreOutputPlugins == null)
                {
                    return new Type[0];
                }
                else
                {
                    return (from p in this.PreOutputPlugins
                            select p.GetType()).ToArray();
                }
            }
        }
        
        [ImportMany]
        public IOutputPlugin[] OutputPlugins { get; set; }
        public Type[] OutputPluginTypes
        {
            get
            {
                if (this.OutputPlugins == null)
                {
                    return new Type[0];
                }
                else
                {
                    return (from p in this.OutputPlugins
                            select p.GetType()).ToArray();
                }
            }
        }

        public Type[] AllPlugins
        {
            get
            {
                List<Type> types = this.LightSetupPluginTypes.ToList();
                types.AddRange(this.CapturePluginTypes);
                types.AddRange(this.ColourExtractionPluginTypes);
                types.AddRange(this.PostProcessPluginTypes);
                types.AddRange(this.PreOutputPluginTypes);
                types.AddRange(this.OutputPluginTypes);
                
                return types.ToArray();
            }
        }
        public void Load()
        {
            Compose();
        }

        private void Compose()
        {
            AfterglowRuntime.Logger.Info("Loading Plugins...");
            string folder = Path.Combine((new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location)).Directory.FullName, PLUGINS_DIRECTORY);

            var catalog = new DirectoryCatalog(folder);
            var container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(this);

                AfterglowRuntime.Logger.Info("The following plugins were loaded");
                if (LightSetupPlugins != null && LightSetupPlugins.Any())
                {
                    LightSetupPluginTypes.ToList().ForEach(p => AfterglowRuntime.Logger.Info("Type: Light Setup Plugin Name: {0}", p.Name));
                }
                else
                {
                    AfterglowRuntime.Logger.Fatal("No Light Setup Plugins were loaded");
                }
                if (CapturePlugins != null && CapturePlugins.Any())
                {
                    CapturePluginTypes.ToList().ForEach(p => AfterglowRuntime.Logger.Info("Type: Capture Plugin Name: {0}", p.Name));
                }
                else
                {
                    AfterglowRuntime.Logger.Fatal("No Capture Plugins were loaded");
                }
                if (ColourExtractionPlugins != null && ColourExtractionPlugins.Any())
                {
                    ColourExtractionPluginTypes.ToList().ForEach(p => AfterglowRuntime.Logger.Info("Type: Colour Extraction Plugin Name: {0}", p.Name));
                }
                else
                {
                    AfterglowRuntime.Logger.Info("No Colour Extraction Plugins were loaded");
                }
                if (PostProcessPlugins != null && PostProcessPlugins.Any())
                {
                    PostProcessPluginTypes.ToList().ForEach(p => AfterglowRuntime.Logger.Info("Type: Post Process Plugin Name: {0}", p.Name));
                }
                else
                {
                    AfterglowRuntime.Logger.Info("No Post Process Plugins were loaded");
                }
                if (PreOutputPlugins != null && PreOutputPlugins.Any())
                {
                    PreOutputPluginTypes.ToList().ForEach(p => AfterglowRuntime.Logger.Info("Type: Pre Output Plugin Name: {0}", p.Name));
                }
                else
                {
                    AfterglowRuntime.Logger.Info("No Pre Output Plugins were loaded");
                }
                if (OutputPlugins != null && OutputPlugins.Any())
                {
                    OutputPluginTypes.ToList().ForEach(p => AfterglowRuntime.Logger.Info("Type: Output Plugin Name: {0}", p.Name));
                }
                else
                {
                    AfterglowRuntime.Logger.Fatal("No Output Plugins were loaded");
                }

            }
            catch (System.Reflection.ReflectionTypeLoadException reflectionTypeLoadException)
            {
                foreach (Exception exception in reflectionTypeLoadException.LoaderExceptions)
	            {
                    AfterglowRuntime.Logger.Fatal(exception, "Plugin Loader");
	            }                
            }
            finally
            {
                AfterglowRuntime.Logger.Info("Plugin Loading Complete");
            }
        }
    }
}

