using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class AppendTransform : BaseTransform {
        private readonly Field _input;
        public AppendTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = row[_input] + Context.Transform.Value;
            Increment();
            return row;
        }
    }
}