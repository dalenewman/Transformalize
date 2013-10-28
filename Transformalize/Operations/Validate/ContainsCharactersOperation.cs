using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class ContainsCharactersOperation : ValidationOperation {

        public ContainsCharactersOperation(string keyToValidate, string outKey, string characters, string containsCharacters, string messageTemplate, bool negated, bool append)
            : base(keyToValidate, outKey, append) {

            Validator = new ContainsCharactersValidator(
                characters,
                (ContainsCharacters)Enum.Parse(typeof(ContainsCharacters), containsCharacters, true),
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
