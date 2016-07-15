using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class InvertTransform : BaseTransform, ITransform {
        private readonly Field _input;

        public InvertTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = !(bool)row[_input];
            Increment();
            return row;
        }
    }
}