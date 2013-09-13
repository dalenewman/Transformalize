// 
// Copyright (c) 2004-2011 Jaroslaw Kowalski <jaak@jkowalski.net>
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Internal.NetworkSenders;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Sends log messages over the network.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Network_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Network/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Network/Simple/Example.cs" />
    ///     <p>
    ///         To print the results, use any application that's able to receive messages over
    ///         TCP or UDP. <a href="http://m.nu/program/util/netcat/netcat.html">NetCat</a> is
    ///         a simple but very powerful command-line tool that can be used for that. This image
    ///         demonstrates the NetCat tool receiving log messages from Network target.
    ///     </p>
    ///     <img src="examples/targets/Screenshots/Network/Output.gif" />
    ///     <p>
    ///         NOTE: If your receiver application is ever likely to be off-line, don't use TCP protocol
    ///         or you'll get TCP timeouts and your application will be very slow.
    ///         Either switch to UDP transport or use <a href="target.AsyncWrapper.html">AsyncWrapper</a> target
    ///         so that your application threads will not be blocked by the timing-out connection attempts.
    ///     </p>
    ///     <p>
    ///         There are two specialized versions of the Network target: <a href="target.Chainsaw.html">Chainsaw</a>
    ///         and <a href="target.NLogViewer.html">NLogViewer</a> which write to instances of Chainsaw log4j viewer
    ///         or NLogViewer application respectively.
    ///     </p>
    /// </example>
    [Target("Network")]
    public class NetworkTarget : TargetWithLayout
    {
        private readonly Dictionary<string, NetworkSender> currentSenderCache = new Dictionary<string, NetworkSender>();
        private readonly List<NetworkSender> openNetworkSenders = new List<NetworkSender>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkTarget" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        public NetworkTarget()
        {
            SenderFactory = NetworkSenderFactory.Default;
            Encoding = Encoding.UTF8;
            OnOverflow = NetworkTargetOverflowAction.Split;
            KeepConnection = true;
            MaxMessageSize = 65000;
            ConnectionCacheSize = 5;
        }

        /// <summary>
        ///     Gets or sets the network address.
        /// </summary>
        /// <remarks>
        ///     The network address can be:
        ///     <ul>
        ///         <li>tcp://host:port - TCP (auto select IPv4/IPv6) (not supported on Windows Phone 7.0)</li>
        ///         <li>tcp4://host:port - force TCP/IPv4 (not supported on Windows Phone 7.0)</li>
        ///         <li>tcp6://host:port - force TCP/IPv6 (not supported on Windows Phone 7.0)</li>
        ///         <li>udp://host:port - UDP (auto select IPv4/IPv6, not supported on Silverlight and on Windows Phone 7.0)</li>
        ///         <li>udp4://host:port - force UDP/IPv4 (not supported on Silverlight and on Windows Phone 7.0)</li>
        ///         <li>udp6://host:port - force UDP/IPv6  (not supported on Silverlight and on Windows Phone 7.0)</li>
        ///         <li>http://host:port/pageName - HTTP using POST verb</li>
        ///         <li>https://host:port/pageName - HTTPS using POST verb</li>
        ///     </ul>
        ///     For SOAP-based webservice support over HTTP use WebService target.
        /// </remarks>
        /// <docgen category='Connection Options' order='10' />
        public Layout Address { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to keep connection open whenever possible.
        /// </summary>
        /// <docgen category='Connection Options' order='10' />
        [DefaultValue(true)]
        public bool KeepConnection { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to append newline at the end of log message.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [DefaultValue(false)]
        public bool NewLine { get; set; }

        /// <summary>
        ///     Gets or sets the maximum message size in bytes.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [DefaultValue(65000)]
        public int MaxMessageSize { get; set; }

        /// <summary>
        ///     Gets or sets the size of the connection cache (number of connections which are kept alive).
        /// </summary>
        /// <docgen category="Connection Options" order="10" />
        [DefaultValue(5)]
        public int ConnectionCacheSize { get; set; }

        /// <summary>
        ///     Gets or sets the action that should be taken if the message is larger than
        ///     maxMessageSize.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        public NetworkTargetOverflowAction OnOverflow { get; set; }

        /// <summary>
        ///     Gets or sets the encoding to be used.
        /// </summary>
        /// <docgen category='Layout Options' order='10' />
        [DefaultValue("utf-8")]
        public Encoding Encoding { get; set; }

        internal INetworkSenderFactory SenderFactory { get; set; }

        /// <summary>
        ///     Flush any pending log messages asynchronously (in case of asynchronous targets).
        /// </summary>
        /// <param name="asyncContinuation">The asynchronous continuation.</param>
        protected override void FlushAsync(AsyncContinuation asyncContinuation)
        {
            int remainingCount = 0;

            AsyncContinuation continuation =
                ex =>
                    {
                        // ignore exception
                        if (Interlocked.Decrement(ref remainingCount) == 0)
                        {
                            asyncContinuation(null);
                        }
                    };

            lock (openNetworkSenders)
            {
                remainingCount = openNetworkSenders.Count;
                if (remainingCount == 0)
                {
                    // nothing to flush
                    asyncContinuation(null);
                }
                else
                {
                    // otherwise call FlushAsync() on all senders
                    // and invoke continuation at the very end
                    foreach (NetworkSender openSender in openNetworkSenders)
                    {
                        openSender.FlushAsync(continuation);
                    }
                }
            }
        }

        /// <summary>
        ///     Closes the target.
        /// </summary>
        protected override void CloseTarget()
        {
            base.CloseTarget();

            lock (openNetworkSenders)
            {
                foreach (NetworkSender openSender in openNetworkSenders)
                {
                    openSender.Close(ex => { });
                }

                openNetworkSenders.Clear();
            }
        }

        /// <summary>
        ///     Sends the
        ///     rendered logging event over the network optionally concatenating it with a newline character.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(AsyncLogEventInfo logEvent)
        {
            string address = Address.Render(logEvent.LogEvent);
            byte[] bytes = GetBytesToWrite(logEvent.LogEvent);

            if (KeepConnection)
            {
                NetworkSender sender = GetCachedNetworkSender(address);

                ChunkedSend(
                    sender,
                    bytes,
                    ex =>
                        {
                            if (ex != null)
                            {
                                InternalLogger.Error("Error when sending {0}", ex);
                                ReleaseCachedConnection(sender);
                            }

                            logEvent.Continuation(ex);
                        });
            }
            else
            {
                NetworkSender sender = SenderFactory.Create(address);
                sender.Initialize();

                lock (openNetworkSenders)
                {
                    openNetworkSenders.Add(sender);
                    ChunkedSend(
                        sender,
                        bytes,
                        ex =>
                            {
                                lock (openNetworkSenders)
                                {
                                    openNetworkSenders.Remove(sender);
                                }

                                if (ex != null)
                                {
                                    InternalLogger.Error("Error when sending {0}", ex);
                                }

                                sender.Close(ex2 => { });
                                logEvent.Continuation(ex);
                            });
                }
            }
        }

        /// <summary>
        ///     Gets the bytes to be written.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        /// <returns>Byte array.</returns>
        protected virtual byte[] GetBytesToWrite(LogEventInfo logEvent)
        {
            string text;

            if (NewLine)
            {
                text = Layout.Render(logEvent) + "\r\n";
            }
            else
            {
                text = Layout.Render(logEvent);
            }

            return Encoding.GetBytes(text);
        }

        private NetworkSender GetCachedNetworkSender(string address)
        {
            lock (currentSenderCache)
            {
                NetworkSender sender;

                // already have address
                if (currentSenderCache.TryGetValue(address, out sender))
                {
                    return sender;
                }

                if (currentSenderCache.Count >= ConnectionCacheSize)
                {
                    // make room in the cache by closing the least recently used connection
                    int minAccessTime = int.MaxValue;
                    NetworkSender leastRecentlyUsed = null;

                    foreach (var kvp in currentSenderCache)
                    {
                        if (kvp.Value.LastSendTime < minAccessTime)
                        {
                            minAccessTime = kvp.Value.LastSendTime;
                            leastRecentlyUsed = kvp.Value;
                        }
                    }

                    if (leastRecentlyUsed != null)
                    {
                        ReleaseCachedConnection(leastRecentlyUsed);
                    }
                }

                sender = SenderFactory.Create(address);
                sender.Initialize();
                lock (openNetworkSenders)
                {
                    openNetworkSenders.Add(sender);
                }

                currentSenderCache.Add(address, sender);
                return sender;
            }
        }

        private void ReleaseCachedConnection(NetworkSender sender)
        {
            lock (currentSenderCache)
            {
                lock (openNetworkSenders)
                {
                    if (openNetworkSenders.Remove(sender))
                    {
                        // only remove it once
                        sender.Close(ex => { });
                    }
                }

                NetworkSender sender2;

                // make sure the current sender for this address is the one we want to remove
                if (currentSenderCache.TryGetValue(sender.Address, out sender2))
                {
                    if (ReferenceEquals(sender, sender2))
                    {
                        currentSenderCache.Remove(sender.Address);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "Using property names in message.")]
        private void ChunkedSend(NetworkSender sender, byte[] buffer, AsyncContinuation continuation)
        {
            int tosend = buffer.Length;
            int pos = 0;

            AsyncContinuation sendNextChunk = null;

            sendNextChunk = ex =>
                                {
                                    if (ex != null)
                                    {
                                        continuation(ex);
                                        return;
                                    }

                                    if (tosend <= 0)
                                    {
                                        continuation(null);
                                        return;
                                    }

                                    int chunksize = tosend;
                                    if (chunksize > MaxMessageSize)
                                    {
                                        if (OnOverflow == NetworkTargetOverflowAction.Discard)
                                        {
                                            continuation(null);
                                            return;
                                        }

                                        if (OnOverflow == NetworkTargetOverflowAction.Error)
                                        {
                                            continuation(new OverflowException("Attempted to send a message larger than MaxMessageSize (" + MaxMessageSize + "). Actual size was: " + buffer.Length + ". Adjust OnOverflow and MaxMessageSize parameters accordingly."));
                                            return;
                                        }

                                        chunksize = MaxMessageSize;
                                    }

                                    int pos0 = pos;
                                    tosend -= chunksize;
                                    pos += chunksize;

                                    sender.Send(buffer, pos0, chunksize, sendNextChunk);
                                };

            sendNextChunk(null);
        }
    }
}