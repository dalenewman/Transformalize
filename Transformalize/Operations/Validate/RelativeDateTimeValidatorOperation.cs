using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {

    public class RelativeDateTimeValidatorOperation : ValidationOperation {

        public RelativeDateTimeValidatorOperation(
            string keyToValidate,
            string resultKey,
            int lowerBound,
            DateTimeUnit lowerUnit,
            RangeBoundaryType lowerBoundaryType,
            int upperBound,
            DateTimeUnit upperUnit,
            RangeBoundaryType upperBoundaryType,
            bool negated)
            : base(keyToValidate, resultKey) {

            Validator = new RelativeDateTimeValidator(
                lowerBound,
                lowerUnit,
                lowerBoundaryType,
                upperBound,
                upperUnit,
                upperBoundaryType,
                string.Empty,
                negated
            ) { Tag = keyToValidate };
        }
    }
}