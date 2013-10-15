using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations.Validate {
    public class ValidatorOperationFactory {

        private const string RESULTS = "TflValidationResults";

        public AbstractOperation Create(string inKey, ValidatorConfigurationElement element) {
            var method = element.Method.ToLower();
            switch (method) {
                case "containscharacters":
                    return new ContainsCharactersOperation(
                        inKey,
                        RESULTS,
                        element.Characters,
                        element.Scope,
                        element.Message,
                        element.Negated
                    );
                case "datetimerange":
                    return new DateTimeRangeOperation(
                        inKey,
                        RESULTS,
                        element.LowerBound,
                        element.LowerBoundType,
                        element.UpperBound,
                        element.UpperBoundType,
                        element.Message,
                        element.Negated
                    );
            }
            return new EmptyValidatorOperation();
        }

    }
}
