#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Runtime.Serialization;

#if !NETCF_1_0 && !NETCF_2_0

#endif

namespace Transformalize.Libs.SharpZLib.Lzw
{

    /// <summary>
    /// LzwException represents a LZW specific exception
    /// </summary>
#if !NETCF_1_0 && !NETCF_2_0
    [Serializable]
#endif
    public class LzwException : SharpZipBaseException
    {

#if !NETCF_1_0 && !NETCF_2_0
        /// <summary>
        /// Deserialization constructor 
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> for this constructor</param>
        /// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
        protected LzwException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
#endif

        /// <summary>
        /// Initialise a new instance of LzwException
        /// </summary>
        public LzwException() {
        }

        /// <summary>
        /// Initialise a new instance of LzwException with its message string.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        public LzwException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initialise a new instance of <see cref="LzwException"></see>.
        /// </summary>
        /// <param name="message">A <see cref="string"/> that describes the error.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
        public LzwException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }
}
