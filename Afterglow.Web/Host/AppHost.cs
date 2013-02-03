using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Razor;
using System.Net;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;

namespace Afterglow.Web.Host
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost() : base("Afterglow Service", typeof(AppHost).Assembly) { }
        
        public override void Configure(Funq.Container container)
        {
            Plugins.Add(new RazorFormat());

            //Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] {
            //    new AfterglowCredentialsAuthProvider()
            //}));


            // The cache the host will use (in memory in this instance - but could be a distributed memory cache / disk etc...)
            container.Register<ICacheClient>(new MemoryCacheClient());

            //RequestFilters.Add((req, resp, dto) =>
            //    {
            //        if (req.OperationName != "Auth")
            //        {
            //            var sessionId = req.GetSession();
            //            if (!sessionId.IsAuthenticated)
            //            {
            //                new AuthenticateAttribute()
            //                    .Execute(req, resp, dto);
            //            }
            //        }
            //    });

        }
    }
}
