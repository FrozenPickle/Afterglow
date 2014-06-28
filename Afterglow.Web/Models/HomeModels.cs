using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Afterglow.Web.Models
{

    [DataContract]
    public class PreviewSetupResponse
    {
        [DataMember(Name = "lightSetup")]
        public LightSetup LightSetup { get; set; }
    }

    [DataContract]
    public class PreviewLightResponse
    {
        [DataMember(Name = "lights")]
        public List<LightPreview> Lights { get; set; }
        [DataMember(Name = "captureFPS")]
        public double CaptureFPS { get; set; }
        [DataMember(Name = "captureFrameTime")]
        public double CaptureFrameTime { get; set; }
        [DataMember(Name = "outputFPS")]
        public double OutputFPS { get; set; }
        [DataMember(Name = "outputFrameTime")]
        public double OutputFrameTime { get; set; }
    }

    [DataContract]
    public class LightPreview
    {
        [DataMember(Name = "top")]
        public int Top { get; set; }
        [DataMember(Name = "left")]
        public int Left { get; set; }
        [DataMember(Name = "colour")]
        public string Colour { get; set; }
    }
}
