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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Xml;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     XML event description compatible with log4j, Chainsaw and NLogViewer.
    /// </summary>
    [LayoutRenderer("log4jxmlevent")]
    public class Log4JXmlEventLayoutRenderer : LayoutRenderer, IUsesStackTrace
    {
        private static readonly DateTime log4jDateBase = new DateTime(1970, 1, 1);

        private static readonly string dummyNamespace = "http://nlog-project.org/dummynamespace/" + Guid.NewGuid();

        private static readonly string dummyNLogNamespace = "http://nlog-project.org/dummynamespace/" + Guid.NewGuid();

        /// <summary>
        ///     Initializes a new instance of the <see cref="Log4JXmlEventLayoutRenderer" /> class.
        /// </summary>
        public Log4JXmlEventLayoutRenderer()
        {
            IncludeNLogData = true;
            NdcItemSeparator = " ";
#if NET_CF
            this.AppInfo = ".NET CF Application";
#elif SILVERLIGHT
            this.AppInfo = "Silverlight Application";
#else
            AppInfo = string.Format(
                CultureInfo.InvariantCulture,
                "{0}({1})",
                AppDomain.CurrentDomain.FriendlyName,
                ThreadIDHelper.Instance.CurrentProcessID);
#endif
            Parameters = new List<NLogViewerParameterInfo>();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to include NLog-specific extensions to log4j schema.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        [DefaultValue(true)]
        public bool IncludeNLogData { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the XML should use spaces for indentation.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IndentXml { get; set; }

        /// <summary>
        ///     Gets or sets the AppInfo field. By default it's the friendly name of the current AppDomain.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public string AppInfo { get; set; }

#if !NET_CF
        /// <summary>
        ///     Gets or sets a value indicating whether to include call site (class and method name) in the information sent over the network.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeCallSite { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include source info (file name and line number) in the information sent over the network.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeSourceInfo { get; set; }
#endif

        /// <summary>
        ///     Gets or sets a value indicating whether to include contents of the <see cref="MappedDiagnosticsContext" /> dictionary.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeMdc { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to include contents of the <see cref="NestedDiagnosticsContext" /> stack.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        public bool IncludeNdc { get; set; }

        /// <summary>
        ///     Gets or sets the NDC item separator.
        /// </summary>
        /// <docgen category='Payload Options' order='10' />
        [DefaultValue(" ")]
        public string NdcItemSeparator { get; set; }

        /// <summary>
        ///     Gets the level of stack trace information required by the implementing class.
        /// </summary>
        StackTraceUsage IUsesStackTrace.StackTraceUsage
        {
            get
            {
#if !NET_CF
                if (IncludeSourceInfo)
                {
                    return StackTraceUsage.Max;
                }

                if (IncludeCallSite)
                {
                    return StackTraceUsage.WithoutSource;
                }
#endif

                return StackTraceUsage.None;
            }
        }

        internal IList<NLogViewerParameterInfo> Parameters { get; set; }

        internal void AppendToStringBuilder(StringBuilder sb, LogEventInfo logEvent)
        {
            Append(sb, logEvent);
        }

        /// <summary>
        ///     Renders the XML logging event and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var settings = new XmlWriterSettings
                               {
                                   Indent = IndentXml,
                                   ConformanceLevel = ConformanceLevel.Fragment,
                                   IndentChars = "  ",
                               };

            var sb = new StringBuilder();
            using (var xtw = XmlWriter.Create(sb, settings))
            {
                xtw.WriteStartElement("log4j", "event", dummyNamespace);
                xtw.WriteAttributeString("xmlns", "nlog", null, dummyNLogNamespace);
                xtw.WriteAttributeString("logger", logEvent.LoggerName);
                xtw.WriteAttributeString("level", logEvent.Level.Name.ToUpper(CultureInfo.InvariantCulture));
                xtw.WriteAttributeString("timestamp", Convert.ToString((long) (logEvent.TimeStamp.ToUniversalTime() - log4jDateBase).TotalMilliseconds, CultureInfo.InvariantCulture));
                xtw.WriteAttributeString("thread", Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));

                xtw.WriteElementString("log4j", "message", dummyNamespace, logEvent.FormattedMessage);
                if (IncludeNdc)
                {
                    xtw.WriteElementString("log4j", "NDC", dummyNamespace, string.Join(NdcItemSeparator, NestedDiagnosticsContext.GetAllMessages()));
                }

                if (logEvent.Exception != null)
                {
                    xtw.WriteStartElement("log4j", "throwable", dummyNamespace);
                    xtw.WriteCData(logEvent.Exception.ToString());
                    xtw.WriteEndElement();
                }

#if !NET_CF
                if (IncludeCallSite || IncludeSourceInfo)
                {
                    var frame = logEvent.UserStackFrame;
                    var methodBase = frame.GetMethod();
                    var type = methodBase.DeclaringType;

                    xtw.WriteStartElement("log4j", "locationInfo", dummyNamespace);
                    if (type != null)
                    {
                        xtw.WriteAttributeString("class", type.FullName);
                    }

                    xtw.WriteAttributeString("method", methodBase.ToString());
#if !SILVERLIGHT
                    if (IncludeSourceInfo)
                    {
                        xtw.WriteAttributeString("file", frame.GetFileName());
                        xtw.WriteAttributeString("line", frame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture));
                    }
#endif
                    xtw.WriteEndElement();

                    if (IncludeNLogData)
                    {
                        xtw.WriteElementString("nlog", "eventSequenceNumber", dummyNLogNamespace, logEvent.SequenceID.ToString(CultureInfo.InvariantCulture));
                        xtw.WriteStartElement("nlog", "locationInfo", dummyNLogNamespace);
                        if (type != null)
                        {
                            xtw.WriteAttributeString("assembly", type.Assembly.FullName);
                        }

                        xtw.WriteEndElement();
                    }
                }
#endif

                xtw.WriteStartElement("log4j", "properties", dummyNamespace);
                if (IncludeMdc)
                {
                    foreach (var entry in MappedDiagnosticsContext.ThreadDictionary)
                    {
                        xtw.WriteStartElement("log4j", "data", dummyNamespace);
                        xtw.WriteAttributeString("name", entry.Key);
                        xtw.WriteAttributeString("value", entry.Value);
                        xtw.WriteEndElement();
                    }
                }

                foreach (var parameter in Parameters)
                {
                    xtw.WriteStartElement("log4j", "data", dummyNamespace);
                    xtw.WriteAttributeString("name", parameter.Name);
                    xtw.WriteAttributeString("value", parameter.Layout.Render(logEvent));
                    xtw.WriteEndElement();
                }

                xtw.WriteStartElement("log4j", "data", dummyNamespace);
                xtw.WriteAttributeString("name", "log4japp");
                xtw.WriteAttributeString("value", AppInfo);
                xtw.WriteEndElement();

                xtw.WriteStartElement("log4j", "data", dummyNamespace);
                xtw.WriteAttributeString("name", "log4jmachinename");
#if NET_CF
            xtw.WriteAttributeString("value", "netcf");
#elif SILVERLIGHT
            xtw.WriteAttributeString("value", "silverlight");
#else
                xtw.WriteAttributeString("value", Environment.MachineName);
#endif
                xtw.WriteEndElement();
                xtw.WriteEndElement();

                xtw.WriteEndElement();
                xtw.Flush();

                // get rid of 'nlog' and 'log4j' namespace declarations
                sb.Replace(" xmlns:log4j=\"" + dummyNamespace + "\"", string.Empty);
                sb.Replace(" xmlns:nlog=\"" + dummyNLogNamespace + "\"", string.Empty);

                builder.Append(sb);
            }
        }
    }
}