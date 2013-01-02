using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;
using ServiceStack.Text;

namespace Afterglow.Web
{
    [Route("/runtime")]
    [Route("/runtime/{ToggleActive}")]
    public class Runtime
    {
        public bool ToggleActive { get; set; }
    }

    public class RuntimeResponse
    {
        public bool Active { get; set; }
        public int NumberOfLights { get; set; }
    }

    [Route("/profiles")]
    [Route("/profiles/{Id}")]
    [Route("/profiles/name/{Name}")]
    public class Profiles
    {
        public int Id { get; set; }
        public string Name { get; set; }
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
            if (request.ToggleActive)
            {
                Program.ToggleActive();
            }
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
    }
}
