using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class EndsWithValidator : BaseTransform, ITransform {
        private readonly Field _input;

        public EndsWithValidator(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = row[_input].ToString().EndsWith(Context.Transform.Value);
            Increment();
            return row;
        }
    }
}