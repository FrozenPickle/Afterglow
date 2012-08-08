using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core;

namespace Afterglow.Web
{
    class Program
    {
        private static AfterglowRuntime _runtime;
        static void Main(string[] args)
        {


            _runtime = new AfterglowRuntime();

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
