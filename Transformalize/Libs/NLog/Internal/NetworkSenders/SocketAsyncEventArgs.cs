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