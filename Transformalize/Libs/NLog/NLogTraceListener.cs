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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;
using Transformalize.Libs.NLog.Internal;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog
{
    /// <summary>
    ///     TraceListener which routes all messages through NLog.
    /// </summary>
    public class NLogTraceListener : TraceListener
    {
        private static readonly Assembly systemAssembly = typeof (Trace).Assembly;
        private LogFactory logFactory;
        private LogLevel defaultLogLevel = LogLevel.Debug;
        private bool attributesLoaded;
#if !NET_CF
        private bool autoLoggerName;
#endif
        private LogLevel forceLogLevel;

        /// <summary>
        ///     Gets or sets the log factory to use when outputting messages (null - use LogManager).
        /// </summary>
        public LogFactory LogFactory
        {
            get
            {
                InitAttributes();
                return logFactory;
            }

            set
            {
                attributesLoaded = true;
                logFactory = value;
            }
        }

        /// <summary>
        ///     Gets or sets the default log level.
        /// </summary>
        public LogLevel DefaultLogLevel
        {
            get
            {
                InitAttributes();
                return defaultLogLevel;
            }

            set
            {
                attributesLoaded = true;
                defaultLogLevel = value;
            }
        }

        /// <summary>
        ///     Gets or sets the log which should be always used regardless of source level.
        /// </summary>
        public LogLevel ForceLogLevel
        {
            get
            {
                InitAttributes();
                return forceLogLevel;
            }

            set
            {
                attributesLoaded = true;
                forceLogLevel = value;
            }
        }

#if !NET_CF
        /// <summary>
        ///     Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        /// <value></value>
        /// <returns>true if the trace listener is thread safe; otherwise, false. The default is false.</returns>
        public override bool IsThreadSafe
        {
            get { return true; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to use auto logger name detected from the stack trace.
        /// </summary>
        public bool AutoLoggerName
        {
            get
            {
                InitAttributes();
                return autoLoggerName;
            }

            set
            {
                attributesLoaded = true;
                autoLoggerName = value;
            }
        }
#endif

        /// <summary>
        ///     When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            ProcessLogEventInfo(DefaultLogLevel, null, message, null, null);
        }

        /// <summary>
        ///     When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            ProcessLogEventInfo(DefaultLogLevel, null, message, null, null);
        }

        /// <summary>
        ///     When overridden in a derived class, closes the output stream so it no longer receives tracing or debugging output.
        /// </summary>
        public override void Close()
        {
        }

        /// <summary>
        ///     Emits an error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        public override void Fail(string message)
        {
            ProcessLogEventInfo(LogLevel.Error, null, message, null, null);
        }

        /// <summary>
        ///     Emits an error message and a detailed error message.
        /// </summary>
        /// <param name="message">A message to emit.</param>
        /// <param name="detailMessage">A detailed message to emit.</param>
        public override void Fail(string message, string detailMessage)
        {
            ProcessLogEventInfo(LogLevel.Error, null, message + " " + detailMessage, null, null);
        }

        /// <summary>
        ///     Flushes the output buffer.
        /// </summary>
        public override void Flush()
        {
            if (LogFactory != null)
            {
                LogFactory.Flush();
            }
            else
            {
                LogManager.Flush();
            }
        }

#if !NET_CF
        /// <summary>
        ///     Writes trace information, a data object and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">
        ///     One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceData(eventCache, source, eventType, id, new[] {data});
        }

        /// <summary>
        ///     Writes trace information, an array of data objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">
        ///     One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">An array of objects to emit as data.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; ++i)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append("{");
                sb.Append(i);
                sb.Append("}");
            }

            ProcessLogEventInfo(TranslateLogLevel(eventType), source, sb.ToString(), data, id);
        }

        /// <summary>
        ///     Writes trace and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">
        ///     One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            ProcessLogEventInfo(TranslateLogLevel(eventType), source, string.Empty, null, id);
        }

        /// <summary>
        ///     Writes trace information, a formatted array of objects and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">
        ///     One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        ///     A format string that contains zero or more format items, which correspond to objects in the <paramref name="args" /> array.
        /// </param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            ProcessLogEventInfo(TranslateLogLevel(eventType), source, format, args, id);
        }

        /// <summary>
        ///     Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">
        ///     One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            ProcessLogEventInfo(TranslateLogLevel(eventType), source, message, null, id);
        }

        /// <summary>
        ///     Writes trace information, a message, a related activity identity and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        ///     A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.
        /// </param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        /// <param name="relatedActivityId">
        ///     A <see cref="T:System.Guid" />  object identifying a related activity.
        /// </param>
        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            ProcessLogEventInfo(LogLevel.Debug, source, message, null, id);
        }

        /// <summary>
        ///     Gets the custom attributes supported by the trace listener.
        /// </summary>
        /// <returns>
        ///     A string array naming the custom attributes supported by the trace listener, or null if there are no custom attributes.
        /// </returns>
        protected override string[] GetSupportedAttributes()
        {
            return new[] {"defaultLogLevel", "autoLoggerName", "forceLogLevel"};
        }

        /// <summary>
        ///     Translates the event type to level from <see cref="TraceEventType" />.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns>Translated log level.</returns>
        private static LogLevel TranslateLogLevel(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Verbose:
                    return LogLevel.Trace;

                case TraceEventType.Information:
                    return LogLevel.Info;

                case TraceEventType.Warning:
                    return LogLevel.Warn;

                case TraceEventType.Error:
                    return LogLevel.Error;

                case TraceEventType.Critical:
                    return LogLevel.Fatal;

                default:
                    return LogLevel.Debug;
            }
        }
#endif

        private void ProcessLogEventInfo(LogLevel logLevel, string loggerName, [Localizable(false)] string message, object[] arguments, int? eventId)
        {
            var ev = new LogEventInfo();

            ev.LoggerName = (loggerName ?? Name) ?? string.Empty;

#if !NET_CF
            if (AutoLoggerName)
            {
                var stack = new StackTrace();
                var userFrameIndex = -1;
                MethodBase userMethod = null;

                for (var i = 0; i < stack.FrameCount; ++i)
                {
                    var frame = stack.GetFrame(i);
                    var method = frame.GetMethod();

                    if (method.DeclaringType == GetType())
                    {
                        // skip all methods of this type
                        continue;
                    }

                    if (method.DeclaringType.Assembly == systemAssembly)
                    {
                        // skip all methods from System.dll
                        continue;
                    }

                    userFrameIndex = i;
                    userMethod = method;
                    break;
                }

                if (userFrameIndex >= 0)
                {
                    ev.SetStackTrace(stack, userFrameIndex);
                    if (userMethod.DeclaringType != null)
                    {
                        ev.LoggerName = userMethod.DeclaringType.FullName;
                    }
                }
            }
#endif

            ev.TimeStamp = CurrentTimeGetter.Now;
            ev.Message = message;
            ev.Parameters = arguments;
            ev.Level = forceLogLevel ?? logLevel;

            if (eventId.HasValue)
            {
                ev.Properties.Add("EventID", eventId.Value);
            }

            var logger = LogManager.GetLogger(ev.LoggerName);
            logger.Log(ev);
        }

        private void InitAttributes()
        {
            if (!attributesLoaded)
            {
                attributesLoaded = true;
#if !NET_CF
                foreach (DictionaryEntry de in Attributes)
                {
                    var key = (string) de.Key;
                    var value = (string) de.Value;

                    switch (key.ToUpperInvariant())
                    {
                        case "DEFAULTLOGLEVEL":
                            defaultLogLevel = LogLevel.FromString(value);
                            break;

                        case "FORCELOGLEVEL":
                            forceLogLevel = LogLevel.FromString(value);
                            break;

                        case "AUTOLOGGERNAME":
                            AutoLoggerName = XmlConvert.ToBoolean(value);
                            break;
                    }
                }
#endif
            }
        }
    }
}

#endif