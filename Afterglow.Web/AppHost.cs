using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Razor;
using System.Net;

namespace Afterglow.Web
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost() : base("Afterglow Service", typeof(AppHost).Assembly) { }
        
        public override void Configure(Funq.Container container)
        {
            //Plugins.Add(new RazorFormat());

            Routes
                .Add<Afterglow.Core.Profile>("/profile")
                .Add<Afterglow.Core.Profile>("/profile/{Name}");

            SetConfig(new EndpointHostConfig
            {
                CustomHttpHandlers = {
                    { HttpStatusCode.NotFound, new RazorHandler("/notfound") }
                }
            });
        }
    }
}
