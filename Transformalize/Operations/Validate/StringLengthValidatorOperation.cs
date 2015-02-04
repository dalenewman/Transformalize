using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class StringLengthValidatorOperation : ValidationOperation {

        public StringLengthValidatorOperation(
            string keyToValidate,
            string resultKey,
            int lowerBound,
            RangeBoundaryType lowerBoundaryType,
            int upperBound,
            RangeBoundaryType upperBoundaryType,
            bool negated)
            : base(keyToValidate, resultKey) {

            Validator = new StringLengthValidator(
                lowerBound,
                lowerBoundaryType,
                upperBound,
                upperBoundaryType,
                string.Empty,
                negated
                );
        }
    }
}