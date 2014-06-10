using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using System.Web.Http;

namespace Afterglow.Web.Host
{


    public class AppHost
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {

            appBuilder.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\")
            });

            appBuilder.UseStaticFiles("/Views");
            appBuilder.UseStaticFiles("/Scripts");
            appBuilder.UseStaticFiles("/js");
            appBuilder.UseStaticFiles("/Content");

            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            //  Enable attribute based routing
            //  http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
            config.MapHttpAttributeRoutes();

            // Remove the XML formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Routes.MapHttpRoute(
                name: "AfterglowAPI",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        } 
    }
}
