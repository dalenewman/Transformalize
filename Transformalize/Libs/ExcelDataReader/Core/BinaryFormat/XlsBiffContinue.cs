#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
	/// <summary>
	/// Represents additional space for very large records
	/// </summary>
	internal class XlsBiffContinue : XlsBiffRecord
	{
		internal XlsBiffContinue(byte[] bytes, uint offset)
			: base(bytes, offset)
		{
		}

		internal XlsBiffContinue(byte[] bytes)
			: this(bytes, 0)
		{
		}
	}
}
