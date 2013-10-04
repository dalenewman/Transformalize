#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Creates instances of <see cref="NetworkSender" /> objects for given URLs.
    /// </summary>
    internal interface INetworkSenderFactory
    {
        /// <summary>
        ///     Creates a new instance of the network sender based on a network URL.
        /// </summary>
        /// <param name="url">
        ///     URL that determines the network sender to be created.
        /// </param>
        /// <returns>
        ///     A newly created network sender.
        /// </returns>
        NetworkSender Create(string url);
    }
}