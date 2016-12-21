using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Vin {
    public class VinValidateTransform : BaseTransform {
        private readonly Field _input;

        public VinValidateTransform(IContext context) : base(context, "bool") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = DaleNewman.Vin.IsValid(row[_input] as string);
            Increment();
            return row;
        }
    }
}
