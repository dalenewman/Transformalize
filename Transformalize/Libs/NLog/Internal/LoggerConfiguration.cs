#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Logger configuration.
    /// </summary>
    internal class LoggerConfiguration
    {
        private readonly TargetWithFilterChain[] targetsByLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggerConfiguration" /> class.
        /// </summary>
        /// <param name="targetsByLevel">The targets by level.</param>
        public LoggerConfiguration(TargetWithFilterChain[] targetsByLevel)
        {
            this.targetsByLevel = targetsByLevel;
        }

        /// <summary>
        ///     Gets targets for the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Chain of targets with attached filters.</returns>
        public TargetWithFilterChain GetTargetsForLevel(LogLevel level)
        {
            return targetsByLevel[level.Ordinal];
        }

        /// <summary>
        ///     Determines whether the specified level is enabled.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>
        ///     A value of <c>true</c> if the specified level is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel level)
        {
            return targetsByLevel[level.Ordinal] != null;
        }
    }
}