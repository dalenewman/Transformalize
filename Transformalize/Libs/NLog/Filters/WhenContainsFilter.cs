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
using System.ComponentModel;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Filters
{
    /// <summary>
    ///     Matches when the calculated layout contains the specified substring.
    ///     This filter is deprecated in favour of <c>&lt;when /&gt;</c> which is based on <a href="conditions.html">contitions</a>.
    /// </summary>
    [Filter("whenContains")]
    public class WhenContainsFilter : LayoutBasedFilter
    {
        /// <summary>
        ///     Gets or sets a value indicating whether to ignore case when comparing strings.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [DefaultValue(false)]
        public bool IgnoreCase { get; set; }

        /// <summary>
        ///     Gets or sets the substring to be matched.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public string Substring { get; set; }

        /// <summary>
        ///     Checks whether log event should be logged or not.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        /// <returns>
        ///     <see cref="FilterResult.Ignore" /> - if the log event should be ignored<br />
        ///     <see cref="FilterResult.Neutral" /> - if the filter doesn't want to decide<br />
        ///     <see cref="FilterResult.Log" /> - if the log event should be logged<br />
        ///     .
        /// </returns>
        protected override FilterResult Check(LogEventInfo logEvent)
        {
            var comparisonType = IgnoreCase
                                     ? StringComparison.OrdinalIgnoreCase
                                     : StringComparison.Ordinal;

            if (Layout.Render(logEvent).IndexOf(Substring, comparisonType) >= 0)
            {
                return Action;
            }

            return FilterResult.Neutral;
        }
    }
}