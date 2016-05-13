using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class ToTimeTransform : BaseTransform, ITransform {
        private readonly Field _input;
        public ToTimeTransform(IContext context) : base(context) {
            _input = SingleInput();
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = TimeSpan.FromHours(_input.Type == "double" ? (double)row[_input] : Convert.ToDouble(row[_input])).ToString();
            Increment();
            return row;
        }
    }
}