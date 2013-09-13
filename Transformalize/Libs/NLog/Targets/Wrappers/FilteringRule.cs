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

using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Filtering rule for <see cref="PostFilteringTargetWrapper" />.
    /// </summary>
    [NLogConfigurationItem]
    public class FilteringRule
    {
        /// <summary>
        ///     Initializes a new instance of the FilteringRule class.
        /// </summary>
        public FilteringRule()
            : this(null, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the FilteringRule class.
        /// </summary>
        /// <param name="whenExistsExpression">Condition to be tested against all events.</param>
        /// <param name="filterToApply">Filter to apply to all log events when the first condition matches any of them.</param>
        public FilteringRule(ConditionExpression whenExistsExpression, ConditionExpression filterToApply)
        {
            Exists = whenExistsExpression;
            Filter = filterToApply;
        }

        /// <summary>
        ///     Gets or sets the condition to be tested.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public ConditionExpression Exists { get; set; }

        /// <summary>
        ///     Gets or sets the resulting filter to be applied when the condition matches.
        /// </summary>
        /// <docgen category='Filtering Options' order='10' />
        [RequiredParameter]
        public ConditionExpression Filter { get; set; }
    }
}