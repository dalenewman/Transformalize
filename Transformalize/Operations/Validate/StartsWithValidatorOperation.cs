namespace Transformalize.Operations.Validate
{
    public class StartsWithValidatorOperation : ValidationOperation {
        public StartsWithValidatorOperation(string keyToValidate, string value, string resultKey, bool negated)
            : base(keyToValidate, resultKey) {
            Validator = new StartsWithValidator(value, string.Empty, keyToValidate, negated);
            }
    }
}