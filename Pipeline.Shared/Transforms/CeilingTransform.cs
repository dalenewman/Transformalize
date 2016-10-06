using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class CeilingTransform : BaseTransform {

        readonly Field _input;

        public CeilingTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            var input = Convert.ToDecimal(row[_input]);
            row[Context.Field] = Context.Field.Convert(Math.Ceiling(input));
            Increment();
            return row;
        }
    }
}