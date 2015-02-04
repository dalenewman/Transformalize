using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class ContainsCharactersValidatorOperation : ValidationOperation {

        public ContainsCharactersValidatorOperation(string keyToValidate, string resultKey, string characters, ContainsCharacters containsCharacters, bool negated)
            : base(keyToValidate, resultKey) {

            Validator = new ContainsCharactersValidator(
                characters,
                containsCharacters,
                negated
            ) { Tag = keyToValidate };
        }

    }
}
