using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Core.Configuration;
using System.Timers;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Afterglow.Plugins.Capture
{
    [DataContract]
    public class CopyScreenCapture : BasePlugin, ICapturePlugin
    {

        private Graphics _graphics;
        private FastBitmap _fastBitmap;
        private Bitmap _img;
        private Rectangle _dispBounds;
        private Timer _captureCorrectionTimer;
        private bool _captureCorrectionTimeElapsed = false;

        public CopyScreenCapture()
        {
            SetupCaptureCorrectionTypes();
        }

        #region Read Only Properties

        [DataMember]
        public override string Name
        {
            get { return ".Net CopyScreen"; }
        }

        [DataMember]
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        [DataMember]
        public override string Description
        {
            get { return "A plugin from the Afterglow default plugins"; }
        }

        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 1); }
        }
        #endregion

        #region Screen Selection Properties
        [DataMember]
        [Required]
        [Display(Name = "Screen", Order = 100)]
        [ConfigLookup(RetrieveValuesFrom = "Screens")]
        public string Screen
        {
            get { return Get(() => Screen, () => Screens[0]); }
            set { Set(() => Screen, value); }
        }

        [DataMember]
        public string[] Screens
        {
            get
            {
                var screens = new List<String>();
                for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                    screens.Add("Screen " + (i + 1).ToString());
                return screens.ToArray();
            }
        }
        #endregion

        #region Capture Correction Properties

        #region Capture Correction Types
        [XmlIgnore]
        public ObservableCollection<string> CaptureCorrectionTypes;
        public const int CAPTURE_CORRECTION_TYPE_NONE = 0;
        public const int CAPTURE_CORRECTION_TYPE_TOP_BOTTOM = 1;
        public const int CAPTURE_CORRECTION_TYPE_LEFT_RIGHT = 2;
        public const int CAPTURE_CORRECTION_TYPE_ALL = 3;

        public void SetupCaptureCorrectionTypes()
        {
            CaptureCorrectionTypes = new ObservableCollection<string>();
            CaptureCorrectionTypes.Add("None");
            CaptureCorrectionTypes.Add("Remove Top and Bottom Black");
            CaptureCorrectionTypes.Add("Remove Left and Right Black");
            CaptureCorrectionTypes.Add("Remove All Black");
        }
        #endregion

        [DataMember]
        [Required]
        [Display(Name = "Screen", Order = 100)]
        [ConfigLookup(RetrieveValuesFrom = "CaptureCorrectionTypes")]
        public int CaptureCorrection
        {
            get { return Get(() => CaptureCorrection, () => CAPTURE_CORRECTION_TYPE_ALL); }
            set { Set(() => CaptureCorrection, value); }
        }

        public enum CaptureCorrectionIntervalTypeEnum
        {
            Seconds,
            Minutes
        }

        [DataMember]
        [Required]
        [Display(Name = "Capture Correction Interval Type", Order = 200)]
        public CaptureCorrectionIntervalTypeEnum CaptureCorrectionIntervalType
        {
            get { return Get(() => CaptureCorrectionIntervalType, () => CaptureCorrectionIntervalTypeEnum.Minutes); }
            set { Set(() => CaptureCorrectionIntervalType, value); }
        }

        [DataMember]
        [Display(Name = "Capture Correction Interval", Order = 300)]
        [Range(0, 10000)]
        public int? CaptureCorrectionInterval
        {
            get { return Get(() => CaptureCorrectionInterval, () => 1); }
            set { Set(() => CaptureCorrectionInterval, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Black Segment - Pixels to skip", Description = "Increasing this value speeds up the process but decreases the accuracy of the black.", Order = 400)]
        [Range(0, 999)]
        public int PixelSkip
        {
            get { return Get(() => PixelSkip, () => 20); }
            set { Set(() => PixelSkip, value); }
        }

        [DataMember]
        [Display(Name = "Black Segment - Height %", Order = 500)]
        [Range(0, 100)]
        public int? BlackSegmentHeight
        {
            get { return Get(() => BlackSegmentHeight, () => 1); }
            set { Set(() => BlackSegmentHeight, value); }
        }

        [DataMember]
        [Display(Name = "Darkness Threshold", Order = 600)]
        [Range(0, 50)]
        public int? DarknessThreshold
        {
            get { return Get(() => DarknessThreshold, () => 5); }
            set { Set(() => DarknessThreshold, value); }
        }
        #endregion

        public override void Start()
        {
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[this.Screens.ToList().IndexOf(Screen)];

            _dispBounds = new Rectangle(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);

            _img = new Bitmap(_dispBounds.Width, _dispBounds.Height);
            _graphics = Graphics.FromImage(_img);
            _captureHeight = _dispBounds.Height;
            _captureWidth = _dispBounds.Width;

            if (this.CaptureCorrection != CAPTURE_CORRECTION_TYPE_NONE)
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

        int _captureHeight = 0;
        int _captureWidth = 0;
        int _leftOffset = 0;
        int _topOffset = 0;
        public IDictionary<Core.Light, Core.PixelReader> Capture(ILightSetupPlugin lightSetup)
        {
            _graphics.CopyFromScreen(_dispBounds.Left, _dispBounds.Top, 0, 0, new Size(_dispBounds.Width, _dispBounds.Height));

            _fastBitmap = new FastBitmap(_img);


            GetCaptureSize();

            IDictionary<Core.Light, Core.PixelReader> dictionary = new Dictionary<Core.Light, Core.PixelReader>();
            foreach (Light light in lightSetup.GetLightsForBounds(_captureWidth, _captureHeight, _leftOffset, _topOffset))
            {
                dictionary[light] = new PixelReader(_fastBitmap, light.Region);
            }

            return dictionary;
        }

        private void GetCaptureSize()
        {
            if (this.CaptureCorrection != CAPTURE_CORRECTION_TYPE_NONE &&
                this._captureCorrectionTimeElapsed &&
                this.BlackSegmentHeight != null &&
                this.BlackSegmentHeight.Value != 100 &&
                this.DarknessThreshold != null &&
                this.DarknessThreshold.Value <= 50)
            {
                this._captureCorrectionTimeElapsed = false;

                _captureHeight = _dispBounds.Height;
                _captureWidth = _dispBounds.Width;
                _leftOffset = 0;
                _topOffset = 0;
                int topOffset = 0;
                int leftOffset = 0;

                switch (this.CaptureCorrection)
                {
                    case CAPTURE_CORRECTION_TYPE_TOP_BOTTOM:
                        topOffset = GetTopBandingHeight();
                        break;

                    case CAPTURE_CORRECTION_TYPE_LEFT_RIGHT:
                        leftOffset = GetLeftBandingWidth();
                        break;

                    case CAPTURE_CORRECTION_TYPE_ALL:

                        topOffset = GetTopBandingHeight();
                        leftOffset = GetLeftBandingWidth();
                        break;

                    default:
                        break;
                }
                if (topOffset != 0 && (_captureHeight - (topOffset * 2)) > 0)
                {
                    _captureHeight = _dispBounds.Height - (topOffset * 2);
                    _topOffset = topOffset;
                }
                if (leftOffset != 0 && (_captureWidth - (leftOffset * 2)) > 0)
                {
                    _captureWidth = _dispBounds.Width - (leftOffset * 2);
                    _leftOffset = leftOffset;
                }


            }
            //TODO log info if BlackSegmentHeight or DarknessThreshold values are invalid
        }

        private int GetTopBandingHeight()
        {
            double segmentHeightPercent = this.BlackSegmentHeight.Value / 100.00;

            int topHeight = _captureHeight / 2;
            int segmentHeightPixels = Convert.ToInt32(_captureHeight * segmentHeightPercent);
            if (segmentHeightPixels <= 0)
            {
                segmentHeightPixels = 1;
            }
            //TODO log info segmentHeightPixels

            int blackHeight = 0;

            //iterate long segments
            for (int i = 0; i < topHeight; i += segmentHeightPixels)
            {
                //create region
                Rectangle region = new Rectangle(0, i, _captureWidth, segmentHeightPixels);

                PixelReader pixelReader = new PixelReader(_fastBitmap, region);

                // Average the pixels
                int r = 0, g = 0, b = 0, pixelCount = 0;
                foreach (var pixel in pixelReader.GetEveryNthPixel(this.PixelSkip))
                {
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    pixelCount++;
                }

                int redAvg = r / pixelCount;

                int greenAvg = g / pixelCount;

                int blueAvg = b / pixelCount;

                if (redAvg <= this.DarknessThreshold && greenAvg <= this.DarknessThreshold && blueAvg <= this.DarknessThreshold)
                {
                    blackHeight = i + segmentHeightPixels;
                }
            }

            return blackHeight;
        }

        private int GetLeftBandingWidth()
        {
            double segmentWidthPercent = this.BlackSegmentHeight.Value / 100.00;

            int leftHalfWidth = _captureWidth / 2;
            int segmentWidthPixels = Convert.ToInt32(_captureWidth * segmentWidthPercent);
            if (segmentWidthPixels <= 0)
            {
                segmentWidthPixels = 1;
            }
            //TODO log info segmentWidthPixels

            int blackHeight = 0;

            //iterate long segments
            for (int i = 0; i < leftHalfWidth; i += segmentWidthPixels)
            {
                //create region
                Rectangle region = new Rectangle(i, _topOffset, segmentWidthPixels, _captureHeight);

                PixelReader pixelReader = new PixelReader(_fastBitmap, region);

                // Average the pixels
                int r = 0, g = 0, b = 0, pixelCount = 0;
                foreach (var pixel in pixelReader.GetEveryNthPixel(this.PixelSkip))
                {
                    r += pixel.R;
                    g += pixel.G;
                    b += pixel.B;
                    pixelCount++;
                }

                int redAvg = r / pixelCount;

                int greenAvg = g / pixelCount;

                int blueAvg = b / pixelCount;

                if (redAvg <= this.DarknessThreshold && greenAvg <= this.DarknessThreshold && blueAvg <= this.DarknessThreshold)
                {
                    blackHeight = i + segmentWidthPixels;
                }

            }

            return blackHeight;
        }

        /// <summary>
        /// The FastBitmap created to wrap the capture during the capture is disposed of here
        /// </summary>
        public void ReleaseCapture()
        {
            _fastBitmap.Dispose();
            _fastBitmap = null;
        }
    }
}
