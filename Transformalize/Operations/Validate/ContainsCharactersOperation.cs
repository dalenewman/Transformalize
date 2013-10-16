using System;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate {
    public class ContainsCharactersOperation : ValidationOperation {

        public ContainsCharactersOperation(string inKey, string outKey, string characters, string containsCharacters, string messageTemplate, bool negated, bool append)
            : base(inKey, outKey, append) {

            Validator = new ContainsCharactersValidator(
                characters,
                (ContainsCharacters)Enum.Parse(typeof(ContainsCharacters), containsCharacters, true),
                messageTemplate,
                negated
            ) { Tag = inKey };
        }

    }
}
