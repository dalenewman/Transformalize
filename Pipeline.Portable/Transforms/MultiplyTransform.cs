using System;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class MultiplyTransform : BaseTransform, ITransform {
        readonly Field[] _input;

        public MultiplyTransform(IContext context) : base(context) {
            _input = MultipleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = Context.Field.Convert(_input.Aggregate<Field, decimal>(1, (current, field) => current * (field.Type == "decimal" ? (decimal)row[field] : Convert.ToDecimal(row[field]))));
            Increment();
            return row;
        }
    }
}