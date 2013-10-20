using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Operations.Validate {
    public class JsonValidator : ValueValidator<string> {
        private string _errorMessage = "JSON is invalid.";

        public JsonValidator(string tag) : base("", tag, false) { }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults) {
            var result = JSON.Instance.Validate(objectToValidate);
            if (result.IsValid)
                return;
            validationResults.AddResult(new ValidationResult(result.Message + ".", objectToValidate, key, Tag, this));
            _errorMessage = result.Message + ".";
        }

        protected override string DefaultNonNegatedMessageTemplate {
            get { return _errorMessage; }
        }

        protected override string DefaultNegatedMessageTemplate {
            get { return _errorMessage; }
        }
    }
}