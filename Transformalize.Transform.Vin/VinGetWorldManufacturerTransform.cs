using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Transform.Vin {
    public class VinGetWorldManufacturerTransform : BaseTransform {
        private readonly Field _input;

        public VinGetWorldManufacturerTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = DaleNewman.Vin.GetWorldManufacturer(row[_input] as string);
            Increment();
            return row;
        }
    }
}