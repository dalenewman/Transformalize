using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class StringLengthValidatorOperation : ValidationOperation {

        public StringLengthValidatorOperation(

            string keyToValidate,
            string outKey,
            int lowerBound,
            RangeBoundaryType lowerBoundaryType,
            int upperBound,
            RangeBoundaryType upperBoundaryType,
            string messageTemplate,
            bool negated,
            bool append)
            : base(keyToValidate, outKey, append) {
            
            Validator = new StringLengthValidator(
                lowerBound,
                lowerBoundaryType,
                upperBound,
                upperBoundaryType,
                messageTemplate,
                negated
                );
            }
    }
}