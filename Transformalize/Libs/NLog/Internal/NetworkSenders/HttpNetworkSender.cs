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
using System.Net;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Internal.NetworkSenders
{
    /// <summary>
    ///     Network sender which uses HTTP or HTTPS POST.
    /// </summary>
    internal class HttpNetworkSender : NetworkSender
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpNetworkSender" /> class.
        /// </summary>
        /// <param name="url">The network URL.</param>
        public HttpNetworkSender(string url)
            : base(url)
        {
        }

        /// <summary>
        ///     Actually sends the given text over the specified protocol.
        /// </summary>
        /// <param name="bytes">The bytes to be sent.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <param name="length">Number of bytes to send.</param>
        /// <param name="asyncContinuation">The async continuation to be invoked after the buffer has been sent.</param>
        /// <remarks>To be overridden in inheriting classes.</remarks>
        protected override void DoSend(byte[] bytes, int offset, int length, AsyncContinuation asyncContinuation)
        {
            var webRequest = WebRequest.Create(new Uri(Address));
            webRequest.Method = "POST";

            AsyncCallback onResponse =
                r =>
                    {
                        try
                        {
                            using (var response = webRequest.EndGetResponse(r))
                            {
                            }

                            // completed fine
                            asyncContinuation(null);
                        }
                        catch (Exception ex)
                        {
                            if (ex.MustBeRethrown())
                            {
                                throw;
                            }

                            asyncContinuation(ex);
                        }
                    };

            AsyncCallback onRequestStream =
                r =>
                    {
                        try
                        {
                            using (var stream = webRequest.EndGetRequestStream(r))
                            {
                                stream.Write(bytes, offset, length);
                            }

                            webRequest.BeginGetResponse(onResponse, null);
                        }
                        catch (Exception ex)
                        {
                            if (ex.MustBeRethrown())
                            {
                                throw;
                            }

                            asyncContinuation(ex);
                        }
                    };

            webRequest.BeginGetRequestStream(onRequestStream, null);
        }
    }
}