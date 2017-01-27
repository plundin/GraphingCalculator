
// Source: CalculatorMain.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CalculatorControls.Events;
using CalculatorControls.Utils;
using CalculatorControls;
using MathLib;

namespace Calculator
{
    /// <summary>
    /// Graph drawing scientific calculator
    /// </summary>
    public partial class CalculatorMain : Form
    {
        // Holds the expression evaluator instance
        private ExpressionEvaluator m_eval;

        // Holds expressions we graph
        private Dictionary<string,string> m_graphExpressions;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CalculatorMain()
        {
            InitializeComponent();
            InitializeEventHandlers();

            // Create the expression evaluator we will use to evaluate with
            m_eval = new ExpressionEvaluator();

            // Create dictionary to hold expressions we graph
            m_graphExpressions = new Dictionary<string,string>();
        }

        #region Private Methods

        /// <summary>
        /// Initializes the event handlers
        /// </summary>
        private void InitializeEventHandlers()
        {
            // Subscribe to receive button commands from panel
            btnPanel.InputCommand += new EventHandler<InputCommandEventArgs>(btnPanel_InputCommand);
            // Subscribe to settings from coord system settings panel
            coordinateSystemSettings1.SettingsChanged += new EventHandler(coordinateSystemSettings1_SettingsChanged);
            // Subscribe to the command event from the coord system settings panel
            coordinateSystemSettings1.InputCommand += new EventHandler<InputCommandEventArgs>(coordinateSystemSettings1_InputCommand);
        }

        /// <summary>
        /// Interprets the command string and performs the correct action
        /// </summary>
        /// <param name="command">command string</param>
        private void HandleCommand(string command)
        {
            // Reset any previous error
            ResetError();

            // Interpret the command string and perform action needed
            switch (command)
            {
                case "=":
                    Evaluate();                    
                    break;
                case "C":
                    Cls();
                    break;
                case "±":
                    Negate();
                    break;
                case "Clear":
                    ClearCoordSystem();
                    break;
                case "←":
                    Back();
                    break;
                default:
                    // Build the string expression from command
                    BuildExpression(command);
                    break;
            }
        }

        /// <summary>
        /// Evaluates the current expression and shows the result
        /// </summary>
        private void Evaluate()
        {
            string expression = txtInOut.Text.Trim();
            if (String.IsNullOrEmpty(expression)) return;

            try
            {
                // Evaluate the built expression with the expression evaluator
                // Replace any comma in result with dot for consistency
                txtInOut.Text = m_eval.Eval(txtInOut.Text).ToString();

                SetCaretPosition(0);
            }
            catch (ParserException p)
            {
                // If we failed to evaluate try to draw
                EvalFx(expression);

                // txtError.Text = p.Message;
            }
        }

        /// <summary>
        /// Evaluates the current expression as a function of x and adds it to the coordinate system
        /// </summary>
        private void EvalFx(string expression)
        {
            try
            {
                // Evaluate function in the x range from coord settings panel
                PointF[] points = m_eval.EvalFx(
                    expression,
                    coordinateSystemSettings1.Xmin,
                    coordinateSystemSettings1.Xmax,
                    coordinateSystemSettings1.XStep
                );

                // Draw graph in coordinate system
                coordinateSystem1.AddGraph(expression, points);

                // Save the expression 
                if (!m_graphExpressions.ContainsKey(expression))
                {
                    m_graphExpressions.Add(expression, null);
                }
            }
            catch (ParserException p)
            {
                txtError.Text = p.Message;
            }
        }

        /// <summary>
        /// Builds a new expression by negating the current expression 
        /// </summary>
        private void Negate()
        {
            if (String.IsNullOrEmpty(txtInOut.Text)) return;

            // Negate expression by putting it within paranthesis with a minus in front
            txtInOut.Text = String.Format("-({0})", txtInOut.Text);

            SetCaretPosition(0);
        }

        /// <summary>
        /// Removes the character at the position immediately before the caret position
        /// </summary>
        private void Back()
        {
            int iPos = txtInOut.SelectionStart - 1;
            if (iPos >= 0 && iPos <= txtInOut.Text.Length)
            {
                txtInOut.Text = txtInOut.Text.Remove(iPos, 1);

                txtInOut.Focus();
                txtInOut.SelectionStart = iPos;
                txtInOut.SelectionLength = 0;
            }
        }

        /// <summary>
        /// Builds an expression using the command string
        /// </summary>
        /// <param name="command">command string</param>
        private void BuildExpression(string command)
        {
            // Assume ending with () is a function
            bool bIsFunction = command.EndsWith("()");

            if (bIsFunction && txtInOut.SelectionLength > 0)
            {
                // Create command out of selected text
                command = command.Insert(command.Length - 1, txtInOut.SelectedText);

                // Remove selected expression and insert new
                txtInOut.Text = txtInOut.Text.Remove(txtInOut.SelectionStart, txtInOut.SelectionLength).Insert(txtInOut.SelectionStart, command);

                // We already handled this
                bIsFunction = false;
            }
            else
            {
                // Append command
                txtInOut.Text = txtInOut.Text.Insert(txtInOut.SelectionStart, command);
            }

            // Set caret position to end, or within the () if it was a function
            if (bIsFunction)
            {
                SetCaretPosition(1); // Put one step into function
            }
            else
            {
                SetCaretPosition(0); // Set to end
            }
        }

        /// <summary>
        /// Sets the caret position for next inset in the in out textbox
        /// </summary>
        /// <param name="offset">The offset from text length to place the caret at</param>
        private void SetCaretPosition(int offset)
        {
            txtInOut.Focus();
            txtInOut.SelectionStart = txtInOut.Text.Length - offset;
            txtInOut.SelectionLength = 0;
        }

        /// <summary>
        /// Clears the current expression and the output textbox
        /// </summary>
        private void Cls()
        {
            ResetOutput();
        }

        /// <summary>
        /// Resets output textbox
        /// </summary>
        private void ResetOutput()
        {
            txtInOut.Text = "";
            SetCaretPosition(0);
        }

        /// <summary>
        /// Resets error textbox
        /// </summary>
        private void ResetError()
        {
            txtError.Text = String.Empty;
        }

        /// <summary>
        /// Clears the coordinate system
        /// </summary>
        private void ClearCoordSystem()
        {
            if (MessageBox.Show("Clear coordinate system?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                coordinateSystem1.DeleteAll();
            }         
        }

        #endregion

        /// <summary>
        /// Called when the button panel receives input.
        /// </summary>
        private void btnPanel_InputCommand(object sender, InputCommandEventArgs e)
        {
            HandleCommand(e.CommandString);
        }

        /// <summary>
        /// Called when coordinate system settings are updated 
        /// </summary>
        private void coordinateSystemSettings1_SettingsChanged(object sender, EventArgs e)
        {
            // Get reference to settings panel
            CoordinateSystemSettings settings = sender as CoordinateSystemSettings;
            if (sender == null) return;

            // New bounds
            Bounds boundsX = new Bounds(settings.Xmin, settings.Xmax);
            Bounds boundsY = new Bounds(settings.Ymin, settings.Ymax);

            // Check to see if we need to update all graphs
            bool bNeedUpdate = !(boundsX.IsWithin(coordinateSystem1.BoundsX) && boundsY.IsWithin(coordinateSystem1.BoundsY));

            // Update coordinate system
            coordinateSystem1.BoundsX = boundsX;
            coordinateSystem1.BoundsY = boundsY;

            if (bNeedUpdate)
            {
                // Remove all graphs
                coordinateSystem1.DeleteAll();

                // Re-evaluate all saved expressions
                foreach (string fx in m_graphExpressions.Keys) EvalFx(fx);
            }

            // Cause a redraw
            coordinateSystem1.Invalidate();
        }

        /// <summary>
        /// Called when a command button on the coord system settings panel is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void coordinateSystemSettings1_InputCommand(object sender, InputCommandEventArgs e)
        {
            HandleCommand(e.CommandString);
        }

        /// <summary>
        /// Opens the help html file when the help icon is clicked on the tool strip menu
        /// </summary>
        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            Help.ShowHelpIndex(this, "doc/calculator.html");
        }

        /// <summary>
        /// Called when the save button on the toolstrip menu is clicked
        /// </summary>
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            // Show dialog, return if cancelled
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel) return;

            // Call save in coordinate system
            try
            {
                coordinateSystem1.Save(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Called when the open file button on the toolstrip is clicked
        /// </summary>
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            // Fill default filename if one has been selected in save dialog
            if (String.IsNullOrEmpty(openFileDialog.FileName)
                && !String.IsNullOrEmpty(saveFileDialog.FileName))
            {
                openFileDialog.FileName = saveFileDialog.FileName;
            }

            // Show file dialog, return if cancelled
            if (openFileDialog.ShowDialog() == DialogResult.Cancel) return;

            // Call load in diagram
            try
            {
                coordinateSystem1.Load(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            // Show print dialog, return if cancelled
            if (printDialog.ShowDialog() == DialogResult.Cancel) return;
            
            // Print the document
            printDocument.Print();
        }

        /// <summary>
        /// Called when printing of the print document starts
        /// </summary>
        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Draw the coordinate system to the printer document graphics object
            coordinateSystem1.DrawToGraphics(e.Graphics);
        }

        /// <summary>
        /// Called on keydown in text input field
        /// </summary>
        private void txtInOut_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle enter, delete and back button
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Evaluate();
            }
            else if (e.KeyCode == Keys.Back)
            {
                e.Handled = true;

                if (txtInOut.SelectionLength > 0)
                {
                    // Delete all selected text
                    txtInOut.Text = txtInOut.Text.Remove(txtInOut.SelectionStart,txtInOut.SelectionLength);
                }
                else
                {
                    Back();
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (txtInOut.SelectionLength > 0)
                {
                    // Delete all selected text
                    txtInOut.Text = txtInOut.Text.Remove(txtInOut.SelectionStart, txtInOut.SelectionLength);
                }
            }
        }
    }
}
