using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Afterglow.Core
{
    public class LightData: IEnumerable<System.Drawing.Color>
    {
        public LightData(int numberOfLights)
        {
            ColourData = new byte[numberOfLights * 3];
        }

        public LightData(LightData copyFrom)
        {
            copyFrom.CopyTo(this);
        }

        /// <summary>
        /// The timestamp in ticks
        /// </summary>
        public long Time;

        /// <summary>
        /// Byte array containing the unstructured RGB elements for the light data
        /// </summary>
        public Byte[] ColourData;

        /// <summary>
        /// Returns the number of colour (RGB) elements stored (i.e. this.ColourData.Length / 3)
        /// </summary>
        public int Length { get { return ColourData.Length / 3; } }

        /// <summary>
        /// Provides structured access to the underlying ColourData byte array as System.Drawing.Color structures.
        /// For the number of elements <see cref="Length"/>.
        /// </summary>
        /// <param name="i">0-based light index</param>
        /// <returns>A System.Drawing.Color for the provided index</returns>
        public System.Drawing.Color this[int i]
        {
            get
            {
                int lightIndex = i * 3;
                return System.Drawing.Color.FromArgb(ColourData[lightIndex], ColourData[lightIndex + 1], ColourData[lightIndex + 2]);
            }
            set
            {
                int lightIndex = i * 3;
                ColourData[lightIndex] = value.R;
                ColourData[lightIndex+1] = value.G;
                ColourData[lightIndex+2] = value.B;
            }
        }

        public void CopyTo(LightData destination)
        {
            destination.Time = Time;
            if (ColourData != null)
            {
                byte[] colourData = new byte[ColourData.Length];
                Buffer.BlockCopy(ColourData, 0, colourData, 0, ColourData.Length);
                destination.ColourData = colourData;
            }
        }

        public IEnumerator<System.Drawing.Color> GetEnumerator()
        {
            for (var i = 0; i < ColourData.Length / 3; i++)
            {
                yield return this[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
