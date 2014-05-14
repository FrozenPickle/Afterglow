using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core;
using System.Net;
using ServiceStack.Logging;
using Afterglow.Web.Host;

namespace Afterglow.Web
{
    class Program
    {
        public static AfterglowRuntime Runtime
        {
            get
            {
                return _runtime;
            }
        }
        private static AfterglowRuntime _runtime;
                
        static void Main(string[] args)
        {


            using (var appHost = new AppHost())
            {
                //Copy settings to user profile, this will stop overriding settings when getting new versions
                string environmentPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Afterglow");
                if (!System.IO.Directory.Exists(environmentPath))
                {
                    System.IO.Directory.CreateDirectory(environmentPath);
                }
                string filePath = System.IO.Path.Combine(environmentPath, "AfterglowSetup.xml");
                string existingPath = System.IO.Path.Combine(Environment.CurrentDirectory, "AfterglowSetup.xml");
                if (!System.IO.File.Exists(filePath) && System.IO.File.Exists(existingPath))
                {
                    System.IO.File.Copy(existingPath, filePath);
                }

                if (!System.IO.File.Exists(filePath))
                {
                    Console.WriteLine("Settings not found");
                    Console.WriteLine("Press <enter> to exit.");
                    Console.ReadLine();
                    appHost.Stop();
                }

                Console.WriteLine("Loading Afterglow settings...");
                _runtime = new AfterglowRuntime(filePath);
            
                Console.WriteLine("Starting Afterglow runtime...");

                Console.WriteLine("Afterglow runtime started.");

                appHost.Init();
                string host = String.Format("http://localhost:{0}/", _runtime.Setup.Port);
                if (args.Length > 0)
                    host = String.Format("http://{0}:{1}/", args[0], _runtime.Setup.Port);
                
                appHost.Start(host);

                Console.WriteLine(host);
                
                Console.WriteLine("Press <enter> to exit.");
                Console.ReadLine();
                appHost.Stop();
            }


            //_runtime.Setup = new AfterglowSetup();
            //Profile p = new Profile();
            //p.Setup = _runtime.Setup;
            //_runtime.Setup.ConfiguredPostProcessPlugins.Add(new ColourCorrectionPostProcess() { DisplayName = "frank" });
            ////_runtime.Setup.Profiles.Add(_runtime.Settings.Profiles.FirstOrDefault());
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredLightSetupPlugins.Add(a.OLDLightSetupPlugin));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredCapturePlugins.Add(a.OLDCapturePlugin));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredColourExtractionPlugins.Add(a.OLDColourExtractionPlugin));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredPostProcessPlugins.AddRange(a.OLDPostProcessPlugins));
            //_runtime.Settings.Profiles.ToList().ForEach(a => _runtime.Setup.ConfiguredOutputPlugins.AddRange(a.OLDOutputPlugins));

            //Profile profile = new Profile();
            //profile.LightSetupPlugins.AddRange(_runtime.Setup.DefaultLightSetupPlugins());
            //profile.CapturePlugins.AddRange(_runtime.Setup.DefaultCapturePlugins());
            //profile.ColourExtractionPlugins.AddRange(_runtime.Setup.DefaultColourExtractionPlugins());
            //profile.PostProcessPlugins.AddRange(_runtime.Setup.DefaultPostProcessPlugins());
            //profile.OutputPlugins.AddRange(_runtime.Setup.DefaultOutputPlugins());
            //_runtime.Setup.Profiles.Add(profile);
            //_runtime.Setup.ConfiguredLightSetupPlugins.Add(.FirstOrDefault().OLDPostProcessPlugins.FirstOrDefault());
            //_runtime.Save();
        }
    }
}
