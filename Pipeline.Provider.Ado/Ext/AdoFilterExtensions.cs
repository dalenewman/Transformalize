#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.Ado.Ext {

    public static class SqlFilterExtensions {
        public static string ResolveFilter(this IContext c, IConnectionFactory factory) {

            var builder = new StringBuilder("(");
            var last = c.Entity.Filter.Count - 1;

            for (var i = 0; i < c.Entity.Filter.Count; i++) {
                var filter = c.Entity.Filter[i];
                builder.Append(ResolveExpression(c, filter, factory));
                if (i >= last) continue;
                builder.Append(" ");
                builder.Append(filter.Continuation);
                builder.Append(" ");
            }

            builder.Append(")");
            return builder.ToString();
        }

        private static string ResolveExpression(this IContext c, Filter filter, IConnectionFactory factory) {
            if (!string.IsNullOrEmpty(filter.Expression))
                return filter.Expression;

            var builder = new StringBuilder();
            var rightSide = ResolveSide(filter, "right", factory);
            var resolvedOperator = ResolveOperator(filter.Operator);
            if (rightSide.Equals("NULL")) {
                if (filter.Operator == "=") {
                    resolvedOperator = "IS";
                }
                if (filter.Operator == "!=") {
                    resolvedOperator = "IS NOT";
                }
            }

            builder.Append(ResolveSide(filter, "left", factory));
            builder.Append(" ");
            builder.Append(resolvedOperator);
            builder.Append(" ");
            builder.Append(rightSide);

            var expression = builder.ToString();
            c.Info("Filter: {0}", expression);
            return expression;
        }

        private static string ResolveSide(Filter filter, string side, IConnectionFactory factory) {

            bool isField;
            string value;
            bool otherIsField;
            Field otherField;

            if (side == "left") {
                isField = filter.IsField;
                value = filter.Field;
                otherIsField = filter.ValueIsField;
                otherField = filter.ValueField;
            } else {
                isField = filter.ValueIsField;
                value = filter.Value;
                otherIsField = filter.IsField;
                otherField = filter.LeftField;
            }

            if (isField)
                return factory.Enclose(value);

            if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
                return "NULL";

            if (!otherIsField) {
                double number;
                if (double.TryParse(value, out number)) {
                    return number.ToString(CultureInfo.InvariantCulture);
                }
                return "'" + value + "'";
            }

            if (AdoConstants.StringTypes.Any(st => st == otherField.Type)) {
                return "'" + value + "'";
            }
            return value;
        }

        private static string ResolveOperator(string op) {
            switch (op) {
                case "equal":
                    return "=";
                case "greaterthan":
                    return ">";
                case "greaterthanequal":
                    return ">=";
                case "lessthan":
                    return "<";
                case "lessthanequal":
                    return "<=";
                case "notequal":
                    return "!=";
                default:
                    return "=";
            }

        }

        public static string ResolveOrder(this InputContext context, IConnectionFactory cf) {
            if (!context.Entity.Order.Any())
                return string.Empty;
            var orderBy = string.Join(", ", context.Entity.Order.Select(o => $"{cf.Enclose(o.Field)} {o.Sort.ToUpper()}"));
            context.Debug(() => $"ORDER: {orderBy}");
            return $" ORDER BY {orderBy}";
        }
    }
}