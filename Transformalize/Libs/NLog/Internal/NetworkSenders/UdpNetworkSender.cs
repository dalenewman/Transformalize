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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Transformalize.Libs.NLog.Common;

#if !SILVERLIGHT || (WINDOWS_PHONE && !WINDOWS_PHONE_7)

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Sends messages over the network as UDP datagrams.
    /// </summary>
    internal class UdpNetworkSender : NetworkSender
    {
        private EndPoint endpoint;
        private ISocket socket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UdpNetworkSender" /> class.
        /// </summary>
        /// <param name="url">URL. Must start with udp://.</param>
        /// <param name="addressFamily">The address family.</param>
        public UdpNetworkSender(string url, AddressFamily addressFamily)
            : base(url)
        {
            AddressFamily = addressFamily;
        }

        internal AddressFamily AddressFamily { get; set; }

        /// <summary>
        ///     Creates the socket.
        /// </summary>
        /// <param name="addressFamily">The address family.</param>
        /// <param name="socketType">Type of the socket.</param>
        /// <param name="protocolType">Type of the protocol.</param>
        /// <returns>
        ///     Implementation of <see cref="ISocket" /> to use.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Socket is disposed elsewhere.")]
        protected internal virtual ISocket CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return new SocketProxy(addressFamily, socketType, protocolType);
        }

        /// <summary>
        ///     Performs sender-specific initialization.
        /// </summary>
        protected override void DoInitialize()
        {
            endpoint = ParseEndpointAddress(new Uri(Address), AddressFamily);
            socket = CreateSocket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        }

        /// <summary>
        ///     Closes the socket.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        protected override void DoClose(AsyncContinuation continuation)
        {
            lock (this)
            {
                try
                {
                    if (socket != null)
                    {
                        socket.Close();
                    }
                }
                catch (Exception exception)
                {
                    if (exception.MustBeRethrown())
                    {
                        throw;
                    }
                }

                socket = null;
            }
        }

        /// <summary>
        ///     Sends the specified text as a UDP datagram.
        /// </summary>
        /// <param name="bytes">The bytes to be sent.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <param name="length">Number of bytes to send.</param>
        /// <param name="asyncContinuation">The async continuation to be invoked after the buffer has been sent.</param>
        /// <remarks>To be overridden in inheriting classes.</remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose() is called in the event handler.")]
        protected override void DoSend(byte[] bytes, int offset, int length, AsyncContinuation asyncContinuation)
        {
            lock (this)
            {
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(bytes, offset, length);
                args.UserToken = asyncContinuation;
                args.Completed += SocketOperationCompleted;
                args.RemoteEndPoint = endpoint;

                if (!socket.SendToAsync(args))
                {
                    SocketOperationCompleted(socket, args);
                }
            }
        }

        private void SocketOperationCompleted(object sender, SocketAsyncEventArgs e)
        {
            var asyncContinuation = e.UserToken as AsyncContinuation;

            Exception error = null;

            if (e.SocketError != SocketError.Success)
            {
                error = new IOException("Error: " + e.SocketError);
            }

            e.Dispose();

            if (asyncContinuation != null)
            {
                asyncContinuation(error);
            }
        }
    }
}

#endif