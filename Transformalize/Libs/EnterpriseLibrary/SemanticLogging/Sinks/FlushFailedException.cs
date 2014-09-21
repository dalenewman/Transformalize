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
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Properties;

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Sinks
{
    /// <summary>
    /// Represents an error while doing a flush operation.
    /// </summary>
    [Serializable]
    public class FlushFailedException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="FlushFailedException" /> class.</summary>
        public FlushFailedException()
            : this(Resources.FlushFailedException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="FlushFailedException" /> class.</summary>
        /// <param name="message">The exception message.</param>
        public FlushFailedException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="FlushFailedException" /> class.</summary>
        /// <param name="innerException">The references to the inner exception that is the cause of this exception.</param>
        public FlushFailedException(Exception innerException)
            : this(Resources.FlushFailedException, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="FlushFailedException" /> class.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The references to the inner exception that is the cause of this exception.</param>
        public FlushFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the System.Exception class with serialized data.</summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        protected FlushFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
    }
}
