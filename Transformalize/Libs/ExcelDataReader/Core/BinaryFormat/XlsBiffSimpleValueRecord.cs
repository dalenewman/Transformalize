#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents record with the only two-bytes value
	/// </summary>
	internal class XlsBiffSimpleValueRecord : XlsBiffRecord
	{
		internal XlsBiffSimpleValueRecord(byte[] bytes, uint offset)
			: base(bytes, offset)
		{
		}

		internal XlsBiffSimpleValueRecord(byte[] bytes)
			: this(bytes, 0)
		{
		}

		/// <summary>
		/// Returns value
		/// </summary>
		public ushort Value
		{
			get { return ReadUInt16(0x0); }
		}
	}
}
