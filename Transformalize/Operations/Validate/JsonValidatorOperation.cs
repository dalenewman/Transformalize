namespace Transformalize.Operations.Validate
{
    public class JsonValidatorOperation : ValidationOperation {
        public JsonValidatorOperation(string keyToValidate, string outKey, bool append)
            : base(keyToValidate, outKey, append) {
            Validator = new JsonValidator(keyToValidate);
        }
    }
}