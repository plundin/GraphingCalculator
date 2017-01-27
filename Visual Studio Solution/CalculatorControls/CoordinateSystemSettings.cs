
// Source: CoordinateSystemSettings.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CalculatorControls.Utils;
using CalculatorControls.Events;

namespace CalculatorControls
{
    /// <summary>
    /// Settings panel for coordinate system.
    /// </summary>
    public partial class CoordinateSystemSettings : UserControl
    {
        // Backing for properties
        float m_xmin, m_xmax, m_ymin, m_ymax;
        Bounds m_origx, m_origy;
        int m_steps;

        /// <summary>
        /// Event for subscribing to when settings in the panel has changed
        /// </summary>
        public event EventHandler SettingsChanged;

        /// <summary>
        /// Event for subscribing to when a command button in the panel is pressed
        /// </summary>
        public event EventHandler<InputCommandEventArgs> InputCommand;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CoordinateSystemSettings()
        {
            InitializeComponent();
            InitDefaults();

            // Steps to use per unit in the x range ([-20, 20] would be 40 * 12 = 480 steps)
            m_steps = 12;
        }

        #region Public Properties

        /// <summary>
        /// Gets the current xmin value
        /// </summary>
        public float Xmin
        {
            get { return m_xmin; }
        }

        /// <summary>
        /// Gets the current xmax value
        /// </summary>
        public float Xmax
        {
            get { return m_xmax; }
        }

        /// <summary>
        /// Gets the current ymin value
        /// </summary>
        public float Ymin
        {
            get { return m_ymin; }
        }

        /// <summary>
        /// Gets the current ymax value
        /// </summary>
        public float Ymax
        {
            get { return m_ymax; }
        }

        /// <summary>
        /// Gets the number of steps to use when stepping through values in the x range
        /// </summary>
        public int XStep
        {
            get { return (int)Math.Round(Math.Sqrt(Math.Pow(m_xmax - m_xmin, 2)) * m_steps); }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Resets default values
        /// </summary>
        public void ResetDefaults()
        {
            txtXmin.Text = m_origx.Min.ToString();
            txtXmax.Text = m_origx.Max.ToString();
            txtYmin.Text = m_origy.Min.ToString();
            txtYmax.Text = m_origy.Max.ToString();

            // Update values and publish event if any changed
            if (GetValues()) OnSettingsChanged();
        }

        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Saves default values
        /// </summary>
        private void InitDefaults()
        {
            try
            {
                // Save the defaults in the boxes so we can reset it if needed
                m_origx = new Bounds(Single.Parse(txtXmin.Text), Single.Parse(txtXmax.Text));
                m_origy = new Bounds(Single.Parse(txtYmin.Text), Single.Parse(txtYmax.Text));
            }
            catch { }

            // Gets current values and sets them in member variables
            GetValues();
        }

        /// <summary>
        /// Sets the current values in member variables to the textboxes on the control
        /// </summary>
        private void SetValues()
        {
            txtXmin.Text = m_xmin.ToString();
            txtXmax.Text = m_xmax.ToString();
            txtYmin.Text = m_ymin.ToString();
            txtYmax.Text = m_ymax.ToString();
        }

        /// <summary>
        /// Gets the values from the textboxes and sets it in the member variables.
        /// If an error occurs when parsing the textbox value it will be reset.
        /// </summary>
        private bool GetValues()
        {
            float xmin, xmax, ymin, ymax;
            bool bChanged = false;

            if (!Single.TryParse(txtXmin.Text, out xmin))
            {
                txtXmin.Text = m_xmin.ToString();
            }
            else
            {
                m_xmin = xmin;
                bChanged = true;
            }

            if (!Single.TryParse(txtXmax.Text, out xmax))
            {
                txtXmax.Text = m_xmax.ToString();
            }
            else
            {
                m_xmax = xmax;
                bChanged = true;
            }

            if (!Single.TryParse(txtYmin.Text, out ymin))
            {
                txtYmin.Text = m_ymin.ToString();
            }
            else
            {
                m_ymin = ymin;
                bChanged = true;
            }

            if (!Single.TryParse(txtYmax.Text, out ymax))
            {
                txtYmax.Text = m_ymax.ToString();
            }
            else
            {
                m_ymax = ymax;
                bChanged = true;
            }

            if(bChanged)
            {
                /* Currently since coordsystem is fixed at center of control
                // just take the highest value and use for both max and min
                // (ideally origo placement should move but this is not implemented)
                m_xmax = Math.Max(Math.Abs(m_xmin), Math.Abs(m_xmax));
                m_xmin = -1 * m_xmax;

                m_ymax = Math.Max(Math.Abs(m_ymin), Math.Abs(m_ymax));
                m_ymin = -1 * m_ymax;
                */

                // Set in textboxes
                SetValues();
            }

            return bChanged;
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when the update button is clicked
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Update values and publish event if any changed
            if (GetValues())
            {
                OnSettingsChanged();
            }
        }

        /// <summary>
        /// Called when the zoom button is pressed
        /// </summary>
        private void btnZoom_Click(object sender, EventArgs e)
        {
            // Scale current bounds 25% in or out depending on SHIFT pressed
            float scale = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift ? 1.25f : 0.75f);

            m_xmin = m_xmin * scale;
            m_xmax = m_xmax * scale;
            m_ymin = m_ymin * scale;
            m_ymax = m_ymax * scale;

            // Set in textboxes
            SetValues();

            // Update values and publish event if any changed
            if (GetValues()) OnSettingsChanged();
        }

        /// <summary>
        /// Called when the reset button is clicked
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetDefaults();
        }

        /// <summary>
        /// Called when the clear button is pressed
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            // Publish event with the buttons text as the command
            OnInputCommand(btn.Text);
        }

        /// <summary>
        /// Called when focus leaves the box, sets values or 
        /// resets if there was an error parsing values
        /// </summary>
        private void txtXmin_Leave(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Publishes the SettingsChanged event to subscribers
        /// </summary>
        private void OnSettingsChanged()
        {
            if (SettingsChanged != null)
            {
                SettingsChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Publishes the InputCommand event to subscribers
        /// </summary>
        /// <param name="command">The string command to publish with the event</param>
        private void OnInputCommand(string command)
        {
            if (InputCommand != null)
            {
                InputCommand(this, new InputCommandEventArgs(command));
            }
        }

        #endregion
    }
}
