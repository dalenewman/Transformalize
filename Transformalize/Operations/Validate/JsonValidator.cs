using System;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Operations.Validate {
    public class JsonValidator : ValueValidator<string> {
        private string _errorMessage = "JSON is invalid.";

        public JsonValidator(string tag) : base("", tag, false) { }
        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults) {
            try {
                JSON.Instance.Parse(objectToValidate);
            } catch (Exception e) {
                var message = string.Format("{0} in {1}'s contents: {2}.", e.Message, Tag, objectToValidate);
                validationResults.AddResult(new ValidationResult(message, objectToValidate, key, Tag, this));
                _errorMessage = message;
            }
        }

        protected override string DefaultNonNegatedMessageTemplate {
            get { return _errorMessage; }
        }

        protected override string DefaultNegatedMessageTemplate {
            get { return _errorMessage; }
        }
    }
}