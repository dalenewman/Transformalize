#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
#if NET_CF || USE_LEGACY_ASYNC_API

namespace NLog.Internal.NetworkSenders
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Emulate missing functionality from .NET Compact Framework
    /// </summary>
    internal class SocketAsyncEventArgs : EventArgs, IDisposable
    {
        public EventHandler<SocketAsyncEventArgs> Completed;
        public EndPoint RemoteEndPoint { get; set; }
        public object UserToken { get; set; }
        public SocketError SocketError { get; set; }

        public byte[] Buffer { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
        public SocketFlags SocketFlags { get; set; }

        public void EndConnect(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            try
            {
                socket.EndConnect(result);
                this.SocketError = SocketError.Success;
            }
            catch (SocketException)
            {
                this.SocketError = SocketError.SocketError;
            }

            this.OnCompleted(this);
        }

        public void EndSend(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            try
            {
                int sendResult = socket.EndSend(result);
                this.SocketError = SocketError.Success;
            }
            catch (SocketException)
            {
                this.SocketError = SocketError.SocketError;
            }

            this.OnCompleted(this);
        }


        public void EndSendTo(IAsyncResult result)
        {
            var socket = (Socket)result.AsyncState;

            try
            {
                int sendResult = socket.EndSendTo(result);
                this.SocketError = SocketError.Success;
            }
            catch (SocketException)
            {
                this.SocketError = SocketError.SocketError;
            }

            this.OnCompleted(this);
        }

        public void Dispose()
        {
            // not needed
        }

        internal void SetBuffer(byte[] bytes, int offset, int length)
        {
            this.Buffer = bytes;
            this.Offset = offset;
            this.Count = length;
        }

        internal void OnCompleted(SocketAsyncEventArgs args)
        {
            var cb = this.Completed;
            if (cb != null)
            {
                cb(this, this);
            }
        }
    }
}

#endif
}