using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Diagnostics;
using Afterglow.Core;

namespace Afterglow.Plugins.Output
{
    public class DebugOutput : BasePlugin, IOutputPlugin
    {
        #region Read Only Properties
        public override string Name
        {
            get { return "Debug Output Plugin"; }
        }

        public override string Description
        {
            get { return "A Debugging Plugin"; }
        }

        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion


        public void Output(List<Core.Light> leds)
        {
            
            Debug.WriteLine("DebugOutput LEDs:");
            Debug.WriteLine("LED count: {0}", leds.Count);
            
        }

        public override void Start()
        {
            Debug.WriteLine("DebugOutput: OnStart");
        }

        public override void Stop()
        {
            Debug.WriteLine("DebugOutput: OnStop");
        }

    }
}
