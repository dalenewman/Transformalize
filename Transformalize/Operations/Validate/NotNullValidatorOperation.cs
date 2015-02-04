using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class NotNullValidatorOperation : ValidationOperation {
        public NotNullValidatorOperation(string keyToValidate, string resultKey, bool negated)
            : base(keyToValidate, resultKey) {
            Validator = new NotNullValidator(negated, string.Empty) { Tag = keyToValidate };
        }
    }
}