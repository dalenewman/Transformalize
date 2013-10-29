using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class ContainsCharactersValidatorOperation : ValidationOperation {

        public ContainsCharactersValidatorOperation(string keyToValidate, string outKey, string characters, ContainsCharacters containsCharacters, string messageTemplate, bool negated, bool append)
            : base(keyToValidate, outKey, append) {

            Validator = new ContainsCharactersValidator(
                characters,
                containsCharacters,
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
