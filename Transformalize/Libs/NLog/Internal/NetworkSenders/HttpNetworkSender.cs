#region License
// /*
// See license included in this library folder.
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