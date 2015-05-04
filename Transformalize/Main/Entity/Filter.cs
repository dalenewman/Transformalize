using System;
using System.Globalization;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Logging;

namespace Transformalize.Main {

    public class Filter {
        private readonly ILogger _logger;

        public Filter(ILogger logger) {
            Expression = string.Empty;
            _logger = logger;
        }

        public Field LeftField { get; set; }
        public Field RightField { get; set; }

        public string LeftLiteral { get; set; }
        public string RightLiteral { get; set; }

        public string Expression { get; set; }

        public string ResolveExpression(string textQualifier) {
            if (!string.IsNullOrEmpty(Expression))
                return Expression;

            var builder = new StringBuilder();
            var rightSide = ResolveRightSide(textQualifier);
            var resolvedOperator = ResolveOperator();
            if (rightSide.Equals("NULL")) {
                if (Operator == ComparisonOperator.Equal) {
                    resolvedOperator = "IS";
                }
                if (Operator == ComparisonOperator.NotEqual) {
                    resolvedOperator = "IS NOT";
                }
            }

            builder.Append(ResolveLeftSide(textQualifier));
            builder.Append(" ");
            builder.Append(resolvedOperator);
            builder.Append(" ");
            builder.Append(rightSide);

            var expression = builder.ToString();
            _logger.EntityWarn(Entity(), "Input filter: {0}", expression);
            return expression;
        }

        private string ResolveOperator() {
            switch (Operator) {
                case ComparisonOperator.Equal:
                    return "=";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanEqual:
                    return ">=";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanEqual:
                    return "<=";
                case ComparisonOperator.NotEqual:
                    return "!=";
                default:
                    return "=";
            }
        }

        private string ResolveLeftSide(string textQualifier) {
            if (!LeftIsLiteral()) return LeftField.Name;
            if (LeftLiteral.Equals("null", StringComparison.OrdinalIgnoreCase))
                return "NULL";

            if (RightIsLiteral()) {
                double number;
                if (Double.TryParse(LeftLiteral, out number)) {
                    return number.ToString(CultureInfo.InvariantCulture);
                }
                return textQualifier + LeftLiteral + textQualifier;
            }
            if (RightField.SimpleType.Equals("string") || RightField.SimpleType.StartsWith("date") || RightField.SimpleType.Equals("guid")) {
                return textQualifier + LeftLiteral + textQualifier;
            }
            return LeftLiteral;
        }

        private string ResolveRightSide(string textQualifier) {
            if (!RightIsLiteral()) return RightField.Name;
            if (RightLiteral.Equals("null", StringComparison.OrdinalIgnoreCase))
                return "NULL";

            if (LeftIsLiteral()) {
                double number;
                if (Double.TryParse(RightLiteral, out number)) {
                    return number.ToString(CultureInfo.InvariantCulture);
                }
                return textQualifier + RightLiteral + textQualifier;
            }
            if (LeftField.SimpleType.Equals("string") || LeftField.SimpleType.StartsWith("date") || LeftField.SimpleType.Equals("guid")) {
                return textQualifier + RightLiteral + textQualifier;
            }
            return RightLiteral;
        }

        private bool LeftIsLiteral() {
            return LeftLiteral != null;
        }

        private bool RightIsLiteral() {
            return RightLiteral != null;
        }

        public ComparisonOperator Operator { get; set; }
        public Continuation Continuation { get; set; }

        public bool HasExpression() {
            return !string.IsNullOrEmpty(Expression);
        }

        private string Entity() {
            if (!LeftIsLiteral()) {
                return LeftField.Entity;
            }
            if (!RightIsLiteral()) {
                return RightField.Entity;
            }
            return string.Empty;
        }

    }
}