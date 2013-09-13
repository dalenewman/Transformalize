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
using System.Globalization;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition literal expression (numeric, <b>LogLevel.XXX</b>, <b>true</b> or <b>false</b>).
    /// </summary>
    internal sealed class ConditionLiteralExpression : ConditionExpression
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionLiteralExpression" /> class.
        /// </summary>
        /// <param name="literalValue">Literal value.</param>
        public ConditionLiteralExpression(object literalValue)
        {
            LiteralValue = literalValue;
        }

        /// <summary>
        ///     Gets the literal value.
        /// </summary>
        /// <value>The literal value.</value>
        public object LiteralValue { get; private set; }

        /// <summary>
        ///     Returns a string representation of the expression.
        /// </summary>
        /// <returns>The literal value.</returns>
        public override string ToString()
        {
            if (LiteralValue == null)
            {
                return "null";
            }

            return Convert.ToString(LiteralValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Evaluates the expression.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>The literal value as passed in the constructor.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            return LiteralValue;
        }
    }
}