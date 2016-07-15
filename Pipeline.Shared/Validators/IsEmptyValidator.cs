using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class IsEmptyValidator : BaseTransform, ITransform {
        private readonly Field _input;

        public IsEmptyValidator(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = (string)row[_input] == string.Empty;
            Increment();
            return row;
        }
    }
}