#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     The action to be taken when the queue overflows.
    /// </summary>
    public enum AsyncTargetWrapperOverflowAction
    {
        /// <summary>
        ///     Grow the queue.
        /// </summary>
        Grow,

        /// <summary>
        ///     Discard the overflowing item.
        /// </summary>
        Discard,

#if !NET_CF
        /// <summary>
        ///     Block until there's more room in the queue.
        /// </summary>
        Block,
#endif
    }
}