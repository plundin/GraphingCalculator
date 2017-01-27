
// Source: Graph.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CalculatorControls.Utils
{
    /// <summary>
    /// Represents a graph made up of a number of points
    /// </summary>
    [Serializable]
    public class Graph
    {
        // The id of the graph
        private Guid m_id;
        // The label of the graph
        private string m_label;
        // The color to use when drawing this graph
        private Color m_drawingColor;
        // The pen width to sue when drawing this graph
        private float m_penWidth;
        // Holds the points for the graph
        public List<PointF> m_points;

        /// <summary>
        /// Creates a Graph using the specified points
        /// </summary>
        /// <param name="points">All points that the Graph is made up from</param>
        public Graph(PointF[] points)
            : this(Guid.NewGuid(), "", points)
        {

        }

        /// <summary>
        /// Creates a Graph using the specified label and points
        /// </summary>
        /// <param name="label">Label for this Graph</param>
        /// <param name="points">All points that the Graph is made up from</param>
        public Graph(string label, PointF[] points)
            : this(Guid.NewGuid(), label, points)
        {

        }

        /// <summary>
        /// Creates a Graph using the specified id, label and points
        /// </summary>
        /// <param name="id">Unique id for this Graph</param>
        /// <param name="label">Label for this Graph</param>
        /// <param name="points">All points that the Graph is made up from</param>
        public Graph(Guid id, string label, PointF[] points)
        {
            // Set values
            m_id = id;
            m_label = label;

            // Init color and width to use when drawing this graph
            m_penWidth = 1;
            m_drawingColor = Utils.RandomColor(Color.Gray);

            // Create points list
            m_points = new List<PointF>();

            // Add the array of points to list
            m_points.AddRange(points);
        }

        /// <summary>
        /// Gets the unique id for this graph
        /// </summary>
        public Guid ID
        {
            get
            {
                return m_id;
            }
        }

        /// <summary>
        /// Gets or sets the label for this graph
        /// </summary>
        public string Label
        {
            get
            {
                return m_label;
            }
            set
            {
                m_label = value;
            }
        }

        /// <summary>
        /// Gets an array of points that represent this graph
        /// </summary>
        public PointF[] Points
        {
            get
            {
                return m_points.ToArray<PointF>();
            }
        }

        /// <summary>
        /// Gets or sets the Color to use when drawing the graph
        /// </summary>
        public Color DrawingColor
        {
            get
            {
                return m_drawingColor;
            }
            set
            {
                if (value != null) m_drawingColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the pen width to use when drawing the graph
        /// </summary>
        public float DrawingPenWidth
        {
            get
            {
                return m_penWidth;
            }
            set
            {
                m_penWidth = value;
            }
        }
    }
}
