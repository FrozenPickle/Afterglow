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
            if (_loader == null)
            {
                _loader = new AfterglowPluginLoader();
            }
            _loader.Load();
        }

        /// <summary>
        /// Gets a AfterglowPluginLoader object
        /// </summary>
        public static AfterglowPluginLoader Loader
        {
            get
            {
                Load();
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
            string folder = Path.Combine(Environment.CurrentDirectory, PLUGINS_DIRECTORY);

            var catalog = new DirectoryCatalog(folder);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }
}

