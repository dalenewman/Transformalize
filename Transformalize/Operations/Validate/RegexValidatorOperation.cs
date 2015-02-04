using System.Text.RegularExpressions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class RegexValidatorOperation : ValidationOperation {
        public RegexValidatorOperation(string inKey, string resultKey, string pattern, bool negated)
            : base(inKey, resultKey) {
            Validator = new RegexValidator(pattern, RegexOptions.Compiled, negated) { MessageTemplate = string.Empty, Tag = inKey };
        }
    }
}