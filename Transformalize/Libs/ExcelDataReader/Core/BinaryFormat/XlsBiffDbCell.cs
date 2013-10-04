#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Collections.Generic;

namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents cell-indexing record, finishes each row values block
	/// </summary>
	internal class XlsBiffDbCell : XlsBiffRecord
	{
		internal XlsBiffDbCell(byte[] bytes, uint offset)
			: base(bytes, offset)
		{
		}

		internal XlsBiffDbCell(byte[] bytes)
			: this(bytes, 0)
		{
		}

		/// <summary>
		/// Offset of first row linked with this record
		/// </summary>
		public int RowAddress
		{
			get { return (Offset - base.ReadInt32(0x0)); }
		}

		/// <summary>
		/// Addresses of cell values
		/// </summary>
		public uint[] CellAddresses
		{
			get
			{
				var a = RowAddress - 20; // 20 assumed to be row structure size

				var tmp = new List<uint>();
				for (var i = 0x4; i < RecordSize; i += 4)
					tmp.Add((uint) a + base.ReadUInt16(i));
				return tmp.ToArray();
			}
		}
	}
}