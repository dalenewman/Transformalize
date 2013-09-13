using System;
using System.Runtime.Serialization;

namespace Transformalize.Libs.Rhino.Etl.Exceptions
{
    /// <summary>
    ///     Thrown when an access to a quacking dictionary is made with more than a single
    ///     parameter
    /// </summary>
    [Serializable]
    public class ParameterCountException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterCountException" /> class.
        /// </summary>
        public ParameterCountException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterCountException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParameterCountException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterCountException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ParameterCountException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterCountException" /> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="info" /> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        ///     The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).
        /// </exception>
        protected ParameterCountException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}