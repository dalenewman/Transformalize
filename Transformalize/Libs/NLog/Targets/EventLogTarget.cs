#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Layouts;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Writes log message to the Event Log.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/EventLog_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/EventLog/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/EventLog/Simple/Example.cs" />
    /// </example>
    [Target("EventLog")]
    public class EventLogTarget : TargetWithLayout, IInstallable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventLogTarget" /> class.
        /// </summary>
        public EventLogTarget()
        {
            Source = AppDomain.CurrentDomain.FriendlyName;
            Log = "Application";
            MachineName = ".";
        }

        /// <summary>
        ///     Gets or sets the name of the machine on which Event Log service is running.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        [DefaultValue(".")]
        public string MachineName { get; set; }

        /// <summary>
        ///     Gets or sets the layout that renders event ID.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        public Layout EventId { get; set; }

        /// <summary>
        ///     Gets or sets the layout that renders event Category.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        public Layout Category { get; set; }

        /// <summary>
        ///     Gets or sets the value to be used as the event Source.
        /// </summary>
        /// <remarks>
        ///     By default this is the friendly name of the current AppDomain.
        /// </remarks>
        /// <docgen category='Event Log Options' order='10' />
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets the name of the Event Log to write to. This can be System, Application or
        ///     any user-defined name.
        /// </summary>
        /// <docgen category='Event Log Options' order='10' />
        [DefaultValue("Application")]
        public string Log { get; set; }

        /// <summary>
        ///     Performs installation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        public void Install(InstallationContext installationContext)
        {
            if (EventLog.SourceExists(Source, MachineName))
            {
                var currentLogName = EventLog.LogNameFromSourceName(Source, MachineName);
                if (currentLogName != Log)
                {
                    // re-create the association between Log and Source
                    EventLog.DeleteEventSource(Source, MachineName);

                    var escd = new EventSourceCreationData(Source, Log)
                                   {
                                       MachineName = MachineName
                                   };

                    EventLog.CreateEventSource(escd);
                }
            }
            else
            {
                var escd = new EventSourceCreationData(Source, Log)
                               {
                                   MachineName = MachineName
                               };

                EventLog.CreateEventSource(escd);
            }
        }

        /// <summary>
        ///     Performs uninstallation which requires administrative permissions.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        public void Uninstall(InstallationContext installationContext)
        {
            EventLog.DeleteEventSource(Source, MachineName);
        }

        /// <summary>
        ///     Determines whether the item is installed.
        /// </summary>
        /// <param name="installationContext">The installation context.</param>
        /// <returns>
        ///     Value indicating whether the item is installed or null if it is not possible to determine.
        /// </returns>
        public bool? IsInstalled(InstallationContext installationContext)
        {
            return EventLog.SourceExists(Source, MachineName);
        }

        /// <summary>
        ///     Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            var s = EventLog.LogNameFromSourceName(Source, MachineName);
            if (s != Log)
            {
                CreateEventSourceIfNeeded();
            }
        }

        /// <summary>
        ///     Writes the specified logging event to the event log.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            var message = Layout.Render(logEvent);
            if (message.Length > 16384)
            {
                // limitation of EventLog API
                message = message.Substring(0, 16384);
            }

            EventLogEntryType entryType;

            if (logEvent.Level >= LogLevel.Error)
            {
                entryType = EventLogEntryType.Error;
            }
            else if (logEvent.Level >= LogLevel.Warn)
            {
                entryType = EventLogEntryType.Warning;
            }
            else
            {
                entryType = EventLogEntryType.Information;
            }

            var eventId = 0;

            if (EventId != null)
            {
                eventId = Convert.ToInt32(EventId.Render(logEvent), CultureInfo.InvariantCulture);
            }

            short category = 0;

            if (Category != null)
            {
                category = Convert.ToInt16(Category.Render(logEvent), CultureInfo.InvariantCulture);
            }

            EventLog.WriteEntry(Source, message, entryType, eventId, category);
        }

        private void CreateEventSourceIfNeeded()
        {
            // if we throw anywhere, we remain non-operational
            try
            {
                if (EventLog.SourceExists(Source, MachineName))
                {
                    var currentLogName = EventLog.LogNameFromSourceName(Source, MachineName);
                    if (currentLogName != Log)
                    {
                        // re-create the association between Log and Source
                        EventLog.DeleteEventSource(Source, MachineName);
                        var escd = new EventSourceCreationData(Source, Log)
                                       {
                                           MachineName = MachineName
                                       };

                        EventLog.CreateEventSource(escd);
                    }
                }
                else
                {
                    var escd = new EventSourceCreationData(Source, Log)
                                   {
                                       MachineName = MachineName
                                   };

                    EventLog.CreateEventSource(escd);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error when connecting to EventLog: {0}", exception);
                throw;
            }
        }
    }
}

#endif