#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Layouts
{
    /// <summary>
    ///     Specifies allowes CSV quoting modes.
    /// </summary>
    public enum CsvQuotingMode
    {
        /// <summary>
        ///     Quote all column.
        /// </summary>
        All,

        /// <summary>
        ///     Quote nothing.
        /// </summary>
        Nothing,

        /// <summary>
        ///     Quote only whose values contain the quote symbol or
        ///     the separator.
        /// </summary>
        Auto
    }
}