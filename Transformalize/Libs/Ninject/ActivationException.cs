#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

#region Using Directives

using System;
using System.Runtime.Serialization;

#if !NO_EXCEPTION_SERIALIZATION

#endif

#endregion

namespace Transformalize.Libs.Ninject
{
    /// <summary>
    ///     Indicates that an error occured during activation of an instance.
    /// </summary>
#if !NO_EXCEPTION_SERIALIZATION
    [Serializable]
#endif
    public class ActivationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationException" /> class.
        /// </summary>
        public ActivationException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ActivationException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ActivationException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if !NO_EXCEPTION_SERIALIZATION
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActivationException" /> class.
        /// </summary>
        /// <param name="info">The serialized object data.</param>
        /// <param name="context">The serialization context.</param>
        protected ActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
#endif
    }
}