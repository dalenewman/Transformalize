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

namespace Transformalize.Libs.NLog.LogReceiverService
{
#if WCF_SUPPORTED && !SILVERLIGHT && !NET_CF

namespace NLog.LogReceiverService
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of <see cref="ILogReceiverServer" /> which forwards received logs through <see cref="LogManager"/> or a given <see cref="LogFactory"/>.
    /// </summary>
    public class LogReceiverForwardingService : ILogReceiverServer
    {
        private readonly LogFactory logFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReceiverForwardingService"/> class.
        /// </summary>
        public LogReceiverForwardingService()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReceiverForwardingService"/> class.
        /// </summary>
        /// <param name="logFactory">The log factory.</param>
        public LogReceiverForwardingService(LogFactory logFactory)
        {
            this.logFactory = logFactory;
        }

        /// <summary>
        /// Processes the log messages.
        /// </summary>
        /// <param name="events">The events to process.</param>
        public void ProcessLogMessages(NLogEvents events)
        {
            var baseTimeUtc = new DateTime(events.BaseTimeUtc, DateTimeKind.Utc);
            var logEvents = new LogEventInfo[events.Events.Length];

            // convert transport representation of log events into workable LogEventInfo[]
            for (int j = 0; j < events.Events.Length; ++j)
            {
                var ev = events.Events[j];
                LogLevel level = LogLevel.FromOrdinal(ev.LevelOrdinal);
                string loggerName = events.Strings[ev.LoggerOrdinal];

                var logEventInfo = new LogEventInfo();
                logEventInfo.Level = level;
                logEventInfo.LoggerName = loggerName;
                logEventInfo.TimeStamp = baseTimeUtc.AddTicks(ev.TimeDelta);
                logEventInfo.Message = events.Strings[ev.MessageOrdinal];
                logEventInfo.Properties.Add("ClientName", events.ClientName);
                for (int i = 0; i < events.LayoutNames.Count; ++i)
                {
                    logEventInfo.Properties.Add(events.LayoutNames[i], events.Strings[ev.ValueIndexes[i]]);
                }

                logEvents[j] = logEventInfo;
            }

            this.ProcessLogMessages(logEvents);
        }

        /// <summary>
        /// Processes the log messages.
        /// </summary>
        /// <param name="logEvents">The log events.</param>
        protected virtual void ProcessLogMessages(LogEventInfo[] logEvents)
        {
            Logger logger = null;
            string lastLoggerName = string.Empty;

            foreach (var ev in logEvents)
            {
                if (ev.LoggerName != lastLoggerName)
                {
                    if (this.logFactory != null)
                    {
                        logger = this.logFactory.GetLogger(ev.LoggerName);
                    }
                    else
                    {
                        logger = LogManager.GetLogger(ev.LoggerName);
                    }

                    lastLoggerName = ev.LoggerName;
                }

                logger.Log(ev);
            }
        }
    }
}

#endif
}