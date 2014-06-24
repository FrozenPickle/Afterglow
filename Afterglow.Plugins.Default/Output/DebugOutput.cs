using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Diagnostics;
using Afterglow.Core;
using System.Runtime.Serialization;
using System.ComponentModel.Composition;

namespace Afterglow.Plugins.Output
{
    [DataContract]
    [Export(typeof(IOutputPlugin))]
    public class DebugOutput : BasePlugin, IOutputPlugin
    {
        #region Read Only Properties
        [DataMember]
        public override string Name
        {
            get { return "Debug Output Plugin"; }
        }

        [DataMember]
        public override string Description
        {
            get { return "A Debugging Plugin"; }
        }

        [DataMember]
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion


        public void Output(List<Core.Light> leds, LightData data)
        {
            //Debug.WriteLine("DebugOutput LEDs:");
            //Debug.WriteLine("LED count: {0}", leds.Count);
        }

        public override void Start()
        {
            Debug.WriteLine("DebugOutput: OnStart");
        }

        public override void Stop()
        {
            Debug.WriteLine("DebugOutput: OnStop");
        }

        public bool TryStart()
        {
            Start();
            return true;
        }
    }
}
