using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Afterglow.Core.Configuration;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Afterglow.Core
{
    /// <summary>
    /// Describes the Position of a light
    /// </summary>
    public class Light : BaseModel
    {
        /// <summary>
        /// Index is order lights are processed
        /// </summary>
        [Required]
        [Display(Name="Index")]
        [Range(0,999)]
        public int Index 
        {
            get { return Get(() => this.Index); }
            set { Set(() => this.Index, value); }
        }

        /// <summary>
        /// How many positions from the left
        /// </summary>
        [Required]
        [Display(Name = "Left")]
        [Range(0, 999)]
        public int Left
        {
            get { return Get(() => this.Left); }
            set { Set(() => this.Left, value); }
        }

        /// <summary>
        /// How many positions from the top
        /// </summary>
        [Required]
        [Display(Name = "Top")]
        [Range(0, 999)]
        public int Top
        {
            get { return Get(() => this.Top); }
            set { Set(() => this.Top, value); }
        }

        /// <summary>
        /// How many positions wide
        /// </summary>
        [Required]
        [Display(Name = "Width")]
        [Range(1, 999)]
        public int Width
        {
            get { return Get(() => this.Width, () => 1); }
            set { Set(() => this.Width, value); }
        }

        /// <summary>
        /// How many positions high
        /// </summary>
        [Required]
        [Display(Name = "Height")]
        [Range(1, 999)]
        public int Height
        {
            get { return Get(() => this.Height, () => 1); }
            set { Set(() => this.Height, value); }
        }
        /// <summary>
        /// The previous pre processed colour of the light
        /// </summary>
        [XmlIgnore]
        public Color OldSourceColour { get; set; }
        /// <summary>
        /// The current pre processed colour of the light
        /// </summary>
        [XmlIgnore]
        public Color SourceColour { get; set; }
        /// <summary>
        /// The previous processed colour of the light
        /// </summary>
        [XmlIgnore]
        public Color OldLightColour { get; set; }
        /// <summary>
        /// The current processed colour of the light
        /// </summary>
        [XmlIgnore]
        public Color LightColour { get; set; }

        /// <summary>
        /// The region used to capture the screen that this light will correspond to
        /// </summary>
        [XmlIgnore]
        public Rectangle Region { get; set; }

        /// <summary>
        /// Populates the Region property
        /// </summary>
        /// <param name="SegmentWidth">The screen capture segment width that this light will capture from</param>
        /// <param name="SegmentHight">The screen capture segment height that this light will capture from</param>
        /// <param name="LeftOffset">How many positions from the left of the screen capture segment will capture from</param>
        /// <param name="TopOffset">How many positions from the top of the screen capture segment will capture from</param>
        public void CalculateRegion(int SegmentWidth, int SegmentHight, int LeftOffset = 0, int TopOffset = 0)
        {
            int left = 0;
            int top = 0;
            int width = 0;
            int height = 0;

            left = this.Left * SegmentWidth + LeftOffset;
            top = this.Top * SegmentHight + TopOffset;
            width = this.Width * SegmentWidth;
            height = this.Height * SegmentHight;

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
