using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using System.Runtime.Serialization;

namespace Afterglow.Web
{
    [Route("/runtime")]
    public class Runtime
    {
    }

    public class RuntimeResponse
    {
        public bool Active { get; set; }
        public int NumberOfLights { get; set; }
    }

    [Route("/setup")]
    public class Setup
    {
        public Afterglow.Core.AfterglowSetup UpdatedSetup { get; set; }
    }

    public class SetupResponse
    {
        public Afterglow.Core.AfterglowSetup Setup { get; set; }
    }

    [Route("/profiles")]
    [Route("/profiles/{Id}")]
    [Route("/profiles/name/{Name}")]
    public class Profiles
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Route("/preview")]
    public class Preview
    {

    }

    public class PreviewResponse
    {
        public List<LightPreview> Lights { get; set; }
    }

    [DataContract]
    public class LightPreview
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public Color Colour { get; set; }
    }

    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static implicit operator System.Drawing.Color(Color c)
        {
            return System.Drawing.Color.FromArgb(c.R, c.G, c.B);
        }

        public static implicit operator Color(System.Drawing.Color c)
        {
            return new Color() { R = c.R, G = c.G, B = c.B };
        }
    }

    [Csv(CsvBehavior.FirstEnumerable)]
    public class ProfilesResponse
    {
        public int Total { get; set; }
        public List<Afterglow.Core.Profile> Results { get; set; }
    }

    public class AfterglowService : Service
    {
        public object Get(Runtime request)
        {
            return new RuntimeResponse
            {
                Active = Program.Active,
                NumberOfLights = Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count
            };
        }

        public object Post(Runtime request)
        {
            Program.ToggleActive();
            return new RuntimeResponse
            {
                Active = Program.Active,
                NumberOfLights = Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count
            };
        }

        public object Get(Profiles request)
        {
            return new ProfilesResponse
            {
                Total = Program.Runtime.Setup.Profiles.Count,
                Results = !String.IsNullOrEmpty(request.Name)
                    ? Program.Runtime.Setup.Profiles.Where(p => p.Name.ToLower() == request.Name.ToLower()).Select(p => p).ToList()
                    : Program.Runtime.Setup.Profiles
            };
        }



        public object Get(Setup request)
        {
            return new SetupResponse
            {
                Setup = Program.Runtime.Setup
            };
        }

        public object Post(Setup request)
        {
            if (request.UpdatedSetup != null)
            {
                // TODO: replace Program.Runtime.Setup with request.Setup
            }
            return new SetupResponse
            {
                Setup = Program.Runtime.Setup
            };
        }

        public object Get(Preview request)
        {
            var lights = (from light in Program.Runtime.CurrentProfile.LightSetupPlugin.Lights
                         select new LightPreview() { Index = light.Index, Colour = light.LightColour }).ToList();
            return new PreviewResponse
            {
                Lights = lights
            };
        }
    }
}
