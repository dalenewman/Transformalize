using System;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Transformalize.Configuration;

namespace Transformalize.Transform.DateMath {
    public class DateMathTransform : BaseTransform {
        private readonly Field _input;

        public DateMathTransform(IContext context) : base(context, "datetime") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = DaleNewman.DateMath.Apply((DateTime)row[_input], Context.Transform.Expression);
            Increment();
            return row;
        }
    }
}
