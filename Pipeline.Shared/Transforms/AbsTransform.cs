using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class AbsTransform : BaseTransform {

        readonly Field _input;
        private readonly Func<IRow, object> _transform;

        public AbsTransform(IContext context) : base(context, "decimal") {
            _input = SingleInput();

            var typeReceived = Received();

            switch (typeReceived) {
                case "double":
                    Returns = "double";
                    _transform = row => Math.Abs((double)row[_input]);
                    break;
                case "decimal":
                    _transform = row => Math.Abs((decimal)row[_input]);
                    break;
                case "int":
                case "int32":
                    Returns = "int";
                    _transform = row => Math.Abs((int)row[_input]);
                    break;
                case "float":
                    Returns = "float";
                    _transform = row => Math.Abs((float)row[_input]);
                    break;
                default:
                    Returns = Context.Field.Type;
                    Context.Warn($"The Abs transform requires extra conversion to handle a {typeReceived} type.  It handles double, decimal, int, long, and float.");
                    _transform = row => Context.Field.Convert(Math.Abs(Convert.ToDecimal(row[_input])));
                    break;
            }


        }

        public override IRow Transform(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }
    }
}