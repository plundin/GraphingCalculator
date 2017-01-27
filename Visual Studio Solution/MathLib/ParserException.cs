
// Source: ParserException.cs
// Patrik Lundin Jan 2012
// patrik@lundin.info

using System;

namespace MathLib
{
    /// <summary>
    /// Exception class for parser related errors in the ExpressionEvaluator
    /// </summary>
    public class ParserException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ParserException()
            : base()
        {

        }

        /// <summary>
        /// Construct a ParserException with a message
        /// </summary>
        /// <param name="message">Message for the exception</param>
        public ParserException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Construct a ParserException with a message and inner exception
        /// </summary>
        /// <param name="message">Message for the exception</param>
        /// <param name="innerException">The inner exception causing this exception</param>
        public ParserException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
