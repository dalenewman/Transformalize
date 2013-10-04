#region License
// /*
// See license included in this library folder.
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