using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Afterglow.Core;
using Afterglow.Log;
using Afterglow.Plugins.Capture;
using Afterglow.Plugins.ColourExtraction;
using Afterglow.Core.Plugins;
using Afterglow.Plugins.Output;
using System.Drawing;
using Afterglow.Storage;
using Nini.Config;
using System.IO;

namespace Afterglow
{
    static class Program
    {

        private static AfterglowRuntime _runtime;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Afterglow.Log.Log4NetProxy logger = new Log4NetProxy(log4net.LogManager.GetLogger("LoggingSystem"));

            Afterglow.Storage.NiniDatabase storage = new NiniDatabase("Afterglow.ini");
            _runtime = new AfterglowRuntime(storage, logger, new Loader.PluginLoader());

            Application.Run(new Forms.MainForm(_runtime));
        }
    }
}
