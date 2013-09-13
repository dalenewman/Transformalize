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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     A base class for all network senders. Supports one-way sending of messages
    ///     over various protocols.
    /// </summary>
    internal abstract class NetworkSender : IDisposable
    {
        private static int currentSendTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkSender" /> class.
        /// </summary>
        /// <param name="url">The network URL.</param>
        protected NetworkSender(string url)
        {
            Address = url;
            LastSendTime = Interlocked.Increment(ref currentSendTime);
        }

        /// <summary>
        ///     Finalizes an instance of the NetworkSender class.
        /// </summary>
        ~NetworkSender()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Gets the address of the network endpoint.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        ///     Gets the last send time.
        /// </summary>
        public int LastSendTime { get; private set; }

        /// <summary>
        ///     Initializes this network sender.
        /// </summary>
        public void Initialize()
        {
            DoInitialize();
        }

        /// <summary>
        ///     Closes the sender and releases any unmanaged resources.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        public void Close(AsyncContinuation continuation)
        {
            DoClose(continuation);
        }

        /// <summary>
        ///     Flushes any pending messages and invokes a continuation.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        public void FlushAsync(AsyncContinuation continuation)
        {
            DoFlush(continuation);
        }

        /// <summary>
        ///     Send the given text over the specified protocol.
        /// </summary>
        /// <param name="bytes">Bytes to be sent.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <param name="length">Number of bytes to send.</param>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        public void Send(byte[] bytes, int offset, int length, AsyncContinuation asyncContinuation)
        {
            LastSendTime = Interlocked.Increment(ref currentSendTime);
            DoSend(bytes, offset, length, asyncContinuation);
        }

        /// <summary>
        ///     Closes the sender and releases any unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs sender-specific initialization.
        /// </summary>
        protected virtual void DoInitialize()
        {
        }

        /// <summary>
        ///     Performs sender-specific close operation.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        protected virtual void DoClose(AsyncContinuation continuation)
        {
            continuation(null);
        }

        /// <summary>
        ///     Performs sender-specific flush.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        protected virtual void DoFlush(AsyncContinuation continuation)
        {
            continuation(null);
        }

        /// <summary>
        ///     Actually sends the given text over the specified protocol.
        /// </summary>
        /// <param name="bytes">The bytes to be sent.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <param name="length">Number of bytes to send.</param>
        /// <param name="asyncContinuation">The async continuation to be invoked after the buffer has been sent.</param>
        /// <remarks>To be overridden in inheriting classes.</remarks>
        protected abstract void DoSend(byte[] bytes, int offset, int length, AsyncContinuation asyncContinuation);

#if !WINDOWS_PHONE_7
        /// <summary>
        ///     Parses the URI into an endpoint address.
        /// </summary>
        /// <param name="uri">The URI to parse.</param>
        /// <param name="addressFamily">The address family.</param>
        /// <returns>Parsed endpoint.</returns>
        protected virtual EndPoint ParseEndpointAddress(Uri uri, AddressFamily addressFamily)
        {
#if SILVERLIGHT
            return new DnsEndPoint(uri.Host, uri.Port, addressFamily);
#else
            switch (uri.HostNameType)
            {
                case UriHostNameType.IPv4:
                case UriHostNameType.IPv6:
                    return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port);

                default:
                    {
                        var addresses = Dns.GetHostEntry(uri.Host).AddressList;
                        foreach (var addr in addresses)
                        {
                            if (addr.AddressFamily == addressFamily || addressFamily == AddressFamily.Unspecified)
                            {
                                return new IPEndPoint(addr, uri.Port);
                            }
                        }

                        throw new IOException("Cannot resolve '" + uri.Host + "' to an address in '" + addressFamily + "'");
                    }
            }
#endif
        }
#endif

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close(ex => { });
            }
        }
    }
}