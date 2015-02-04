using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {

    public class PropertyComparisonValidatorOperation : ValidationOperation {

        public PropertyComparisonValidatorOperation(string keyToValidate, string targetKey, string resultKey, string comparisonOperator, bool negated)
            : base(keyToValidate, resultKey) {

            ValidateRow = true;

            var valueAccess = new RowValueAccess(targetKey);
            var comparison = (ComparisonOperator)Enum.Parse(typeof(ComparisonOperator), comparisonOperator, true);
            Validator = new PropertyComparisonValidator(valueAccess, comparison, negated) { MessageTemplate = string.Empty, Tag = keyToValidate };

        }
    }
}