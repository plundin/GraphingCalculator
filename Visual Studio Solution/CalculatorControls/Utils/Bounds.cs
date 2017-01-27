
// Source: Bounds.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculatorControls.Utils
{
    /// <summary>
    /// Represents a min and max value
    /// </summary>
    public class Bounds
    {
        private float m_min;
        private float m_max;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="min">the min value</param>
        /// <param name="max">the max value</param>
        public Bounds(float min, float max)
        {
            m_min = min;
            m_max = max;
        }

        /// <summary>
        /// Gets the min value
        /// </summary>
        public float Min
        {
            get
            {
                return m_min;
            }
        }

        /// <summary>
        /// Gets the max value
        /// </summary>
        public float Max
        {
            get
            {
                return m_max;
            }
        }

        /// <summary>
        /// Gets the range between the max and min value
        /// </summary>
        public float Range
        {
            get
            {
                // return Math.Abs(Max) - Math.Abs(Min);
                return (float)Math.Sqrt(Math.Pow(Max - Min, 2));
            }
        }

        /// <summary>
        /// Checks if the Bounds object is within the specified Bounds object
        /// </summary>
        /// <param name="b">The Bounds object to check against</param>
        /// <returns>true if within, false otherwise</returns>
        public bool IsWithin(Bounds b)
        {
            return (m_min >= b.Min && m_max <= b.Max );
        }
    }
}
