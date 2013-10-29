using System.Text.RegularExpressions;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class RegexValidatorOperation : ValidationOperation {
        public RegexValidatorOperation(string inKey, string outKey, string pattern, string message, bool negated, bool append)
            : base(inKey, outKey, append) {
            Validator = new RegexValidator(pattern, RegexOptions.Compiled, negated) { MessageTemplate = message, Tag = inKey };
            }
    }
}