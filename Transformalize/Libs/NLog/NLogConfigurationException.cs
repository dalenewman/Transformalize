#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Runtime.Serialization;

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     Exception thrown during NLog configuration.
    /// </summary>
#if !NET_CF && !SILVERLIGHT
    [Serializable]
#endif
    public class NLogConfigurationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogConfigurationException" /> class.
        /// </summary>
        public NLogConfigurationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogConfigurationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NLogConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogConfigurationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NLogConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !NET_CF && !SILVERLIGHT
        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogConfigurationException" /> class.
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
        protected NLogConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}