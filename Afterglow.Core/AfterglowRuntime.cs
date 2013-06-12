using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using Afterglow.Core.Log;
using Afterglow.Core.Load;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Afterglow.Log;
using System.Text.RegularExpressions;

namespace Afterglow.Core
{
    /// <summary>
    /// The core Afterglow functionality
    /// </summary>
    public sealed class AfterglowRuntime
    {
        /// <summary>
        /// The main loop task
        /// </summary>
        private Task _mainLoopTask;
        /// <summary>
        /// Setting this to false will stop the main loop
        /// </summary>
        private bool _active;
        
        /// <summary>
        /// File location of the setup xml file
        /// </summary>
        private string _setupFileName;

        /// <summary>
        /// The last error occured
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Creates an instance of the Afterglow Runtime using the current execution path to save/load the configuration
        /// </summary>
        public AfterglowRuntime()
            : this("AfterglowSetup.xml")
        {
        }
        
        /// <summary>
        /// Creates an instance of the Afterglow Runtime using the supplied configuration file
        /// </summary>
        /// <param name="SetupFileName">File path of xml configuration</param>
        public AfterglowRuntime(string SetupFileName)
        {
            this._setupFileName = SetupFileName;

            //If nothing could be loaded create a new setup file in the location specified
            if (!this.Load())
            {
                this.Setup = new AfterglowSetup();
            }

            CurrentProfile = this.Setup.Profiles.First();
        }

        private AfterglowSetup _setup;
        /// <summary>
        /// The current setup
        /// </summary>
        public AfterglowSetup Setup
        {
            get
            {
                return _setup;
            }
            set
            {
                if (value != null)
                {
                    value.Runtime = this;
                }
                this._setup = value;
            }
        }

        private Profile _currentProfile;
        /// <summary>
        /// Current profile contains a copied object of the original
        /// copying is done so that changes to settings can be made 
        /// without affecting the currently running profile
        /// copying is done using XML as this has been setup already
        /// </summary>
        public Profile CurrentProfile 
        {
            get
            {
                return _currentProfile;
            }
            set
            {
                if (value == null)
                {
                    _currentProfile = value;
                }
                else
                {
                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                            serializer.Serialize(stream, value);
                            stream.Position = 0;

                            _currentProfile = (Profile)serializer.Deserialize(stream);
                        }
                    }
                    catch (Exception)
                    {
                        //TODO log exception
                        //exception may occur from plugins not created by frozen pickle or we did a bad job :(
                    }
                }
            }
        }

        /// <summary>
        /// Save the current setup
        /// </summary>
        /// <param name="filePath">Optional - by default saving is done to path specfied on object creation, otherwise to the locaiton specified</param>
        /// <returns>Save success or failure</returns>
        public bool Save(string filePath = null)
        {
            if (this.Setup != null)
            {
                string saveFilePath = (filePath ?? _setupFileName);
                try
                {
                    //Gets all possibly used IAfterglowPlugins
                    Type[] extraTypes = PluginLoader.Loader.GetPlugins<IAfterglowPlugin>();
                    
                    XmlSerializer serializer = new XmlSerializer(typeof(AfterglowSetup), extraTypes);

                    StreamWriter writer = new StreamWriter(saveFilePath);
                    serializer.Serialize(writer.BaseStream, this.Setup);
                    writer.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    //TODO log exception
                    //exception may occur from plugins not created by frozen pickle or we did a bad job :(
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// Load Configuration from XML
        /// </summary>
        /// <returns>true - successfull or false - unsucessfull load</returns>
        public bool Load()
        {
            if (File.Exists(_setupFileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AfterglowSetup));

                //Good for debugging
                //serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);

                StreamReader reader = new StreamReader(_setupFileName);
                
                object deserialized = serializer.Deserialize(reader.BaseStream);

                this.Setup = (AfterglowSetup)deserialized;

                reader.Dispose();

                this.Setup.OnDeserialized();
                return true;
            }
            else
            {
                return false;
            }
        }

        //TODO implement logging
        //private ILogger _logger;
        //public ILogger Logger
        //{
        //    get
        //    {
        //        if (_logger == null)
        //        {
        //            this._logger = new Log4NetProxy(log4net.LogManager.GetLogger("LoggingSystem"));
        //        }
        //        return _logger;
        //    }
        //}

        /// <summary>
        /// Start running the current profile
        /// </summary>
        public void Start()
        {
            if (CurrentProfile == null)
                throw new InvalidOperationException("No profile has been selected");

            CurrentProfile.Validate();

            if (!_active)
            {
                _active = true;
                _mainLoopTask = new Task(MainLoop);
                _mainLoopTask.Start();
            }
        }

        /// <summary>
        /// Stop running the current profile
        /// </summary>
        public void Stop()
        {
            if (_active)
            {
                _active = false;

                //TODO the wait sometimes never finishes executing
                _mainLoopTask.Wait();
                _mainLoopTask.Dispose();
                _mainLoopTask = null;
            }
        }

        /// <summary>
        /// The main Afterglow Loop
        /// </summary>
        private void MainLoop()
        {
            string errorMessage = string.Empty;

            /*
            1. Capture regions
            2. Perform colour extraction
            3. Perform post processing
            4. Cleanup Captured Regions
            5. Perform Output(s)
            6. Perform timing waits
            7. Check for messages (stop etc)
             * */

            CurrentProfile.CapturePlugin.Start();
            CurrentProfile.ColourExtractionPlugin.Start();
            CurrentProfile.PostProcessPlugins.ToList().ForEach(p => p.Start());

            int outputFailures = 0;
            foreach (IOutputPlugin outputPlugin in CurrentProfile.OutputPlugins)
            {
                if (!outputPlugin.TryStart(out errorMessage))
                {
                    outputFailures++;
                    ErrorMessage = errorMessage;
                }
            }

            if (outputFailures == CurrentProfile.OutputPlugins.Count())
            {
                Stop();
            }

            try
            {
                //int cycles = 0;
                while (_active)
                {
                    //Get Light Setup Plugin
                    ILightSetupPlugin lightSetupPlugin = CurrentProfile.LightSetupPlugin;

                    //Set previous light colour
                    foreach (var light in lightSetupPlugin.Lights)
                    {
                        light.OldLightColour = light.LightColour;
                        light.OldSourceColour = light.SourceColour;
                    }

                    //Get screen segments
                    IDictionary<Light, PixelReader> ledSources = CurrentProfile.CapturePlugin.Capture(lightSetupPlugin);
                    try
                    {
                        //Extract Colours from segments
                        foreach (var keyValue in ledSources)
                        {
                            keyValue.Key.SourceColour = CurrentProfile.ColourExtractionPlugin.Extract(keyValue.Key, keyValue.Value);
                            keyValue.Key.LightColour = keyValue.Key.SourceColour;
                        }
                    }
                    finally
                    {
                        //Dispose of screen segments
                        foreach (var keyValue in ledSources)
                            keyValue.Value.Dispose();

                        //Dispose of whole screen
                        CurrentProfile.CapturePlugin.ReleaseCapture();
                    }

                    //Run any Post Process Plugins
                    foreach (var postProcessPlugin in CurrentProfile.PostProcessPlugins)
                    {
                        foreach (var led in lightSetupPlugin.Lights)
                        {
                            (postProcessPlugin as IPostProcessPlugin).Process(led);
                        }
                    }

                    //Run Output Plugin
                    CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Output(lightSetupPlugin.Lights.ToList()));

                    //Frame Rate Limiter
                    Thread.Sleep(TimeSpan.FromMilliseconds(CurrentProfile.FrameRateLimiter));

                    //TODO implement debug logging
                    //cycles++;
                    //if (cycles % 5 == 0)
                    //{
                    //    Debug.WriteLine("");
                    //}
                }

                // Set all to black
                CurrentProfile.LightSetupPlugin.Lights.ToList().ForEach(led => led.LightColour = Color.Black);
                CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Output(CurrentProfile.LightSetupPlugin.Lights.ToList()));

            }
            finally
            {
                //Dispose of anything remaining
                CurrentProfile.CapturePlugin.Stop();
                CurrentProfile.ColourExtractionPlugin.Stop();
                CurrentProfile.PostProcessPlugins.ToList().ForEach(p => p.Stop());
                CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Stop());
            }
        }
    }
}
