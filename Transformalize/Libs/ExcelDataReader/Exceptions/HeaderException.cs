#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.ExcelDataReader.Exceptions
{
	public class HeaderException : Exception
	{
		public HeaderException()
		{
		}

		public HeaderException(string message)
			: base(message)
		{
		}

		public HeaderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
