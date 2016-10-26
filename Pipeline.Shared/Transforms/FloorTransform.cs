using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FloorTransform : BaseTransform {

        readonly Field _input;

        public FloorTransform(IContext context) : base(context,"decimal") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = Math.Floor(Convert.ToDecimal(row[_input]));
            Increment();
            return row;
        }
    }
}