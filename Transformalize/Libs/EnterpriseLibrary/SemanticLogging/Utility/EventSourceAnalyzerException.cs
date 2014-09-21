#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Semantic Logging Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

using System;
using System.Runtime.Serialization;

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Utility
{
    /// <summary>
    /// Represents errors that occur during <see cref="EventSourceAnalyzer"/> execution.
    /// </summary>
    [Serializable]
    public class EventSourceAnalyzerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceAnalyzerException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EventSourceAnalyzerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceAnalyzerException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public EventSourceAnalyzerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceAnalyzerException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected EventSourceAnalyzerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
