#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.ComponentModel;
using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     The row-highlighting condition.
    /// </summary>
    [NLogConfigurationItem]
    public class ConsoleRowHighlightingRule
    {
        /// <summary>
        ///     Initializes static members of the ConsoleRowHighlightingRule class.
        /// </summary>
        static ConsoleRowHighlightingRule()
        {
            Default = new ConsoleRowHighlightingRule(null, ConsoleOutputColor.NoChange, ConsoleOutputColor.NoChange);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleRowHighlightingRule" /> class.
        /// </summary>
        public ConsoleRowHighlightingRule()
            : this(null, ConsoleOutputColor.NoChange, ConsoleOutputColor.NoChange)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConsoleRowHighlightingRule" /> class.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public ConsoleRowHighlightingRule(ConditionExpression condition, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor)
        {
            Condition = condition;
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

        /// <summary>
        ///     Gets the default highlighting rule. Doesn't change the color.
        /// </summary>
        public static ConsoleRowHighlightingRule Default { get; private set; }

        /// <summary>
        ///     Gets or sets the condition that must be met in order to set the specified foreground and background color.
        /// </summary>
        /// <docgen category='Rule Matching Options' order='10' />
        [RequiredParameter]
        public ConditionExpression Condition { get; set; }

        /// <summary>
        ///     Gets or sets the foreground color.
        /// </summary>
        /// <docgen category='Formatting Options' order='10' />
        [DefaultValue("NoChange")]
        public ConsoleOutputColor ForegroundColor { get; set; }

        /// <summary>
        ///     Gets or sets the background color.
        /// </summary>
        /// <docgen category='Formatting Options' order='10' />
        [DefaultValue("NoChange")]
        public ConsoleOutputColor BackgroundColor { get; set; }

        /// <summary>
        ///     Checks whether the specified log event matches the condition (if any).
        /// </summary>
        /// <param name="logEvent">
        ///     Log event.
        /// </param>
        /// <returns>
        ///     A value of <see langword="true" /> if the condition is not defined or
        ///     if it matches, <see langword="false" /> otherwise.
        /// </returns>
        public bool CheckCondition(LogEventInfo logEvent)
        {
            if (Condition == null)
            {
                return true;
            }

            return true.Equals(Condition.Evaluate(logEvent));
        }
    }
}

#endif