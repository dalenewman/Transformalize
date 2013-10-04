#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Runtime.Serialization;

#if !NETCF_1_0 && !NETCF_2_0

#endif

namespace Transformalize.Libs.SharpZLib.BZip2
{
	/// <summary>
	/// BZip2Exception represents exceptions specific to Bzip2 algorithm
	/// </summary>
#if !NETCF_1_0 && !NETCF_2_0
	[Serializable]
#endif	
	public class BZip2Exception : SharpZipBaseException
	{

#if !NETCF_1_0 && !NETCF_2_0
		/// <summary>
		/// Deserialization constructor 
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> for this constructor</param>
		/// <param name="context"><see cref="StreamingContext"/> for this constructor</param>
		protected BZip2Exception(SerializationInfo info, StreamingContext context)
			: base(info, context)

		{
		}
#endif
		/// <summary>
		/// Initialise a new instance of BZip2Exception.
		/// </summary>
		public BZip2Exception()
		{
		}
		
		/// <summary>
		/// Initialise a new instance of BZip2Exception with its message set to message.
		/// </summary>
		/// <param name="message">The message describing the error.</param>
		public BZip2Exception(string message) : base(message)
		{
		}

		/// <summary>
		/// Initialise an instance of BZip2Exception
		/// </summary>
		/// <param name="message">A message describing the error.</param>
		/// <param name="exception">The exception that is the cause of the current exception.</param>
		public BZip2Exception(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
