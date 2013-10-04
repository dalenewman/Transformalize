#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Runtime.Serialization;

#if !NETCF_1_0 && !NETCF_2_0

#endif

namespace Transformalize.Libs.SharpZLib.Tar {
	
	/// <summary>
	/// TarExceptions are used for exceptions specific to tar classes and code.	
	/// </summary>
#if !NETCF_1_0 && !NETCF_2_0
	[Serializable]
#endif
	public class TarException : SharpZipBaseException
	{
#if !NETCF_1_0 && !NETCF_2_0
		/// <summary>
		/// Deserialization constructor 
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> for this constructor</param>
		/// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
		protected TarException(SerializationInfo info, StreamingContext context)
			: base(info, context)

		{
		}
#endif

		/// <summary>
		/// Initialises a new instance of the TarException class.
		/// </summary>
		public TarException()
		{
		}
		
		/// <summary>
		/// Initialises a new instance of the TarException class with a specified message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public TarException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message">A message describing the error.</param>
		/// <param name="exception">The exception that is the cause of the current exception.</param>
		public TarException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
