using Afterglow.Core.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Afterglow.Plugins.PreOutput
{
    [DataContract]
    [Export(typeof(IPreOutputPlugin))]
    public class TestPatternPreOutput: BasePlugin, IPreOutputPlugin
    {
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [DataMember]
        public override string Name
        {
            get { return "Test Pattern"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        [DataMember]
        public override string Description
        {
            get { return "Brown and Black test pattern"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [DataMember]
        public override string Author
        {
            get { return "Justin S"; }
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
        
        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public void PreOutput(List<Core.Light> lights, Core.LightData data)
        {
            for(var i = 0; i < data.Length; i++)
            {
                if (i % 2 == 0)
                    data[i] = System.Drawing.Color.Brown;
                else
                    data[i] = System.Drawing.Color.Black;
            }
        }
    }
}
