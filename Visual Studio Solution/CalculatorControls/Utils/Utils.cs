
// Source: Utils.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;

namespace CalculatorControls.Utils
{
    /// <summary>
    /// Misc helper methods
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Creates a random color
        /// </summary>
        /// <returns>the color</returns>
        private Color RandomColor()
        {
            Random rand = new Random();
            return Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
        }

        /// <summary>
        /// Creates a random color and uses the specified color as a "mix"
        /// </summary>
        /// <param name="mix">color to mix with</param>
        /// <returns>the color</returns>
        /// <remarks>Adapted from suggestions at stackoverflow</remarks>
        public static Color RandomColor(Color mix)
        {
            int red = RandomInt(0,256);
            int green = RandomInt(0,256);
            int blue = RandomInt(0,256);

            if (mix != null)
            {
                red = (red + mix.R) / 2;
                green = (green + mix.G) / 2;
                blue = (blue + mix.B) / 2;
            }

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// Get random int, using RNGCryptoServiceProvider for betetr randomness
        /// </summary>
        private static int RandomInt(int min, int max)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];

            rng.GetBytes(buffer);
            int seed = BitConverter.ToInt32(buffer, 0);

            return new Random(seed).Next(min, max);
        } 
    }
}
