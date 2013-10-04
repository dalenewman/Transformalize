#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using Transformalize.Libs.NLog.Common;

#if !WINDOWS_PHONE_7

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Sends messages over a TCP network connection.
    /// </summary>
    internal class TcpNetworkSender : NetworkSender
    {
        private readonly Queue<SocketAsyncEventArgs> pendingRequests = new Queue<SocketAsyncEventArgs>();

        private bool asyncOperationInProgress;
        private AsyncContinuation closeContinuation;
        private AsyncContinuation flushContinuation;
        private Exception pendingError;
        private ISocket socket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TcpNetworkSender" /> class.
        /// </summary>
        /// <param name="url">URL. Must start with tcp://.</param>
        /// <param name="addressFamily">The address family.</param>
        public TcpNetworkSender(string url, AddressFamily addressFamily)
            : base(url)
        {
            AddressFamily = addressFamily;
        }

        internal AddressFamily AddressFamily { get; set; }

        /// <summary>
        ///     Creates the socket with given parameters.
        /// </summary>
        /// <param name="addressFamily">The address family.</param>
        /// <param name="socketType">Type of the socket.</param>
        /// <param name="protocolType">Type of the protocol.</param>
        /// <returns>
        ///     Instance of <see cref="ISocket" /> which represents the socket.
        /// </returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is a factory method")]
        protected internal virtual ISocket CreateSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return new SocketProxy(addressFamily, socketType, protocolType);
        }

        /// <summary>
        ///     Performs sender-specific initialization.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed in the event handler.")]
        protected override void DoInitialize()
        {
            var args = new MySocketAsyncEventArgs();
            args.RemoteEndPoint = ParseEndpointAddress(new Uri(Address), AddressFamily);
            args.Completed += SocketOperationCompleted;
            args.UserToken = null;

            socket = CreateSocket(args.RemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            asyncOperationInProgress = true;

            if (!socket.ConnectAsync(args))
            {
                SocketOperationCompleted(socket, args);
            }
        }

        /// <summary>
        ///     Closes the socket.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        protected override void DoClose(AsyncContinuation continuation)
        {
            lock (this)
            {
                if (asyncOperationInProgress)
                {
                    closeContinuation = continuation;
                }
                else
                {
                    CloseSocket(continuation);
                }
            }
        }

        /// <summary>
        ///     Performs sender-specific flush.
        /// </summary>
        /// <param name="continuation">The continuation.</param>
        protected override void DoFlush(AsyncContinuation continuation)
        {
            lock (this)
            {
                if (!asyncOperationInProgress && pendingRequests.Count == 0)
                {
                    continuation(null);
                }
                else
                {
                    flushContinuation = continuation;
                }
            }
        }

        /// <summary>
        ///     Sends the specified text over the connected socket.
        /// </summary>
        /// <param name="bytes">The bytes to be sent.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <param name="length">Number of bytes to send.</param>
        /// <param name="asyncContinuation">The async continuation to be invoked after the buffer has been sent.</param>
        /// <remarks>To be overridden in inheriting classes.</remarks>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed in the event handler.")]
        protected override void DoSend(byte[] bytes, int offset, int length, AsyncContinuation asyncContinuation)
        {
            var args = new MySocketAsyncEventArgs();
            args.SetBuffer(bytes, offset, length);
            args.UserToken = asyncContinuation;
            args.Completed += SocketOperationCompleted;

            lock (this)
            {
                pendingRequests.Enqueue(args);
            }

            ProcessNextQueuedItem();
        }

        private void CloseSocket(AsyncContinuation continuation)
        {
            try
            {
                var sock = socket;
                socket = null;

                if (sock != null)
                {
                    sock.Close();
                }

                continuation(null);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                continuation(exception);
            }
        }

        private void SocketOperationCompleted(object sender, SocketAsyncEventArgs e)
        {
            lock (this)
            {
                asyncOperationInProgress = false;
                var asyncContinuation = e.UserToken as AsyncContinuation;

                if (e.SocketError != SocketError.Success)
                {
                    pendingError = new IOException("Error: " + e.SocketError);
                }

                e.Dispose();

                if (asyncContinuation != null)
                {
                    asyncContinuation(pendingError);
                }
            }

            ProcessNextQueuedItem();
        }

        private void ProcessNextQueuedItem()
        {
            SocketAsyncEventArgs args;

            lock (this)
            {
                if (asyncOperationInProgress)
                {
                    return;
                }

                if (pendingError != null)
                {
                    while (pendingRequests.Count != 0)
                    {
                        args = pendingRequests.Dequeue();
                        var asyncContinuation = (AsyncContinuation) args.UserToken;
                        asyncContinuation(pendingError);
                    }
                }

                if (pendingRequests.Count == 0)
                {
                    var fc = flushContinuation;
                    if (fc != null)
                    {
                        flushContinuation = null;
                        fc(pendingError);
                    }

                    var cc = closeContinuation;
                    if (cc != null)
                    {
                        closeContinuation = null;
                        CloseSocket(cc);
                    }

                    return;
                }

                args = pendingRequests.Dequeue();

                asyncOperationInProgress = true;
                if (!socket.SendAsync(args))
                {
                    SocketOperationCompleted(socket, args);
                }
            }
        }

        /// <summary>
        ///     Facilitates mocking of <see cref="SocketAsyncEventArgs" /> class.
        /// </summary>
        internal class MySocketAsyncEventArgs : SocketAsyncEventArgs
        {
            /// <summary>
            ///     Raises the Completed event.
            /// </summary>
            public void RaiseCompleted()
            {
                OnCompleted(this);
            }
        }
    }
}

#endif