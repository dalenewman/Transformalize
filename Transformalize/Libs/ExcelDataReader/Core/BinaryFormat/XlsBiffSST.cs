#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents a Shared String Table in BIFF8 format
	/// </summary>
	internal class XlsBiffSST : XlsBiffRecord
	{
		private readonly List<uint> continues = new List<uint>();
		private readonly List<string> m_strings;
		private uint m_size;

		internal XlsBiffSST(byte[] bytes, uint offset)
			: base(bytes, offset)
		{
			m_size = RecordSize;
			m_strings = new List<string>();
		}

		internal XlsBiffSST(byte[] bytes)
			: this(bytes, 0)
		{
		}

		/// <summary>
		/// Returns count of strings in SST
		/// </summary>
		public uint Count
		{
			get { return base.ReadUInt32(0x0); }
		}

		/// <summary>
		/// Returns count of unique strings in SST
		/// </summary>
		public uint UniqueCount
		{
			get { return base.ReadUInt32(0x4); }
		}

		/// <summary>
		/// Reads strings from BIFF stream into SST array
		/// </summary>
		public void ReadStrings()
		{
			var offset = (uint)m_readoffset + 8;
			var last = (uint)m_readoffset + RecordSize;
			var lastcontinue = 0;
			var count = UniqueCount;
			while (offset < last)
			{
				var str = new XlsFormattedUnicodeString(m_bytes, offset);
				var prefix = str.HeadSize;
				var postfix = str.TailSize;
				uint len = str.CharacterCount;
				var size = prefix + postfix + len + ((str.IsMultiByte) ? len : 0);
				if (offset + size > last)
				{
					if (lastcontinue >= continues.Count)
						break;
					var contoffset = continues[lastcontinue];
					var encoding = Buffer.GetByte(m_bytes, (int)contoffset + 4);
					var buff = new byte[size * 2];
					Buffer.BlockCopy(m_bytes, (int)offset, buff, 0, (int)(last - offset));
					if (encoding == 0 && str.IsMultiByte)
					{
						len -= (last - prefix - offset) / 2;
						var temp = Encoding.Default.GetString(m_bytes,
																 (int)contoffset + 5,
																 (int)len);
						var tempbytes = Encoding.Unicode.GetBytes(temp);
						Buffer.BlockCopy(tempbytes, 0, buff, (int)(last - offset), tempbytes.Length);
						Buffer.BlockCopy(m_bytes, (int)(contoffset + 5 + len), buff, (int)(last - offset + len + len), (int)postfix);
						offset = contoffset + 5 + len + postfix;
					}
					else if (encoding == 1 && str.IsMultiByte == false)
					{
						len -= (last - offset - prefix);
						var temp = Encoding.Unicode.GetString(m_bytes,
																 (int)contoffset + 5,
																 (int)(len + len));
						var tempbytes = Encoding.Default.GetBytes(temp);
						Buffer.BlockCopy(tempbytes, 0, buff, (int)(last - offset), tempbytes.Length);
						Buffer.BlockCopy(m_bytes, (int)(contoffset + 5 + len + len), buff, (int)(last - offset + len), (int)postfix);
						offset = contoffset + 5 + len + len + postfix;
					}
					else
					{
						Buffer.BlockCopy(m_bytes, (int)contoffset + 5, buff, (int)(last - offset), (int)(size - last + offset));
						offset = contoffset + 5 + size - last + offset;
					}
					last = contoffset + 4 + BitConverter.ToUInt16(m_bytes, (int)contoffset + 2);
					lastcontinue++;

					str = new XlsFormattedUnicodeString(buff, 0);
				}
				else
				{
					offset += size;
					if (offset == last)
					{
						if (lastcontinue < continues.Count)
						{
							var contoffset = continues[lastcontinue];
							offset = contoffset + 4;
							last = offset + BitConverter.ToUInt16(m_bytes, (int)contoffset + 2);
							lastcontinue++;
						}
						else
							count = 1;
					}
				}
				m_strings.Add(str.Value);
				count--;
				if (count == 0)
					break;
			}
		}

		/// <summary>
		/// Returns string at specified index
		/// </summary>
		/// <param name="SSTIndex">Index of string to get</param>
		/// <returns>string value if it was found, empty string otherwise</returns>
		public string GetString(uint SSTIndex)
		{
			if (SSTIndex < m_strings.Count)
				return m_strings[(int)SSTIndex];


			return string.Empty;
		}

		/// <summary>
		/// Appends Continue record to SST
		/// </summary>
		/// <param name="fragment">Continue record</param>
		public void Append(XlsBiffContinue fragment)
		{
			continues.Add((uint)fragment.Offset);
			m_size += (uint)fragment.Size;
		}
	}
}