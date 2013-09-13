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

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition logger name expression (represented by the <b>logger</b> keyword).
    /// </summary>
    internal sealed class ConditionLoggerNameExpression : ConditionExpression
    {
        /// <summary>
        ///     Returns a string representation of this expression.
        /// </summary>
        /// <returns>
        ///     A <b>logger</b> string.
        /// </returns>
        public override string ToString()
        {
            return "logger";
        }

        /// <summary>
        ///     Evaluates to the logger name.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>The logger name.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            return context.LoggerName;
        }
    }
}