using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculatorControls.Events
{
    /// <summary>
    /// Event args class for the InputCommand event in ButtonPanel
    /// </summary>
    public class InputCommandEventArgs : EventArgs
    {
        // Backing for property
        private string m_command;

        /// <summary>
        /// Default constructor
        /// </summary>
        public InputCommandEventArgs()
            : base()
        {
            m_command = String.Empty;
        }

        /// <summary>
        /// Constructor taking a command string as parameter
        /// </summary>
        /// <param name="command">The command string for this event</param>
        public InputCommandEventArgs(string command)
            : base()
        {
            m_command = command;
        }

        public string CommandString
        {
            get
            {
                return m_command;
            }
            set
            {
                m_command = value;
            }
        }

    }
}
