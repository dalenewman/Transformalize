#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Text;

namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents a string value of formula
	/// </summary>
	internal class XlsBiffFormulaString : XlsBiffRecord
	{
	    private const int LEADING_BYTES_COUNT = 3;
	    private Encoding m_UseEncoding = Encoding.Default;

	    internal XlsBiffFormulaString(byte[] bytes)
			: this(bytes, 0)
		{
		}

		internal XlsBiffFormulaString(byte[] bytes, uint offset)
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
		/// Length of the string
		/// </summary>
		public ushort Length
		{
			get { return base.ReadUInt16(0x0); }
		}

		/// <summary>
		/// String text
		/// </summary>
		public string Value
		{
			get
			{
				//is unicode?
				if (base.ReadUInt16(0x01) != 0)
				{
					return Encoding.Unicode.GetString(m_bytes, m_readoffset + LEADING_BYTES_COUNT, Length * 2);
				}

				return m_UseEncoding.GetString(m_bytes, m_readoffset + LEADING_BYTES_COUNT, Length);
			}
		}
	}
}