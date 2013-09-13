#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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