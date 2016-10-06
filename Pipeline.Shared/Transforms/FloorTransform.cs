using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FloorTransform : BaseTransform {

        readonly Field _input;

        public FloorTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            var input = Convert.ToDecimal(row[_input]);
            row[Context.Field] = Context.Field.Convert(Math.Floor(input));
            Increment();
            return row;
        }
    }
}