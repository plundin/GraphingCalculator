
// Source: ButtonPanel.cs
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
using CalculatorControls.Events;

namespace CalculatorControls
{
    /// <summary>
    /// The button input panel for the calculator
    /// </summary>
    public partial class ButtonPanel : UserControl
    {
        /// <summary>
        /// Event for subscribing to commands sent from the ButtonPanel
        /// </summary>
        public event EventHandler<InputCommandEventArgs> InputCommand;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ButtonPanel()
        {
            InitializeComponent();
            InitializeButtons();
            InitializeRegionalSettings();
        }

        private void InitializeRegionalSettings()
        {
            // Put correct decimal separator on button
            btnPoint.Text = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
        }

        /// <summary>
        /// Initializes event handlers for the command buttons on the panel
        /// </summary>
        private void InitializeButtons()
        {
            // Add subscriber to click for all buttons on the panel
            foreach (Control c in tableLayoutPanel1.Controls)
            {
                if (c is Button) c.Click += new EventHandler(button_Click);
            }
        }

        /// <summary>
        /// Called when a button is clicked
        /// </summary>
        /// <remarks>Causes an InputCommand to be published with the buttons text as the command</remarks>
        private void button_Click(object sender, EventArgs e)
        {
           Button btn = sender as Button;
           if(btn == null) return;

           // Publish event with the buttons text as the command
           OnInputCommand(btn.Text);
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
    }
}
