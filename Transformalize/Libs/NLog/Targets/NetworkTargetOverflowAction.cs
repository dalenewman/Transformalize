#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Action that should be taken if the message overflows.
    /// </summary>
    public enum NetworkTargetOverflowAction
    {
        /// <summary>
        ///     Report an error.
        /// </summary>
        Error,

        /// <summary>
        ///     Split the message into smaller pieces.
        /// </summary>
        Split,

        /// <summary>
        ///     Discard the entire message.
        /// </summary>
        Discard
    }
}