#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Net.Sockets;

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Default implementation of <see cref="INetworkSenderFactory" />.
    /// </summary>
    internal class NetworkSenderFactory : INetworkSenderFactory
    {
        public static readonly INetworkSenderFactory Default = new NetworkSenderFactory();

        /// <summary>
        ///     Creates a new instance of the network sender based on a network URL:.
        /// </summary>
        /// <param name="url">
        ///     URL that determines the network sender to be created.
        /// </param>
        /// <returns>
        ///     A newly created network sender.
        /// </returns>
        public NetworkSender Create(string url)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return new HttpNetworkSender(url);
            }

            if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return new HttpNetworkSender(url);
            }

#if !WINDOWS_PHONE_7
            if (url.StartsWith("tcp://", StringComparison.OrdinalIgnoreCase))
            {
                return new TcpNetworkSender(url, AddressFamily.Unspecified);
            }

            if (url.StartsWith("tcp4://", StringComparison.OrdinalIgnoreCase))
            {
                return new TcpNetworkSender(url, AddressFamily.InterNetwork);
            }

            if (url.StartsWith("tcp6://", StringComparison.OrdinalIgnoreCase))
            {
                return new TcpNetworkSender(url, AddressFamily.InterNetworkV6);
            }
#endif

#if !SILVERLIGHT || (WINDOWS_PHONE && !WINDOWS_PHONE_7)
            if (url.StartsWith("udp://", StringComparison.OrdinalIgnoreCase))
            {
                return new UdpNetworkSender(url, AddressFamily.Unspecified);
            }

            if (url.StartsWith("udp4://", StringComparison.OrdinalIgnoreCase))
            {
                return new UdpNetworkSender(url, AddressFamily.InterNetwork);
            }

            if (url.StartsWith("udp6://", StringComparison.OrdinalIgnoreCase))
            {
                return new UdpNetworkSender(url, AddressFamily.InterNetworkV6);
            }
#endif

            throw new ArgumentException("Unrecognized network address", "url");
        }
    }
}