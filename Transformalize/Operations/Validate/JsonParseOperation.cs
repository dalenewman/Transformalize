namespace Transformalize.Operations.Validate
{
    public class JsonParseOperation : ValidationOperation {
        public JsonParseOperation(string inKey, string outKey, bool append)
            : base(inKey, outKey, append) {
            Validator = new JsonValidator(inKey);
        }
    }
}