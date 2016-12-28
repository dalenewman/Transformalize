using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Vin {
    public class VinGetModelYearTransform : BaseTransform {
        private readonly Field _input;

        public VinGetModelYearTransform(IContext context) : base(context, "int") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = DaleNewman.Vin.GetModelYear(row[_input] as string);
            Increment();
            return row;
        }
    }
}