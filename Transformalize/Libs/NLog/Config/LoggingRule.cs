#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Transformalize.Libs.NLog.Filters;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Represents a logging rule. An equivalent of &lt;logger /&gt; configuration element.
    /// </summary>
    [NLogConfigurationItem]
    public class LoggingRule
    {
        private readonly bool[] logLevels = new bool[LogLevel.MaxLevel.Ordinal + 1];

        private string loggerNameMatchArgument;
        private MatchMode loggerNameMatchMode;
        private string loggerNamePattern;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingRule" /> class.
        /// </summary>
        public LoggingRule()
        {
            Filters = new List<Filter>();
            ChildRules = new List<LoggingRule>();
            Targets = new List<Target>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingRule" /> class.
        /// </summary>
        /// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
        /// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
        /// <param name="target">Target to be written to when the rule matches.</param>
        public LoggingRule(string loggerNamePattern, LogLevel minLevel, Target target)
        {
            Filters = new List<Filter>();
            ChildRules = new List<LoggingRule>();
            Targets = new List<Target>();
            LoggerNamePattern = loggerNamePattern;
            Targets.Add(target);
            for (var i = minLevel.Ordinal; i <= LogLevel.MaxLevel.Ordinal; ++i)
            {
                EnableLoggingForLevel(LogLevel.FromOrdinal(i));
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingRule" /> class.
        /// </summary>
        /// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
        /// <param name="target">Target to be written to when the rule matches.</param>
        /// <remarks>
        ///     By default no logging levels are defined. You should call <see cref="EnableLoggingForLevel" /> and
        ///     <see
        ///         cref="DisableLoggingForLevel" />
        ///     to set them.
        /// </remarks>
        public LoggingRule(string loggerNamePattern, Target target)
        {
            Filters = new List<Filter>();
            ChildRules = new List<LoggingRule>();
            Targets = new List<Target>();
            LoggerNamePattern = loggerNamePattern;
            Targets.Add(target);
        }

        /// <summary>
        ///     Gets a collection of targets that should be written to when this rule matches.
        /// </summary>
        public IList<Target> Targets { get; private set; }

        /// <summary>
        ///     Gets a collection of child rules to be evaluated when this rule matches.
        /// </summary>
        public IList<LoggingRule> ChildRules { get; private set; }

        /// <summary>
        ///     Gets a collection of filters to be checked before writing to targets.
        /// </summary>
        public IList<Filter> Filters { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to quit processing any further rule when this one matches.
        /// </summary>
        public bool Final { get; set; }

        /// <summary>
        ///     Gets or sets logger name pattern.
        /// </summary>
        /// <remarks>
        ///     Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends but not anywhere else.
        /// </remarks>
        public string LoggerNamePattern
        {
            get { return loggerNamePattern; }

            set
            {
                loggerNamePattern = value;
                var firstPos = loggerNamePattern.IndexOf('*');
                var lastPos = loggerNamePattern.LastIndexOf('*');

                if (firstPos < 0)
                {
                    loggerNameMatchMode = MatchMode.Equals;
                    loggerNameMatchArgument = value;
                    return;
                }

                if (firstPos == lastPos)
                {
                    var before = LoggerNamePattern.Substring(0, firstPos);
                    var after = LoggerNamePattern.Substring(firstPos + 1);

                    if (before.Length > 0)
                    {
                        loggerNameMatchMode = MatchMode.StartsWith;
                        loggerNameMatchArgument = before;
                        return;
                    }

                    if (after.Length > 0)
                    {
                        loggerNameMatchMode = MatchMode.EndsWith;
                        loggerNameMatchArgument = after;
                        return;
                    }

                    return;
                }

                // *text*
                if (firstPos == 0 && lastPos == LoggerNamePattern.Length - 1)
                {
                    var text = LoggerNamePattern.Substring(1, LoggerNamePattern.Length - 2);
                    loggerNameMatchMode = MatchMode.Contains;
                    loggerNameMatchArgument = text;
                    return;
                }

                loggerNameMatchMode = MatchMode.None;
                loggerNameMatchArgument = string.Empty;
            }
        }

        /// <summary>
        ///     Gets the collection of log levels enabled by this rule.
        /// </summary>
        public ReadOnlyCollection<LogLevel> Levels
        {
            get
            {
                var levels = new List<LogLevel>();

                for (var i = LogLevel.MinLevel.Ordinal; i <= LogLevel.MaxLevel.Ordinal; ++i)
                {
                    if (logLevels[i])
                    {
                        levels.Add(LogLevel.FromOrdinal(i));
                    }
                }

                return levels.AsReadOnly();
            }
        }

        /// <summary>
        ///     Enables logging for a particular level.
        /// </summary>
        /// <param name="level">Level to be enabled.</param>
        public void EnableLoggingForLevel(LogLevel level)
        {
            logLevels[level.Ordinal] = true;
        }

        /// <summary>
        ///     Disables logging for a particular level.
        /// </summary>
        /// <param name="level">Level to be disabled.</param>
        public void DisableLoggingForLevel(LogLevel level)
        {
            logLevels[level.Ordinal] = false;
        }

        /// <summary>
        ///     Returns a string representation of <see cref="LoggingRule" />. Used for debugging.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(CultureInfo.InvariantCulture, "logNamePattern: ({0}:{1})", loggerNameMatchArgument, loggerNameMatchMode);
            sb.Append(" levels: [ ");
            for (var i = 0; i < logLevels.Length; ++i)
            {
                if (logLevels[0])
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0} ", LogLevel.FromOrdinal(i));
                }
            }

            sb.Append("] appendTo: [ ");
            foreach (var app in Targets)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0} ", app.Name);
            }

            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        ///     Checks whether te particular log level is enabled for this rule.
        /// </summary>
        /// <param name="level">Level to be checked.</param>
        /// <returns>
        ///     A value of <see langword="true" /> when the log level is enabled, <see langword="false" /> otherwise.
        /// </returns>
        public bool IsLoggingEnabledForLevel(LogLevel level)
        {
            return logLevels[level.Ordinal];
        }

        /// <summary>
        ///     Checks whether given name matches the logger name pattern.
        /// </summary>
        /// <param name="loggerName">String to be matched.</param>
        /// <returns>
        ///     A value of <see langword="true" /> when the name matches, <see langword="false" /> otherwise.
        /// </returns>
        public bool NameMatches(string loggerName)
        {
            switch (loggerNameMatchMode)
            {
                case MatchMode.All:
                    return true;

                default:
                case MatchMode.None:
                    return false;

                case MatchMode.Equals:
                    return loggerName.Equals(loggerNameMatchArgument, StringComparison.Ordinal);

                case MatchMode.StartsWith:
                    return loggerName.StartsWith(loggerNameMatchArgument, StringComparison.Ordinal);

                case MatchMode.EndsWith:
                    return loggerName.EndsWith(loggerNameMatchArgument, StringComparison.Ordinal);

                case MatchMode.Contains:
                    return loggerName.IndexOf(loggerNameMatchArgument, StringComparison.Ordinal) >= 0;
            }
        }

        internal enum MatchMode
        {
            All,
            None,
            Equals,
            StartsWith,
            EndsWith,
            Contains,
        }
    }
}