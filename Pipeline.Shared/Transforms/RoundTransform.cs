using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class RoundTransform : BaseTransform {

        readonly Field _input;

        public RoundTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            var input = Convert.ToDecimal(row[_input]);
            row[Context.Field] = Context.Field.Convert(Math.Round(input, Context.Transform.Decimals));
            Increment();
            return row;
        }
    }
}