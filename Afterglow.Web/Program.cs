using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core;
using System.Net;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
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
        public static bool Active = false;
        
        static void Main(string[] args)
        {


            _runtime = new AfterglowRuntime();
            
            LogManager.LogFactory = new ConsoleLogFactory();

            using (var appHost = new AppHost())
            {
                Console.WriteLine("Starting Afterglow runtime...");
                //_runtime.Start();

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

        internal static void ToggleActive()
        {
            if (Active)
            {
                Runtime.Stop();
                Active = false;
            }
            else
            {
                Runtime.Start();
                Active = true;
            }
        }
    }
}
