
// Source: CoordinateSystem.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

#region Using
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CalculatorControls.Utils;
using IOUtilities;
#endregion

namespace CalculatorControls
{
    /// <summary>
    /// Implements a CoordinateSystem and various methods to draw in the CoordinateSystem
    /// </summary>
    public partial class CoordinateSystem : UserControl
    {
        #region Backing for properties
        // Brushes
        private Brush m_foregroundBrush;
        private Brush m_backgroundBrush;
        private Font m_font;
        // Number of ticks in x and y axis
        private int m_ticksX;
        private int m_ticksY;
        // Draw value string at every number of ticks
        private int m_valueAtTick;
        // Size of margins where text and scale shows
        private Padding m_margin;
        // Width, height of tick marks
        private int m_tickWidth;
        // Bounds for the coordinate system
        private Bounds m_boundsX;
        private Bounds m_boundsY;
        // Saves mouse position when mouse moves over
        private Point m_mousePosition;
        // A dashed pen for CoordinateSystem cursor lines
        private Pen m_dashedPen;
        // A pen to use for the CoordinateSystem
        private Pen m_CoordinateSystemPen;
        // List of Graph objects to draw
        List<Graph> m_graphs;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor creating and initializing an instance of the coordinate system
        /// </summary>
        public CoordinateSystem()
        {
            InitializeComponent();

            // These flags enable double buffering for less flicker
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            // Set all default values for colors, brushes, cursors and default dimensions
            m_foregroundBrush = Brushes.Black;
            m_backgroundBrush = Brushes.White;
            m_font = DefaultFont;

            // Default bounds
            m_boundsX = new Bounds(-20, 20);
            m_boundsY = new Bounds(-20, 20);

            // Default margin in CoordinateSystem
            m_margin = new Padding(10, 10, 10, 10);

            // Default tick sizes and number of ticks
            m_tickWidth = 5;
            m_ticksX = 20;
            m_ticksY = 20;
            m_valueAtTick = 5;

            // Set cursor
            Cursor = Cursors.Cross;

            // Pens to use
            m_dashedPen = new Pen(Color.FromArgb(0xCC,0xCC,0xCC), 1);
            m_dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            m_CoordinateSystemPen = new Pen(Color.DarkGreen, 1);

            // The list for holding graphs to draw
            m_graphs = new List<Graph>();
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets and sets the bounds for the horizontal "x"-axis
        /// </summary>
        public Bounds BoundsX
        {
            get { return new Bounds(m_boundsX.Min, m_boundsX.Max); }
            set { if(value != null) m_boundsX = value; }
        }

        /// <summary>
        /// Gets and sets the bounds for the vertical "y"-axis
        /// </summary>
        public Bounds BoundsY
        {
            get { return new Bounds(m_boundsY.Min, m_boundsY.Max); }
            set { if (value != null) m_boundsY = value; }
        }

        /// <summary>
        /// Sets the number of ticks to place on the horizontal "x"-axis
        /// </summary>
        public int TicksX
        {
            get { return m_ticksX; }
            set { if(value > 0) m_ticksX = value; }
        }

        /// <summary>
        /// Sets the number of ticks to use for the vertical "y"-axis
        /// </summary>
        public int TicksY
        {
            get { return m_ticksY; }
            set { if(value > 0) m_ticksY = value; }
        }

        /// <summary>
        /// Place value string at this tick count when drawing the tick marks
        /// </summary>
        public int ValueAtTick
        {
            get { return m_valueAtTick; }
            set { if (value > 0) m_valueAtTick = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds and draws a graph in the CoordinateSystem linked with lines 
        /// </summary>
        /// <param name="points">The points that makes up the graph to draw</param>
        /// <param name="label">a label to use for the graph</param>
        /// <returns>An identifier for the graph</returns>
        public Guid AddGraph(string label, PointF[] points)
        {
            // Create a Graph to hold the points
            Graph graph = new Graph(label, points);

            // Add to list
            m_graphs.Add(graph);

            // Redraw coordinate system
            Invalidate();

            // Return the id for the Graph
            return graph.ID;
        }

        /// <summary>
        /// Deletes the graph with the specified id
        /// </summary>
        /// <param name="id">id of graph to remove</param>
        /// <returns>True if the graph was removed, false otherwhise</returns>
        public bool DeleteGraph(Guid id)
        {
            // Find the Graph
            Graph graph = m_graphs.Find((g) => g.ID == id);
            if (graph == null) return false;

            // Remove graph
            bool bRemoved = m_graphs.Remove(graph);

            // Redraw coordinate system
            Invalidate();

            return bRemoved;
        }

        /// <summary>
        /// Deletes all graphs in the CoordinateSystem
        /// </summary>
        public void DeleteAll()
        {
            m_graphs.Clear();
            Invalidate();
        }

        /// <summary>
        /// Saves all graphs in the coordinate system to the specified file as a binary serialized file
        /// </summary>
        /// <param name="filename">filename to save to</param>
        public void Save(string filename)
        {
            IOUtilities.BinSerialization.Serialize<Graph[]>(m_graphs.ToArray(), filename);
        }

        /// <summary>
        /// Loads all graphs from the specified file to the coordinatesystem
        /// </summary>
        /// <param name="filename">filename to save to</param>
        public void Load(string filename)
        {
            Graph[] graphs = IOUtilities.BinSerialization.Deserialize<Graph[]>(filename);
            if (graphs != null)
            {
                m_graphs.AddRange(graphs);
            }

            // Redraw
            Invalidate();
        }

        /// <summary>
        /// Draws the coordinate system to the graphics object, 
        /// this is to support printing.
        /// </summary>
        /// <param name="g">graphics object to print to</param>
        public void DrawToGraphics(Graphics g)
        {
            Bitmap bitmap = new Bitmap(Width, Height);

            // Draw this control to the bitmap
            DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            g.PageUnit = GraphicsUnit.Display;
            g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }

        #endregion

        #region Private Drawing Methods

        /// <summary>
        /// Draws the axis
        /// </summary>
        /// <param name="g">Graphics object to use for drawing</param>
        private void DrawAxis(Graphics g)
        {
            // Offset for origo placement
            float offXorigo = -m_boundsX.Min / m_boundsX.Range;
            float offYorigo = m_boundsY.Max / m_boundsY.Range;

            // Draw the horizontal "x"-axis
            g.DrawLine(
                Pens.Blue,
                new PointF(m_margin.Left, m_margin.Top + (Height - m_margin.Top - m_margin.Bottom) * offYorigo),
                new PointF(Width - m_margin.Right, m_margin.Top + (Height - m_margin.Top - m_margin.Bottom) * offYorigo)
            );
      
            // Draw the vertical "y"-axis
            g.DrawLine(
                Pens.Blue,
                new PointF(m_margin.Left + (Width - m_margin.Left - m_margin.Right) * offXorigo, Height - m_margin.Bottom),
                new PointF(m_margin.Left + (Width - m_margin.Left - m_margin.Right) * offXorigo, m_margin.Top)
            );  
        }

        /// <summary>
        /// Draws tick marks on the axis
        /// </summary>
        /// <param name="g">Graphics object to use for drawing</param>
        private void DrawTicks(Graphics g)
        {
            DrawXTicks(g);
            DrawYTicks(g);
        }

        /// <summary>
        /// Draws the tick marks along the "y"-axis
        /// </summary>
        /// <param name="g">graphics object</param>
        private void DrawYTicks(Graphics g)
        {
            SizeF textSize;

            // Offset for origo placement
            float offXorigo = -m_boundsX.Min / m_boundsX.Range;
            float offYorigo = m_boundsY.Max / m_boundsY.Range;

            // Calculate value and offset between each tick mark for y axis
            float tickValue = m_boundsY.Range / (float)m_ticksY;
            float tickOffset = (Height - m_margin.Bottom - m_margin.Top) / (float)m_ticksY;

            // Start for first tick mark
            float drawY = m_margin.Top;
            float valueY = 0;

            // Offset width for y axis
            float offsetw = m_margin.Left + (Width - m_margin.Left - m_margin.Right) * offXorigo;

            // Draw tick marks vertically
            for (int i = 0; i <= m_ticksY; i++)
            {
                // Check if we should draw the value string at this mark
                bool bDrawValue = (i % ValueAtTick == 0);

                // Draw the tick and the text value
                g.DrawLine(
                    Pens.Blue,
                    new PointF(offsetw - m_tickWidth / (bDrawValue ? 1 : 2), drawY),
                    new PointF(offsetw + m_tickWidth / (bDrawValue ? 1 : 2), drawY)
                );

                // Value to draw
                float value = (m_boundsY.Max - valueY);

                // Only draw at certain tick marks and avoid origo
                if (bDrawValue && value != 0)
                {
                    // Create and measure value string
                    string valueText = value.ToString("0.##");
                    textSize = g.MeasureString(valueText, m_font);

                    // Draw the value string
                    g.DrawString(valueText, m_font, m_foregroundBrush, new PointF(offsetw - m_tickWidth - textSize.Width, drawY - textSize.Height / 2));
                }

                drawY += tickOffset;
                valueY += tickValue;
            }
        }

        /// <summary>
        /// Draws the tick marks along the "x"-axis
        /// </summary>
        /// <param name="g">graphics object</param>
        private void DrawXTicks(Graphics g)
        {
            SizeF textSize;

            // Offset for origo placement
            float offXorigo = -m_boundsX.Min / m_boundsX.Range;
            float offYorigo = m_boundsY.Max / m_boundsY.Range;

            // Calculate value and offset between each tick mark for x axis
            float tickValue = m_boundsX.Range / (float)m_ticksX;
            float tickOffset = (Width - m_margin.Left - m_margin.Right) / (float)m_ticksX;

            // Start for first tick mark
            float drawX = m_margin.Left;
            float valueX = 0;

            float offseth = m_margin.Top + (Height - m_margin.Top - m_margin.Bottom) * offYorigo;

            // Draw tick marks horizontally
            for (int i = 0; i <= m_ticksX; i++)
            {
                // Check if we should draw the value string at this mark
                bool bDrawValue = (i % ValueAtTick == 0);

                // Draw the tick and the text value, draw wider lines if it's a tick where value string is showing
                g.DrawLine(Pens.Blue,
                    new PointF(drawX, offseth + m_tickWidth / (bDrawValue ? 1 : 2)),
                    new PointF(drawX, offseth - m_tickWidth / (bDrawValue ? 1 : 2))
                );

                // Value to draw
                float value = (m_boundsX.Min + valueX);

                // Only draw at certain tick marks and avoid origo
                if (bDrawValue && value != 0)
                {
                    // Create and measure value string
                    string valueText = value.ToString("0.##");
                    textSize = g.MeasureString(valueText, m_font);

                    // Draw the value string
                    g.DrawString(valueText, m_font, m_foregroundBrush, new PointF(drawX - textSize.Width / 2, offseth + textSize.Height / 2));
                }

                drawX += tickOffset;
                valueX += tickValue;
            }
        }

        /// <summary>
        /// Draws end arrows on axis
        /// </summary>
        /// <param name="g">Graphics object to use for drawing</param>
        private void DrawEndArrows(Graphics g)
        {

            // Offset for origo placement
            float offXorigo = -m_boundsX.Min / m_boundsX.Range;
            float offYorigo = m_boundsY.Max / m_boundsY.Range;

            float offset = m_margin.Left + (Width - m_margin.Left - m_margin.Right) * offXorigo;

            // Arrow on vertical "y"-axis
            g.FillPolygon(
                Brushes.Blue,
                new PointF[] 
                    { 
                        new PointF(offset - 5, m_margin.Top ), 
                        new PointF(offset + 5, m_margin.Top ), 
                        new PointF(offset, m_margin.Top - 10) 
                    }
            );

            offset = m_margin.Top + (Height - m_margin.Top - m_margin.Bottom) * offYorigo;

            // Arrow on horizontal "x"-axis
            g.FillPolygon(
                Brushes.Blue,
                new PointF[] 
                    {
                        new PointF(Width - m_margin.Right, offset - 5), 
                        new PointF(Width - m_margin.Right + 10, offset),
                        new PointF(Width - m_margin.Right, offset + 5)
                    }
            );
        }

        /// <summary>
        /// Draws crossed lines and the value where the cursor is located
        /// </summary>
        /// <param name="g">Graphics object to use for drawing</param>
        private void DrawCursorTracking(Graphics g)
        {
            // Draw lines and info only if within the bounds
            if (m_mousePosition.X > m_margin.Left / 2 && m_mousePosition.X < Width - m_margin.Right / 2
                && m_mousePosition.Y > m_margin.Top / 2 && m_mousePosition.Y < Height - m_margin.Bottom / 2)
            {
                // Draw tracking lines
                g.DrawLine(m_dashedPen, 0, m_mousePosition.Y, Width, m_mousePosition.Y);
                g.DrawLine(m_dashedPen, m_mousePosition.X, 0, m_mousePosition.X, Height);

                // Get value to show
                PointF value = PointPixelsToValue(new PointF(m_mousePosition.X,m_mousePosition.Y));
                string valueStr = String.Format("({0}; {1})",value.X,value.Y);

                // Draw string value
                SizeF textSize = g.MeasureString(valueStr, m_font);
                g.DrawString(valueStr, m_font, m_foregroundBrush, Width - textSize.Width, Height - textSize.Height);
            }
        }

        /// <summary>
        /// Draws all added graphs in the coordinate system
        /// </summary>
        private void DrawAllGraphs(Graphics g)
        {
            // Loop through all added graphs and draw them
            foreach (Graph graph in m_graphs)
            {
                // Convert all to pixels, doing this here when drawing instead of when adding points
                // makes it possible to change bounds at any time.
                PointF[] points = AllPointsToPixels(graph.Points);

                // Draw all points
                DrawPoints(g, new Pen(graph.DrawingColor, graph.DrawingPenWidth), points);
            }
        }

        /// <summary>
        /// Drawns the points with connected lines
        /// </summary>
        /// <param name="g">Graphics object to use</param>
        /// <param name="pen">Pen to use</param>
        /// <param name="points">Point to draw connected</param>
        private void DrawPoints(Graphics g, Pen pen, PointF[] points)
        {
            PointF oldp = default(PointF);
            bool bFirst = true;

            // Loop through points
            foreach (PointF p in points)
            {
                try
                {
                    // If within bounds then draw, allow some drawing outside width and height so lines look connected at ends
                    if (p.X >= -2 * Width && p.X <= 2 * Width && p.Y >= -2 * Height && p.Y <= 2 * Height)
                    {
                        // Only start drawing when we have a starting point
                        if (!bFirst)
                        {
                            g.DrawLine(pen, oldp, p);
                        }

                        bFirst = false;
                        oldp = p;
                    }  
                }
                catch (OverflowException)
                {
                    // Ignore and continue drawing
                }
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Converts all value points in the List to points using pixels for drawing
        /// </summary>
        /// <param name="points">Array of points using "user coordinate" values</param>
        /// <returns>Array of PointF points</returns>
        private PointF[] AllPointsToPixels(PointF[] mpoints)
        {
            PointF[] points = new PointF[mpoints.Length];
            for (int i = 0; i < mpoints.Length; i++)
            {
                points[i] = PointValueToPixels(mpoints[i]);
            }

            return points;
        }

        /// <summary>
        /// Takes a point value in "user coordinates" and converts it to a point in pixels for drawing
        /// </summary>
        /// <param name="point">The point in the user coordinate</param>
        /// <returns>PointF with the pixel coordinates</returns>
        private PointF PointValueToPixels(PointF point)
        {
            // Offset for origo placement
            float offXorigo = -m_boundsX.Min / m_boundsX.Range;
            float offYorigo = m_boundsY.Max / m_boundsY.Range;

            float x = point.X;
            float y = point.Y;

            // Calculate in pixels where this point should be located
            float xpixel = (m_margin.Left + (Width - m_margin.Left - m_margin.Right) * offXorigo) + x * ((Width - m_margin.Left - m_margin.Right) / m_boundsX.Range);
            float ypixel = (m_margin.Top + (Height - m_margin.Top - m_margin.Bottom) * offYorigo) - y * ((Height - m_margin.Top - m_margin.Bottom) / m_boundsY.Range);

            return new PointF(xpixel, ypixel);
        }

        /// <summary>
        /// Takes a point value in pixels and converts it to "user coordinates"
        /// </summary>
        /// <param name="point">The point in pixels</param>
        /// <returns>PointF with the user coordinate</returns>
        private PointF PointPixelsToValue(PointF point)
        {
            float x = point.X;
            float y = point.Y;

            // Calculate the value for the pixel point
            float xvalue = (x - m_margin.Left) * m_boundsX.Range / (Width - m_margin.Left - m_margin.Right) - m_boundsX.Range / 2f;
            float yvalue = m_boundsY.Max - (y - m_margin.Top) * m_boundsY.Range / (Height - m_margin.Top - m_margin.Bottom);

            return new PointF(xvalue, yvalue);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when user control is painted
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        /// <remarks>Draws axis, ticks, points saved and value string</remarks>
        private void CoordinateSystem_Paint(object sender, PaintEventArgs e)
        {
            DrawAxis(e.Graphics);
            DrawEndArrows(e.Graphics);
            DrawTicks(e.Graphics);
            DrawAllGraphs(e.Graphics);
            DrawCursorTracking(e.Graphics);
        }

        /// <summary>
        /// Called when the mouse moves over control
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        /// <remarks>Saves mouse position and invalidates component causing a repaint</remarks>
        private void CoordinateSystem_MouseMove(object sender, MouseEventArgs e)
        {          
            // Save mouse location
            m_mousePosition = new Point(e.X, e.Y);

            // Invalidate to repaint
            Invalidate();
        }
        #endregion
    }
}
