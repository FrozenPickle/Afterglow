using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core.Configuration;
using System.Timers;

namespace Afterglow.Plugins.Capture
{
    public class CopyScreenCapture : BasePlugin, ICapturePlugin
    {
        
        Graphics _graphics;
        private FastBitmap _fastBitmap;
        Bitmap _img;
        private Rectangle _dispBounds;
        private Timer _captureCorrectionTimer;
        private bool _captureCorrectionTimeElapsed = false;

        public CopyScreenCapture()
        {
        }

        public CopyScreenCapture(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        #region Read Only Properties
        
        public override string Name
        {
            get { return ".Net CopyScreen"; }
        }

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
            get { return new Version(1, 0, 1); }
        }
        #endregion

        #region Screen Selection
        [ConfigLookup(DisplayName = "Screen", RetrieveValuesFrom = "Screens", SortIndex = 100)]
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
        #endregion

        #region Capture Correction

        public enum CaptureCorrectionEnum
        {
            None,
            RemoveLetterbox
        }

        [ConfigLookup(DisplayName = "Capture Correction", SortIndex = 200)]
        public CaptureCorrectionEnum CaptureCorrection
        {
            get { return Get(() => CaptureCorrection, () => CaptureCorrectionEnum.None); }
            set { Set(() => CaptureCorrection, value); }
        }

        public enum CaptureCorrectionIntervalTypeEnum
        {
            Seconds,
            Minutes
        }

        [ConfigLookup(DisplayName = "Capture Correction Interval Type", SortIndex = 300)]
        public CaptureCorrectionIntervalTypeEnum CaptureCorrectionIntervalType
        {
            get { return Get(() => CaptureCorrectionIntervalType, () => CaptureCorrectionIntervalTypeEnum.Minutes); }
            set { Set(() => CaptureCorrectionIntervalType, value); }
        }

        [ConfigNumber(DisplayName = "Capture Correction Interval", Min = 0, Max = 10000)]
        public int? CaptureCorrectionInterval
        {
            get { return Get(() => CaptureCorrectionInterval, () => 1); }
            set { Set(() => CaptureCorrectionInterval, value); }
        }

        [ConfigNumber(DisplayName = "Black Segment Number of Pixels to skip", Min = 0, Max = 999,
            Description = "Increasing this value speeds up the process but decreases the accuracy of the colour.", SortIndex= 300)]
        public int? PixelSkip
        {
            get { return Get(() => PixelSkip, () => 3); }
            set { Set(() => PixelSkip, value); }
        }

        [ConfigNumber(DisplayName = "Black Segment Height %", Min = 0, Max = 100)]
        public int? BlackSegmentHeight
        {
            get { return Get(() => BlackSegmentHeight, () => 100); }
            set { Set(() => BlackSegmentHeight, value); }
        }
        #endregion
        




        int _captureHeight = 0;
        int _captureWidth = 0;
        int _leftOffset = 0;
        int _topOffset = 0;
        public IDictionary<Core.Light, Core.PixelReader> Capture(ILightSetupPlugin lightSetup)
        {
            _graphics.CopyFromScreen(_dispBounds.Left, _dispBounds.Top, 0, 0, new Size(_dispBounds.Width, _dispBounds.Height));

            _fastBitmap = new FastBitmap(_img);

            _captureHeight = _dispBounds.Height;
            _captureWidth = _dispBounds.Width;

            if (this._captureCorrectionTimeElapsed && this.BlackSegmentHeight != null && this.BlackSegmentHeight.Value != 100)
            {
                _topOffset = GetTopBandingHeight();
                if (_topOffset != 0 && (_captureHeight - (_topOffset * 2)) > 0)
                {
                    _captureHeight = _dispBounds.Height - (_topOffset * 2);
                }
                else
                {
                    _topOffset = 0;
                }
            }
            
            IDictionary<Core.Light, Core.PixelReader> dictionary = new Dictionary<Core.Light, Core.PixelReader>();
            foreach (Light light in lightSetup.GetLightsForBounds(_captureWidth, _captureHeight, _leftOffset, _topOffset))
            {
                dictionary[light] = new PixelReader(_fastBitmap, light.Region);
            }

            return dictionary;
        }

        private int GetTopBandingHeight()
        {
            double segmentHeightPercent = this.BlackSegmentHeight.Value / 100.00;
            int blackRange = 5;

            int topHeight = _dispBounds.Height / 2;
            int segmentHeightPixels = Convert.ToInt32(_dispBounds.Height * segmentHeightPercent);
            if (segmentHeightPixels <= 0)
            {
                segmentHeightPixels = 1;
            }
            //TODO log segmentHeightPixels

            int blackSize = 0;

            //iterate long segments
            for (int i = 0; i < topHeight; i += segmentHeightPixels)
            {
                //create region
                Rectangle region = new Rectangle(0, i, _dispBounds.Width, segmentHeightPixels);

                PixelReader pixelReader = new PixelReader(_fastBitmap, region);

                // Average the pixels
                int r = 0, g = 0, b = 0, pixelCount = 0;
                foreach (var pixel in pixelReader.GetEveryNthPixel(this.PixelSkip.Value))
                {
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    pixelCount++;
                }

                int redAvg = r / pixelCount;

                int greenAvg = g / pixelCount;

                int blueAvg = b / pixelCount;

                if (redAvg <= blackRange && greenAvg <= blackRange && blueAvg <= blackRange)
                {
                    blackSize = i + segmentHeightPixels;
                }               

            }


            return blackSize;
        }

        /// <summary>
        /// The FastBitmap created to wrap the capture during the capture is disposed of here
        /// </summary>
        public void ReleaseCapture()
        {
            _fastBitmap.Dispose();
            _fastBitmap = null;
        }

        public override void Start()
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[this.Screens.ToList().IndexOf(Screen)];

            _dispBounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);

            _img = new Bitmap(_dispBounds.Width, _dispBounds.Height);
            _graphics = Graphics.FromImage(_img);

            if (this.CaptureCorrection != CaptureCorrectionEnum.None)
            {
                int minutes = (this.CaptureCorrectionIntervalType == CaptureCorrectionIntervalTypeEnum.Minutes ? this.CaptureCorrectionInterval.Value : 0);
                int seconds = (this.CaptureCorrectionIntervalType == CaptureCorrectionIntervalTypeEnum.Seconds ? this.CaptureCorrectionInterval.Value : 0);
                _captureCorrectionTimer = new Timer(new TimeSpan(0, minutes, seconds).TotalMilliseconds);
                _captureCorrectionTimer.Elapsed += delegate(object sender, ElapsedEventArgs e)
                {
                    _captureCorrectionTimeElapsed = true;
                };
                _captureCorrectionTimer.Start();
            }
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
