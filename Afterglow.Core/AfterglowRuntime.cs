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
    public sealed class AfterglowRuntime
    {
        public DynamicPluginLoader Loader;

        public bool ShowPreview { get; set; }

        private Task _mainLoopTask;
        private bool _active;
        
        private string _setupFileName;

        public AfterglowRuntime(string SetupFileName = "AfterglowSetup.xml")
        {
            this._setupFileName = SetupFileName;

            this.Loader = new DynamicPluginLoader();

            PluginLoader.Loader = this.Loader;


            if (!this.Load())
            {
                this.Setup = new AfterglowSetup();

            }

            CurrentProfile = this.Setup.Profiles.First();
            this.Start();
            Console.Read();
            //this.Setup.ConfiguredCapturePlugins = this.Setup.DefaultCapturePlugins();
            //this.Save();
        }

        private AfterglowSetup _setup;
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

        /// <summary>
        /// Current profile contains a copied object of the original
        /// copying is done so that changes to settings can be made 
        /// without affecting the currently running profile
        /// copying is done using XML as this has been setup already
        /// </summary>
        private Profile _currentProfile;
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

        public bool Save()
        {
            if (this.Setup != null)
            {
                try
                {
                    Type[] extraTypes = this.Loader.GetPlugins<IAfterglowPlugin>();
                    // use reflection to look at all assemblies

                    XmlSerializer serializer = new XmlSerializer(typeof(AfterglowSetup),extraTypes);
                    
                    StreamWriter writer = new StreamWriter(_setupFileName);
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
        
        void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public bool Load()
        {
            
            //this.Save();
            if (File.Exists(_setupFileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AfterglowSetup));

                //Good for debug
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

        private void MainLoop()
        {

            /*
            1. Capture regions
            2. Perform colour extraction
            3. Perform post processing
            4. Cleanup Captured Regions
            5. Perform Output(s)
            6. Perform timing waits / whatever
            7. Check for messages (stop etc)
             * */

            CurrentProfile.CapturePlugin.Start();
            CurrentProfile.ColourExtractionPlugin.Start();
            CurrentProfile.PostProcessPlugins.ToList().ForEach(p => p.Start());
            CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Start());

            // TODO: until Logging is complete assign a default logger

            try
            {
                //int cycles = 0;
                while (_active)
                {
                    ILightSetupPlugin lightSetupPlugin = CurrentProfile.LightSetupPlugin;

                    foreach (var light in lightSetupPlugin.Lights)
                    {
                        light.OldLEDColour = light.LEDColour;
                        light.OldSourceColour = light.SourceColour;
                    }

                    IDictionary<Light, PixelReader> ledSources = CurrentProfile.CapturePlugin.Capture(lightSetupPlugin);
                    try
                    {
                        foreach (var keyValue in ledSources)
                        {
                            keyValue.Key.SourceColour = CurrentProfile.ColourExtractionPlugin.Extract(keyValue.Key, keyValue.Value);
                            keyValue.Key.LEDColour = keyValue.Key.SourceColour;
                        }
                    }
                    finally
                    {
                        foreach (var keyValue in ledSources)
                            keyValue.Value.Dispose();

                        CurrentProfile.CapturePlugin.ReleaseCapture();
                    }

                    foreach (var postProcessPlugin in CurrentProfile.PostProcessPlugins)
                    {
                        foreach (var led in lightSetupPlugin.Lights)
                        {
                            (postProcessPlugin as IPostProcessPlugin).Process(led);
                        }
                    }

                    CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Output(lightSetupPlugin.Lights.ToList()));

                    // TODO: implement timing logic

                    //TODO implement debug logging
                    //cycles++;
                    //if (cycles % 5 == 0)
                    //{
                    //    Debug.WriteLine("");
                    //}
                }


                // Set all to black
                Thread.Sleep(50);

                CurrentProfile.LightSetupPlugin.Lights.ToList().ForEach(led => led.LEDColour = Color.Black);
                CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Output(CurrentProfile.LightSetupPlugin.Lights.ToList()));

            }
            finally
            {
                CurrentProfile.CapturePlugin.Stop();
                CurrentProfile.ColourExtractionPlugin.Stop();
                CurrentProfile.PostProcessPlugins.ToList().ForEach(p => p.Stop());
                CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Stop());
            }
        }
    }
}
