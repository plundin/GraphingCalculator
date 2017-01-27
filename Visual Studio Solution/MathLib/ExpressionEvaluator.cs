
// Source: ExpressionEvaluator.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using info.lundin.math;

namespace MathLib
{
    public class ExpressionEvaluator
    {
        // Holds reference to the evaluator
        ExpressionParser m_eval;
        // Holds reference to hashtable for values
        Hashtable m_values;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExpressionEvaluator()
        {
            // Create the evaluator
            m_eval = new ExpressionParser();
            // Create hashtable for values
            m_values = new Hashtable();
        }

        /// <summary>
        /// Evaluates a math expression and returns a double value
        /// </summary>
        /// <param name="expression">The expression to evaluate</param>
        /// <returns>result of the evaluation</returns>
        public double Eval(string expression)
        {
            // Evaluate and try to cast as a double
            double result = 0;

            try
            {
                // Evaluate and try to convert to a double, the JScript eval might have returned an object
                // if it was not a simple math expression 
                result = System.Convert.ToDouble(m_eval.Parse(expression, m_values));

                return result;
            }
            catch (InvalidCastException)
            {
                // If we failed to cast as a double value throw a ParserException
                throw new ParserException("The result returned could not be cast to a double value ");
            }
            catch (Exception e)
            {
                // Relay the message from any other exception
                throw new ParserException(e.Message, e);
            }
        }

        /// <summary>
        /// Evaluates the expression as a function f(x) in the range [xmin,xmax]
        /// </summary>
        /// <param name="fx">A string expression that is a mathematical function of x, for example cos(x) or 3x-5</param>
        /// <param name="xmin">The min value of the x range to calculate values for</param>
        /// <param name="xmax">The max value of the x range to calculate values for</param>
        /// <param name="npoints">The number of points to calculate in the range</param>
        /// <returns>An array of PointF objects</returns>
        public PointF[] EvalFx(string fx, float xmin, float xmax, int npoints)
        {
            if (npoints <= 0) throw new ArgumentException("Number of points must be >= 0", "npoints");
            if (xmin >= xmax) throw new ArgumentException("The value of xmin must be less than xmax", "xmin");

            // Create array to hold points
            PointF[] points = new PointF[npoints];

            // The value we increase per step in x direction, 
            // (distance between xmax and xmin through number of points)
            float xValueStep = (float)Math.Sqrt(Math.Pow(xmax - xmin, 2)) / npoints;
            
            // Starting x value
            float xValue = xmin;

            // Loop and evaluate the expression
            for (int i = 0; i < npoints; i++)
            {
                m_values["x"] = xValue.ToString();

                // Evaluate expression and create a PointF
                points[i] = new PointF(xValue, (float)Eval(fx));

                // Increase x value
                xValue += xValueStep;
            }

            m_values.Remove("x");

            return points;
        }
    }
}
