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

#if !WINDOWS_PHONE_7

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Socket proxy for mocking Socket code.
    /// </summary>
    internal sealed class SocketProxy : ISocket, IDisposable
    {
        private readonly Socket socket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SocketProxy" /> class.
        /// </summary>
        /// <param name="addressFamily">The address family.</param>
        /// <param name="socketType">Type of the socket.</param>
        /// <param name="protocolType">Type of the protocol.</param>
        internal SocketProxy(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            socket = new Socket(addressFamily, socketType, protocolType);
        }

        /// <summary>
        ///     Closes the wrapped socket.
        /// </summary>
        public void Close()
        {
            socket.Close();
        }

#if USE_LEGACY_ASYNC_API || NET_CF
    // emulate missing .NET CF behavior

    /// <summary>
    /// Invokes ConnectAsync method on the wrapped socket.
    /// </summary>
    /// <param name="args">The <see cref="SocketAsyncEventArgs"/> instance containing the event data.</param>
    /// <returns>Result of original method.</returns>
        public bool ConnectAsync(SocketAsyncEventArgs args)
        {
            this.socket.BeginConnect(args.RemoteEndPoint, args.EndConnect, this.socket);
            return true;
        }

        /// <summary>
        /// Invokes SendAsync method on the wrapped socket.
        /// </summary>
        /// <param name="args">The <see cref="SocketAsyncEventArgs"/> instance containing the event data.</param>
        /// <returns>Result of original method.</returns>
        public bool SendAsync(SocketAsyncEventArgs args)
        {
            this.socket.BeginSend(args.Buffer, args.Offset, args.Count, args.SocketFlags, args.EndSend, this.socket);
            return true;
        }

        /// <summary>
        /// Invokes SendToAsync method on the wrapped socket.
        /// </summary>
        /// <param name="args">The <see cref="SocketAsyncEventArgs"/> instance containing the event data.</param>
        /// <returns>Result of original method.</returns>
        public bool SendToAsync(SocketAsyncEventArgs args)
        {
            this.socket.BeginSendTo(args.Buffer, args.Offset, args.Count, args.SocketFlags, args.RemoteEndPoint, args.EndSendTo, this.socket);
            return true;
        }
#else
        /// <summary>
        ///     Invokes ConnectAsync method on the wrapped socket.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="SocketAsyncEventArgs" /> instance containing the event data.
        /// </param>
        /// <returns>Result of original method.</returns>
        public bool ConnectAsync(SocketAsyncEventArgs args)
        {
            return socket.ConnectAsync(args);
        }

        /// <summary>
        ///     Invokes SendAsync method on the wrapped socket.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="SocketAsyncEventArgs" /> instance containing the event data.
        /// </param>
        /// <returns>Result of original method.</returns>
        public bool SendAsync(SocketAsyncEventArgs args)
        {
            return socket.SendAsync(args);
        }

#if !SILVERLIGHT || (WINDOWS_PHONE && !WINDOWS_PHONE_7)
        /// <summary>
        ///     Invokes SendToAsync method on the wrapped socket.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="SocketAsyncEventArgs" /> instance containing the event data.
        /// </param>
        /// <returns>Result of original method.</returns>
        public bool SendToAsync(SocketAsyncEventArgs args)
        {
            return socket.SendToAsync(args);
        }
#endif

#endif

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ((IDisposable) socket).Dispose();
        }
    }
}

#endif