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
                Console.WriteLine("Afterglow runtime loaded.");
                //Console.WriteLine("Starting Afterglow runtime...");
                //Console.WriteLine("Afterglow runtime started.");

                string host = String.Format("http://localhost:{0}/", _runtime.Setup.Port);
                if (args.Length > 0)
                    host = String.Format("http://{0}:{1}/", args[0], _runtime.Setup.Port);
                using (WebApp.Start<AppHost>(url: host))
                {
                    //HttpConfiguration.EnsureInitialized();
                    AfterglowRuntime.Logger.Info("Afterglow running on host {0}", host);

                    Console.WriteLine("Press <enter> to exit.");
                    Console.ReadLine();
                }
            }
            finally
            {
                AfterglowRuntime.Logger.Info("Exit Application");
            }
        }
    }
}
