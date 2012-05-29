using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core.Storage;
using Afterglow.Core.Configuration;

namespace Afterglow.Core
{
    public class Light : BaseRecord
    {
        public Light()
        {
        }

        public Light(ITable table, Afterglow.Core.Log.ILogger logger, AfterglowRuntime runtime)
            : base(table, logger, runtime)
        {
        }

        [ConfigNumber(DisplayName = "Index", IsHidden = true, Min = 0, Max = 999)]
        public int? Index 
        {
            get { return Get(() => this.Index); }
            set { Set(() => this.Index, value); }
        }

        [ConfigNumber(DisplayName = "Left", IsHidden = true, Min = 0, Max = 999)]
        public int? Left
        {
            get { return Get(() => this.Left); }
            set { Set(() => this.Left, value); }
        }

        [ConfigNumber(DisplayName = "Top", IsHidden = true, Min = 0, Max = 999)]
        public int? Top
        {
            get { return Get(() => this.Top); }
            set { Set(() => this.Top, value); }
        }

        [ConfigNumber(DisplayName = "Width", IsHidden = true, Min = 1, Max = 999)]
        public int? Width
        {
            get { return Get(() => this.Width, () => 1); }
            set { Set(() => this.Width, value); }
        }

        [ConfigNumber(DisplayName = "Height", IsHidden = true, Min = 1, Max = 999)]
        public int? Height
        {
            get { return Get(() => this.Height, () => 1); }
            set { Set(() => this.Height, value); }
        }

        [ConfigNumber(DisplayName = "Screen Number", IsHidden = true, Min = 0, Max = 999)]
        public int? ScreenNumber
        {
            get { return Get(() => this.ScreenNumber, () => 0); }
            set { Set(() => this.ScreenNumber, value); }
        }

        public Color OldSourceColour { get; set; }
        public Color SourceColour { get; set; }
        public Color OldLEDColour { get; set; }
        public Color LEDColour { get; set; }
        public Rectangle Region { get; set; }

        public void CalculateRegion(int SegmentWidth, int SegmentHight, int LeftOffset = 0, int TopOffset = 0)
        {
            int left = 0;
            int top = 0;
            int width = 0;
            int height = 0;

            left = this.Left.Value * SegmentWidth + LeftOffset;
            top = this.Top.Value * SegmentHight + TopOffset;
            width = this.Width.Value * SegmentWidth;
            height = this.Height.Value * SegmentHight;

            //if width or height is 0 then set region to empty this will equate to black
            if (width == 0 || height == 0)
            {
                this.Region = Rectangle.Empty;
            }
            else
            {
                this.Region = new Rectangle(left, top, width, height);
            }
        }
    }
}
