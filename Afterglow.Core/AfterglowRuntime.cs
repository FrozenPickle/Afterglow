using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using Afterglow.Core.Storage;
using Afterglow.Core.Log;
using Afterglow.Core.Load;

namespace Afterglow.Core
{
    public sealed class AfterglowRuntime
    {
        public Settings Settings { get; set; }

        public Profile CurrentProfile { get; set; }

        public ILoader Loader { get; set; }

        public bool ShowPreview { get; set; }
        
        private Task _mainLoopTask;
        private bool _active;
        private IDatabase _database;
        private ILogger _logger;

        public AfterglowRuntime(IDatabase database, ILogger logger, ILoader loader)
        {
            this._database = database;
            this._logger = logger;
            this.Loader = loader;

            Settings = new Settings(database.AddTable("Settings"), logger, this);
        }

        public ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public IDatabase Database
        {
            get
            {
                return _database;
            }
        }

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
                int cycles = 0;
                while (_active)
                {
                    ILightSetupPlugin lightSetupPlugin = CurrentProfile.LightSetupPlugin;

                    foreach (var light in lightSetupPlugin.Lights)
                    {
                        light.OldLEDColour = light.LEDColour;
                        light.OldSourceColour = light.SourceColour;
                    }

                    //TODO this will only run one capture plugin
                    IDictionary<Light, PixelReader> ledSources = CurrentProfile.CapturePlugin.Capture(lightSetupPlugin);
                    try
                    {
                        foreach (var keyValue in ledSources)
                        {
                            //TODO this will only run one colour extraction plugin
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
                            postProcessPlugin.Process(led);
                        }
                    }

                    CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Output(lightSetupPlugin.Lights.ToList()));

                    // TODO: implement timing logic


                    cycles++;
                    if (cycles % 5 == 0)
                    {
                        Debug.WriteLine("");
                    }
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
