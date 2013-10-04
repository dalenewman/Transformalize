#region License
// /*
// See license included in this library folder.
// */
#endregion
#if !NET_CF

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     Format of the ${stacktrace} layout renderer output.
    /// </summary>
    public enum StackTraceFormat
    {
        /// <summary>
        ///     Raw format (multiline - as returned by StackFrame.ToString() method).
        /// </summary>
        Raw,

        /// <summary>
        ///     Flat format (class and method names displayed in a single line).
        /// </summary>
        Flat,

        /// <summary>
        ///     Detailed flat format (method signatures displayed in a single line).
        /// </summary>
        DetailedFlat,
    }
}

#endif