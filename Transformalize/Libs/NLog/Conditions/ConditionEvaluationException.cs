#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Runtime.Serialization;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Exception during evaluation of condition expression.
    /// </summary>
#if !NET_CF && !SILVERLIGHT
    [Serializable]
#endif
    public class ConditionEvaluationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionEvaluationException" /> class.
        /// </summary>
        public ConditionEvaluationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionEvaluationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConditionEvaluationException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionEvaluationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ConditionEvaluationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !NET_CF && !SILVERLIGHT
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionEvaluationException" /> class.
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
        protected ConditionEvaluationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
    }
}