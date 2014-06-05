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
using Afterglow.Core.IO;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;
using log4net;

namespace Afterglow.Core
{
    /// <summary>
    /// The core Afterglow functionality
    /// </summary>
    public sealed class AfterglowRuntime
    {
        /// <summary>
        /// The capture loop task
        /// </summary>
        private Task _captureLoopTask;

        /// <summary>
        /// The output loop task
        /// </summary>
        private Task _outputLoopTask;

        public double CaptureLoopFPS { get { return _captureLoopFPS.FPS; } }
        public double CaptureLoopFrameTime { get { return _captureLoopFPS.FrameTimeInMilliseconds; } }
        public double OutputLoopFPS { get { return _outputLoopFPS.FPS; } }
        public double OutputLoopFrameTime { get { return _outputLoopFPS.FrameTimeInMilliseconds; } }

        private FpsCalculator _captureLoopFPS = new FpsCalculator();
        private FpsCalculator _outputLoopFPS = new FpsCalculator();

        public const string APPLICATION_DATA_FOLDER = "Afterglow";
        public const string SETUP_FILE_NAME = "AfterglowSetup.xml";
        public const string LOGGING_FILE = "Logs\\Log.txt";
        /// <summary>
        /// This log level is used until settings can be loaded
        /// </summary>
#if DEBUG
        public const int DEFAULT_LOG_LEVEL = LoggingLevels.LOG_LEVEL_DEBUG;
#else
        public const int DEFAULT_LOG_LEVEL = LoggingLevels.LOG_LEVEL_ERROR;
#endif
        /// <summary>
        /// Synchronisation object
        /// </summary>
        private object sync = new object();

        private bool _active = false;
        /// <summary>
        /// Setting this to false will stop the capture and output threads
        /// </summary>
        /// <remarks>Thread Safe</remarks>
        public bool Active
        {
            get
            {
                lock (sync)
                    return _active;
            }
            set
            {
                lock (sync)
                    _active = value;
            }
        }

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
        {
            Logger.Info("Runtime Initializing...");

            PluginLoader.Loader.Load();

            if (!this.Load())
            {
                return;
            }

            //Set the logging level from user configuration
            Logger.LoggingLevel = this.Setup.LogLevel;

            if (this.Setup.Profiles.Any())
            {
                this.CurrentProfile = (from p in this.Setup.Profiles
                                      where p.Id == this.Setup.CurrentProfileId
                                      select p).FirstOrDefault();
            }
            if (this.CurrentProfile == null)
            {
                Logger.Warn("No current profile set");
            }

            Logger.Info("Runtime Initialized");
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
                Logger.Info("Setting current profile...");
                if (value == null)
                {
                    _currentProfile = value;
                    this.Setup.CurrentProfileId = -1; //Set to invalid profile id
                }
                else
                {
                    try
                    {
                        this.Setup.CurrentProfileId = value.Id;

                        using (var stream = new MemoryStream())
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Profile));
                            serializer.Serialize(stream, value);
                            stream.Position = 0;

                            _currentProfile = (Profile)serializer.Deserialize(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex, "Setting AfterglowRuntime.CurrentProfile failed");
                    }
                }
                Logger.Info("Current profile set");
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
                    Type[] extraTypes = PluginLoader.Loader.AllPlugins;
                    
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
            Logger.Info("Loading Settings...");

            //Copy settings to user profile, this will stop overriding settings when getting new versions
            string environmentPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), APPLICATION_DATA_FOLDER);
            if (!System.IO.Directory.Exists(environmentPath))
            {
                System.IO.Directory.CreateDirectory(environmentPath);
            }
            _setupFileName = System.IO.Path.Combine(environmentPath, SETUP_FILE_NAME);
            string existingPath = System.IO.Path.Combine(Environment.CurrentDirectory, SETUP_FILE_NAME);

            if (!File.Exists(_setupFileName) && File.Exists(existingPath))
            {
                if (!IsFileLocked(existingPath))
                {
                    File.Copy(existingPath, _setupFileName);
                }
                else
                {
                    Logger.Fatal("Existing file in use");
                    return false;
                }
            }

            if (!File.Exists(_setupFileName))
            {
                Logger.Fatal("Settings not found");
                return false;
            }
            else if (IsFileLocked(_setupFileName))
            {
                Logger.Fatal("Settings file in use");
                return false;
            }
            else
            {
                //Gets all possibly used IAfterglowPlugins
                Type[] extraTypes = PluginLoader.Loader.AllPlugins;

                XmlSerializer serializer = new XmlSerializer(typeof(AfterglowSetup), extraTypes);
                
                serializer.UnknownNode += serializer_UnknownNode;

                StreamReader reader = new StreamReader(_setupFileName);
                
                object deserialized = serializer.Deserialize(reader.BaseStream);

                this.Setup = (AfterglowSetup)deserialized;

                reader.Dispose();

                this.Setup.OnDeserialized();

                Logger.Info("Settings Loaded");
                return true;
            }
            
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                Logger.Error("Unknown Node {0}", e.Name);
            }
        }

        private bool IsFileLocked(string filePath)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private static ILogger _logger;
        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    try
                    {
                        _logger = new Afterglow.Core.Log.AfterglowLogger(APPLICATION_DATA_FOLDER, LOGGING_FILE, DEFAULT_LOG_LEVEL);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Creating logger failed {0}"), ex);
                    }
                }
                return _logger;
            }
        }

        LightData _nextLightData;
        LightData _prevLightData;
        object previousSync = new object();

        /// <summary>
        /// Thread safe retrieval of the previous frame's final adjusted light data
        /// </summary>
        /// <returns>A copy of the current light data (can be null if no light data)</returns>
        /// <remarks>Frequent calling of this method may have a performance impact due to thread synchronisation.</remarks>
        public LightData GetPreviousLightData()
        {
            lock (previousSync)
            {
                if (_prevLightData == null)
                    return null;
                LightData data = new LightData(_prevLightData);
                return data;
            }
        }

        /// <summary>
        /// Start running the current profile
        /// </summary>
        public void Start()
        {
            Stop();
            Logger.Info("Starting...");

            bool profileJustSet = false;
            if (this.CurrentProfile == null)
            {
                this.CurrentProfile = (from p in this.Setup.Profiles
                                       where p.Id == this.Setup.CurrentProfileId
                                       select p).FirstOrDefault();
                profileJustSet = true;
            }

            if (this.CurrentProfile == null)
            {
                CurrentProfile = this.Setup.Profiles.FirstOrDefault();
                profileJustSet = true;
            }

            if (CurrentProfile == null)
            {
                Logger.Error("No profile has been selected");
                return;
            }
            else
            {
                //Reset current profile incase there has been any setting changes
                if (!profileJustSet)
                {
                    this.CurrentProfile = (from p in this.Setup.Profiles
                                           where p.Id == this.CurrentProfile.Id
                                           select p).First();
                }

                CurrentProfile.Validate();

                if (!Active)
                {
                    Active = true;

                    _captureLoopTask = new Task(CaptureLoop);
                    _outputLoopTask = new Task(OutputLoop);

                    // Prepare light data buffer
                    _nextLightData = new LightData(CurrentProfile.LightSetupPlugin.Lights.Count);
                    _prevLightData = new LightData(CurrentProfile.LightSetupPlugin.Lights.Count); ;

                    // Start all plugins
                    CurrentProfile.CapturePlugin.Start();
                    CurrentProfile.ColourExtractionPlugin.Start();
                    CurrentProfile.PostProcessPlugins.ToList().ForEach(p => p.Start());
                    CurrentProfile.PreOutputPlugins.ToList().ForEach(p => p.Start());

                    string errorMessage = String.Empty;
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
                    else
                    {
                        // Commence capture and output threads
                        _captureLoopTask.Start();
                        _outputLoopTask.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Stop running the current profile
        /// </summary>
        public void Stop()
        {
            Logger.Info("Stopping...");
            if (Active)
            {
                Active = false;

                //TODO the wait sometimes never finishes executing
                Task.WaitAll(_captureLoopTask, _outputLoopTask);
                _captureLoopTask.Dispose();
                _outputLoopTask.Dispose();
            }
        }

        private void CaptureLoop()
        {
            try
            {
                //Get Light Setup Plugin
                ILightSetupPlugin lightSetupPlugin = CurrentProfile.LightSetupPlugin;

                Stopwatch timer = Stopwatch.StartNew();
                long lastTicks = 0;
                double waitTicks = 1.0 / CurrentProfile.CaptureFrequency;
                double freq = 1.0 / Stopwatch.Frequency;

                LightData newLightData = new LightData(_nextLightData);

                while (Active)
                {
                    lastTicks = timer.ElapsedTicks;

                    //Get screen segments
                    IDictionary<Light, PixelReader> ledSources = CurrentProfile.CapturePlugin.Capture(lightSetupPlugin);
                    newLightData.Time = timer.ElapsedTicks;
                    try
                    {
                        //Extract Colours from segments
                        int i = 0;
                        foreach (var keyValue in ledSources)
                        {
                            newLightData[i] = CurrentProfile.ColourExtractionPlugin.Extract(keyValue.Key, keyValue.Value);
                            i++;
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
                        (postProcessPlugin as IPostProcessPlugin).Process(lightSetupPlugin.Lights, newLightData);
                    }

                    // Thread safe swap of new light data into _nextLightData
                    lock (sync)
                    {
                        var t = _nextLightData;
                        _nextLightData = newLightData;
                        newLightData = t;
                    }

                    // Throttle Capture Frequency
                    while ((timer.ElapsedTicks - lastTicks) * freq < waitTicks)
                    {
                        Thread.Sleep(5);
                    }

                    // Update capture FPS counter
                    _captureLoopFPS.Tick();
                }
            }
            catch (Exception e)
            {
                lock (sync)
                {
                    Active = false;
                    ErrorMessage = e.ToString();
                }
            }
            finally
            {
                // Allow plugins to clean up
                CurrentProfile.CapturePlugins.ForEach(cp => cp.Stop());
                CurrentProfile.PostProcessPlugins.ForEach(pp => pp.Stop());
            }
        }

        private void OutputLoop()
        {
            try
            {
                //Get Light Setup Plugin
                ILightSetupPlugin lightSetupPlugin = CurrentProfile.LightSetupPlugin;

                Stopwatch timer = new Stopwatch();
                double waitTicks = 1.0 / CurrentProfile.OutputFrequency;
                double freq = 1.0 / Stopwatch.Frequency;

                long lastUpdate = 0;

                // Initialise temporary lightData
                LightData lightData = new LightData(lightSetupPlugin.Lights.Count);

                while (Active)
                {
                    timer.Restart();

                    // If available, replace _currentLightData with _nextLightData
                    lock (sync)
                    {
                        if (lastUpdate < _nextLightData.Time)
                        {
                            var t = lightData;
                            lightData = _nextLightData;
                            _nextLightData = t;
                            lastUpdate = lightData.Time;
                        }
                    }

                    if (lightData != null)
                    {
                        // Run pre-output plugins
                        CurrentProfile.PreOutputPlugins.ToList().ForEach(po => po.PreOutput(lightSetupPlugin.Lights, lightData));

                        //Run Output Plugin(s)
                        CurrentProfile.OutputPlugins.ToList().ForEach(o => o.Output(lightSetupPlugin.Lights, lightData));
                    }

                    // Copy the last frame to _prevLightData
                    lock(previousSync)
                    {
                        Buffer.BlockCopy(lightData.ColourData, 0, _prevLightData.ColourData, 0, lightData.ColourData.Length);
                    }

                    // Throttle Output Frequency
                    while (timer.ElapsedTicks * freq < waitTicks)
                    {
                        Thread.Sleep(5);
                    }

                    _outputLoopFPS.Tick();
                }
            }
            catch (Exception e)
            {
                lock (sync)
                {
                    Active = false;
                    ErrorMessage = e.ToString();
                }
            }
            finally
            {
                // Allow plugins to clean up
                CurrentProfile.PreOutputPlugins.ForEach(po => po.Stop());
                CurrentProfile.OutputPlugins.ForEach(o => o.Stop());
            }
        }
    }
}