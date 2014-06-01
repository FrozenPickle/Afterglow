using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core;
using System.Net;
using ServiceStack.Logging;
using Afterglow.Web.Host;

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
            using (var appHost = new AppHost())
            {
                Console.WriteLine("Loading Afterglow settings...");
                _runtime = new AfterglowRuntime();
            
                Console.WriteLine("Starting Afterglow runtime...");

                Console.WriteLine("Afterglow runtime started.");

                appHost.Init();
                string host = String.Format("http://localhost:{0}/", _runtime.Setup.Port);
                if (args.Length > 0)
                    host = String.Format("http://{0}:{1}/", args[0], _runtime.Setup.Port);
                
                appHost.Start(host);

                Console.WriteLine(host);
                
                Console.WriteLine("Press <enter> to exit.");
                Console.ReadLine();
                appHost.Stop();
            }
        }
    }
}
