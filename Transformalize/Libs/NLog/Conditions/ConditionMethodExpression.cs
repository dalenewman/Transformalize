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

using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Transformalize.Libs.NLog.Common;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition method invocation expression (represented by <b>method(p1,p2,p3)</b> syntax).
    /// </summary>
    internal sealed class ConditionMethodExpression : ConditionExpression
    {
        private readonly bool acceptsLogEvent;
        private readonly string conditionMethodName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionMethodExpression" /> class.
        /// </summary>
        /// <param name="conditionMethodName">Name of the condition method.</param>
        /// <param name="methodInfo">
        ///     <see cref="MethodInfo" /> of the condition method.
        /// </param>
        /// <param name="methodParameters">The method parameters.</param>
        public ConditionMethodExpression(string conditionMethodName, MethodInfo methodInfo, IEnumerable<ConditionExpression> methodParameters)
        {
            MethodInfo = methodInfo;
            this.conditionMethodName = conditionMethodName;
            MethodParameters = new List<ConditionExpression>(methodParameters).AsReadOnly();

            var formalParameters = MethodInfo.GetParameters();
            if (formalParameters.Length > 0 && formalParameters[0].ParameterType == typeof (LogEventInfo))
            {
                acceptsLogEvent = true;
            }

            var actualParameterCount = MethodParameters.Count;
            if (acceptsLogEvent)
            {
                actualParameterCount++;
            }

            if (formalParameters.Length != actualParameterCount)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Condition method '{0}' expects {1} parameters, but passed {2}.",
                    conditionMethodName,
                    formalParameters.Length,
                    actualParameterCount);

                InternalLogger.Error(message);
                throw new ConditionParseException(message);
            }
        }

        /// <summary>
        ///     Gets the method info.
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        /// <summary>
        ///     Gets the method parameters.
        /// </summary>
        /// <value>The method parameters.</value>
        public IList<ConditionExpression> MethodParameters { get; private set; }

        /// <summary>
        ///     Returns a string representation of the expression.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the condition expression.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(conditionMethodName);
            sb.Append("(");

            var separator = string.Empty;
            foreach (var expr in MethodParameters)
            {
                sb.Append(separator);
                sb.Append(expr);
                separator = ", ";
            }

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        ///     Evaluates the expression.
        /// </summary>
        /// <param name="context">Evaluation context.</param>
        /// <returns>Expression result.</returns>
        protected override object EvaluateNode(LogEventInfo context)
        {
            var parameterOffset = acceptsLogEvent ? 1 : 0;

            var callParameters = new object[MethodParameters.Count + parameterOffset];
            var i = 0;
            foreach (var ce in MethodParameters)
            {
                callParameters[i++ + parameterOffset] = ce.Evaluate(context);
            }

            if (acceptsLogEvent)
            {
                callParameters[0] = context;
            }

            return MethodInfo.Invoke(null, callParameters);
        }
    }
}