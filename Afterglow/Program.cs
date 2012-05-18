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

        private static IEnumerable<Light> CreateLEDs()
        {
            Rectangle _dispBounds = new Rectangle(0, 0, System.Windows.Forms.Screen.AllScreens[0].Bounds.Width, System.Windows.Forms.Screen.AllScreens[0].Bounds.Height);
            int[,] leds = {
              // Bottom Left
              {0,7,9}, {0,6,9}, {0,5,9}, {0,4,9}, {0,3,9}, {0,2,9}, {0,1,9},
              // Bottom Left Corner
              {0,0,9}, 
              // Left
              {0,0,8}, {0,0,7}, {0,0,6}, {0,0,5}, {0,0,4}, {0,0,3}, {0,0,2}, {0,0,1},
              // Top Left Corner
              {0,0,0},
              // Top
              {0,1,0}, {0,2,0}, {0,3,0}, {0,4,0}, {0,5,0}, {0,6,0}, {0,7,0}, {0,8,0}, 
              {0,9,0}, {0,10,0}, {0,11,0}, {0,12,0}, {0,13,0}, {0,14,0}, {0,15,0}, {0,16,0}, 
              // Top Right Corner
              {0,17,0},
              // Right
              {0,17,1}, {0,17,2}, {0,17,3}, {0,17,4}, {0,17,5}, {0,17,6}, {0,17,7}, {0,17,8}, 
              // Bottom Right Corner
              {0,17,9}, 
              // Bottom Right
              {0,16,9}, {0,15,9}, {0,14,9}, {0,13,9}, {0,12,9}, {0,11,9}, {0,10,9}
            };

            int LEDsHigh = 10;
            int LEDsWide = 18;

            int height = (int)Math.Floor((double)_dispBounds.Height / (double)LEDsHigh);
            int width = (int)Math.Floor((double)_dispBounds.Width / (double)LEDsWide);
            for (var ledPos = 0; ledPos < leds.GetLength(0); ledPos++)
            {
                Light led = new Light()
                {
                    Index = ledPos,
                    Region = new Rectangle(leds[ledPos, 1] * width, leds[ledPos, 2] * height, width, height)
                };

                yield return led;
            }
        }
    }
}
