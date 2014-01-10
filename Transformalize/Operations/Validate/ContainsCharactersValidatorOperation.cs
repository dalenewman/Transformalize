using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class ContainsCharactersValidatorOperation : ValidationOperation {

        public ContainsCharactersValidatorOperation(string keyToValidate, string resultKey, string messageKey, string characters, ContainsCharacters containsCharacters, string messageTemplate, bool negated, bool messageAppend)
            : base(keyToValidate, resultKey, messageKey, messageAppend) {

            Validator = new ContainsCharactersValidator(
                characters,
                containsCharacters,
                messageTemplate,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
