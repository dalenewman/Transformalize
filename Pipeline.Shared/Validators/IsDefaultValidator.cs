using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class IsDefaultValidator : BaseTransform, ITransform {
        private readonly Field _input;
        private readonly object _default;

        public IsDefaultValidator(IContext context) : base(context) {
            _input = SingleInput();
            _default = _input.Convert(_input.Default);
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = row[_input].Equals(_default);
            Increment();
            return row;
        }
    }
}