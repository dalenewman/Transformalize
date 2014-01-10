using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {

    public class PropertyComparisonValidatorOperation : ValidationOperation {

        public PropertyComparisonValidatorOperation(string keyToValidate, string targetKey, string resultKey, string messageKey, string comparisonOperator, string message, bool negated, bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {

            ValidateRow = true;

            var valueAccess = new RowValueAccess(targetKey);
            var comparison = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), comparisonOperator, true);
            Validator = new PropertyComparisonValidator(valueAccess, comparison, negated) { MessageTemplate = message, Tag = keyToValidate };

        }
    }
}