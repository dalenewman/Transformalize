using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class RangeValidatorOperation : ValidationOperation {
        public RangeValidatorOperation(
            string keyToValidate,
            string outKey,
            IComparable lowerBound,
            RangeBoundaryType lowerBoundary,
            IComparable upperBound,
            RangeBoundaryType upperBoundary,
            string messageTemplate,
            bool negated,
            bool append)
            : base(keyToValidate, outKey, append) {

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