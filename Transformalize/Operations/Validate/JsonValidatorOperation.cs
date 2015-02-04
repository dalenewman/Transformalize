namespace Transformalize.Operations.Validate
{
    public class JsonValidatorOperation : ValidationOperation {
        public JsonValidatorOperation(string keyToValidate, string resultKey, bool negated)
            : base(keyToValidate, resultKey) {
            Validator = new JsonValidator(keyToValidate, negated);
        }
    }
}