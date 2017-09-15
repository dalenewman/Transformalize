using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Validators {
    public class RequiredValidator : StringValidate {

        private readonly Field _input;
        private readonly object _default;

        public RequiredValidator(IContext context) : base(context) {
            if (!Run) {
                return;
            }
            var types = Constants.TypeDefaults();
            _input = SingleInput();
            _default = _input.Default == Constants.DefaultSetting ? types[_input.Type] : _input.Convert(_input.Default);
        }

        public override IRow Operate(IRow row) {
            var valid = !row[_input].Equals(_default);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, "Is required.");
            }
            Increment();
            return row;
        }
    }
}