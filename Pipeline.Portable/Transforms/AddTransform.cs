using System;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class AddTransform : BaseTransform, ITransform {
        readonly Field[] _input;

        public AddTransform(IContext context) : base(context) {
            _input = MultipleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = Context.Field.Convert(_input.Sum(field => field.Type == "decimal" ? (decimal)row[field] : Convert.ToDecimal(row[field])));
            Increment();
            return row;
        }
    }
}