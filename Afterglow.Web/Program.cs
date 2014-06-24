using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core;
using System.Net;
using Afterglow.Web.Host;
using Microsoft.Owin.Hosting;

namespace Afterglow.Web
{
    class Program
    {
        public static AfterglowRuntime Runtime
        {
            get
            {
                return _runtime;
            }
        }
        private static AfterglowRuntime _runtime;
                
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Loading Afterglow settings...");
                _runtime = new AfterglowRuntime();

                if (_runtime == null || _runtime.Setup == null)
                {
                    AfterglowRuntime.Logger.Fatal("Afterglow settings could not be loaded.");
                    return;
                }
                Console.WriteLine("Afterglow runtime loaded.");

                Console.WriteLine("Starting Afterglow site...");
                
                StartOptions startOptions = new StartOptions();
                startOptions.Urls.Add(string.Format("http://localhost:{0}", _runtime.Setup.Port));
                startOptions.Urls.Add(string.Format("http://{0}:{1}", Environment.MachineName.ToLower(), _runtime.Setup.Port));
                foreach (IPAddress addr in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        startOptions.Urls.Add(string.Format("http://{0}:{1}", addr, _runtime.Setup.Port));
                    }
                }

                using (WebApp.Start<AppHost>(startOptions))
                {
                    foreach (string url in startOptions.Urls)
                    {
                        Console.WriteLine("Afterglow running on host {0}", url);
                    }

                    Console.WriteLine("Press <enter> to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                AfterglowRuntime.Logger.Fatal(ex, "Application Error");
            }
            finally
            {
                AfterglowRuntime.Logger.Info("Exit Application");
            }
        }
    }
}
