#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.ExcelDataReader.Core.BinaryFormat
{
    /// <summary>
    /// If present the Calculate Message was in the status bar when Excel saved the file.
    /// This occurs if the sheet changed, the Manual calculation option was on, and the Recalculate Before Save option was off.    
    /// </summary>
    internal class XlsBiffUncalced : XlsBiffRecord
    {
        internal XlsBiffUncalced(byte[] bytes, uint offset)
            : base(bytes, offset)
        {
        }

        internal XlsBiffUncalced(byte[] bytes)
            : this(bytes, 0)
        {
        }
    }
}