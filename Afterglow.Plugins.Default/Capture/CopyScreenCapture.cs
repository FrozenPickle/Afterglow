using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core.Configuration;

namespace Afterglow.Plugins.Capture
{
    public class CopyScreenCapture : BasePlugin, ICapturePlugin
    {
        
        Graphics _graphics;
        private FastBitmap _fastBitmap;
        Bitmap _img;
        private Rectangle _dispBounds;

        public CopyScreenCapture()
        {
        }

        public CopyScreenCapture(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        #region Read Only Properties

        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        public override string Description
        {
            get { return "A plugin from the Afterglow default plugins"; }
        }

        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion

        [ConfigLookup(DisplayName = "Screen", RetrieveValuesFrom = "Screens")]
        public string Screen
        {
            get { return Get(() => Screen, () => Screens[0]); }
            set { Set(() => Screen, value); }
        }

        public string[] Screens
        {
            get 
            { 
                var screens = new List<String>();
                for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                  screens.Add("Screen " + i.ToString());
                return screens.ToArray();
            }
        }

        public IDictionary<Core.Light, Core.PixelReader> Capture(ILightSetupPlugin lightSetup)
        {
            _graphics.CopyFromScreen(_dispBounds.Left, _dispBounds.Top, 0, 0, new Size(_dispBounds.Width, _dispBounds.Height));

            _fastBitmap = new FastBitmap(_img);


            //TODO every n th frame check for Black banding as config as sub plugin


            IDictionary<Core.Light, Core.PixelReader> dictionary = new Dictionary<Core.Light, Core.PixelReader>();
            foreach (Light light in lightSetup.GetLightsForBounds(_dispBounds.Width, _dispBounds.Height))
            {
                dictionary[light] = new PixelReader(_fastBitmap, light.Region);
            }

            return dictionary;
        }

        /// <summary>
        /// The FastBitmap created to wrap the capture during the capture is disposed of here
        /// </summary>
        public void ReleaseCapture()
        {
            _fastBitmap.Dispose();
            _fastBitmap = null;
        }

        public override string Name
        {
            get { return ".Net CopyScreen"; }
        }

        public override void Start()
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[this.Screens.ToList().IndexOf(Screen)];

            _dispBounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);

            _img = new Bitmap(_dispBounds.Width, _dispBounds.Height);
            _graphics = Graphics.FromImage(_img);
        }

        public override void Stop()
        {
            _img.Dispose();
            _img = null;
            _graphics.Dispose();
            _graphics = null;
        }
    }
}
