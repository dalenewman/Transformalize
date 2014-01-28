using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Operations.Validate
{
    public class StartsWithValidator : ValueValidator<string>
    {
        private readonly string _value;
        private readonly bool _needsMessage;

        public StartsWithValidator(string value, string messageTemplate, string tag, bool negated) :
            base(messageTemplate, tag, negated)
        {
            _value = value;
            _needsMessage = !messageTemplate.Equals(string.Empty);
        }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            var result = objectToValidate.StartsWith(_value);
            if (result == !Negated)
                return;
            var message = _needsMessage ? string.Format(MessageTemplate, key, objectToValidate, _value) : string.Empty;
            validationResults.AddResult(new ValidationResult(message, objectToValidate, key, Tag, this));
        }

        protected override string DefaultNonNegatedMessageTemplate
        {
            get { return string.Empty; }
        }

        protected override string DefaultNegatedMessageTemplate
        {
            get { return string.Empty; }
        }
    }
}