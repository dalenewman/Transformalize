//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Configuration;
using System.Runtime.Serialization;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Exception class for exceptions that occur when reading configuration metadata from a <see cref="ConfigurationSourceSection"/>.
    /// </summary>
    /// <seealso cref="ConfigurationSourceSection"/>
    [Serializable]
    public class ConfigurationSourceErrorsException : ConfigurationErrorsException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceErrorsException"/> class.
        /// </summary>
        public ConfigurationSourceErrorsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceErrorsException"/> class.
        /// </summary>
        /// <param name="message">A message that describes why this <see cref="ConfigurationSourceErrorsException"/> exception was thrown.</param>
        public ConfigurationSourceErrorsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceErrorsException"/> class.
        /// </summary>
        /// <param name="message">A message that describes why this <see cref="ConfigurationSourceErrorsException"/> exception was thrown.</param>
        /// <param name="innerException">The inner exception that caused this <see cref="ConfigurationSourceErrorsException"/> exception to be thrown.</param>
        public ConfigurationSourceErrorsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceErrorsException"/> class.
        /// </summary>
        /// <param name="message">A message that describes why this <see cref="ConfigurationSourceErrorsException"/> exception was thrown.</param>
        /// <param name="innerException">The inner exception that caused this <see cref="ConfigurationSourceErrorsException"/> exception to be thrown.</param>
        /// <param name="filename">The path to the configuration file that caused this <see cref="ConfigurationSourceErrorsException"/> exception to be thrown.</param>
        /// <param name="line">The line number within the configuration file at which this <see cref="ConfigurationSourceErrorsException"/> exception was thrown.</param>
        public ConfigurationSourceErrorsException(string message, Exception innerException, string filename, int line)
            :base(message, innerException, filename, line)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSourceErrorsException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the information to be serialized.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected ConfigurationSourceErrorsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
