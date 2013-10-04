#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Runtime.Serialization;

#if !NETCF_1_0 && !NETCF_2_0

#endif

namespace Transformalize.Libs.SharpZLib.GZip
{
	/// <summary>
	/// GZipException represents a Gzip specific exception	
	/// </summary>
#if !NETCF_1_0 && !NETCF_2_0
	[Serializable]
#endif	
	public class GZipException : SharpZipBaseException
	{
#if !NETCF_1_0 && !NETCF_2_0
		/// <summary>
		/// Deserialization constructor 
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> for this constructor</param>
		/// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
		protected GZipException(SerializationInfo info, StreamingContext context)
			: base(info, context)

		{
		}
#endif

		/// <summary>
		/// Initialise a new instance of GZipException
		/// </summary>
		public GZipException()
		{
		}
		
		/// <summary>
		/// Initialise a new instance of GZipException with its message string.
		/// </summary>
		/// <param name="message">A <see cref="string"/> that describes the error.</param>
		public GZipException(string message)
			: base(message)
		{
		}
		
		/// <summary>
		/// Initialise a new instance of <see cref="GZipException"></see>.
		/// </summary>
		/// <param name="message">A <see cref="string"/> that describes the error.</param>
		/// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
		public GZipException(string message, Exception innerException)
			: base (message, innerException)
		{	
		}
	}
}
