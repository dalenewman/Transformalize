using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class RangeValidatorOperation : ValidationOperation {
        public RangeValidatorOperation(
            string keyToValidate,
            string resultKey,
            string messageKey,
            IComparable lowerBound,
            RangeBoundaryType lowerBoundary,
            IComparable upperBound,
            RangeBoundaryType upperBoundary,
            string messageTemplate,
            bool negated,
            bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {

            Validator = new RangeValidator(
                lowerBound,
                lowerBoundary,
                upperBound,
                upperBoundary,
                messageTemplate,
                negated
                ) { Tag = keyToValidate };

            }
    }
}