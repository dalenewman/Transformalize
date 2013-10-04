#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;

namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents a string (max 255 bytes)
	/// </summary>
	internal class XlsBiffLabelCell : XlsBiffBlankCell
	{
		private Encoding m_UseEncoding = Encoding.Default;

		internal XlsBiffLabelCell(byte[] bytes)
			: this(bytes, 0)
		{
		}

		internal XlsBiffLabelCell(byte[] bytes, uint offset)
			: base(bytes, offset)
		{
		}

		/// <summary>
		/// Encoding used to deal with strings
		/// </summary>
		public Encoding UseEncoding
		{
			get { return m_UseEncoding; }
			set { m_UseEncoding = value; }
		}

		/// <summary>
		/// Length of string value
		/// </summary>
		public byte Length
		{
			get { return base.ReadByte(0x6); }
		}

		/// <summary>
		/// Returns value of this cell
		/// </summary>
		public string Value
		{
			get
			{
				var bts = base.ReadArray(0x8, Length * (Helpers.IsSingleByteEncoding(m_UseEncoding) ? 1 : 2));

				return m_UseEncoding.GetString(bts, 0, bts.Length);
			}
		}
	}
}