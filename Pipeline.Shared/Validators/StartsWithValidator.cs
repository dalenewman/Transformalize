using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class StartsWithValidator : BaseTransform, ITransform {
        private readonly Field _input;

        public StartsWithValidator(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = row[_input].ToString().StartsWith(Context.Transform.Value);
            Increment();
            return row;
        }
    }
}