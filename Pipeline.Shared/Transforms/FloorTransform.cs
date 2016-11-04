using System;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FloorTransform : BaseTransform {

        readonly Field _input;
        private readonly Func<IRow, object> _transform;

        public FloorTransform(IContext context) : base(context, "decimal") {
            _input = SingleInput();
            switch (_input.Type) {
                case "decimal":
                    Returns = "decimal";
                    _transform = row => Math.Floor((decimal)row[_input]);
                    break;
                case "double":
                    Returns = "double";
                    _transform = row => Math.Floor((double)row[_input]);
                    break;
                default:
                    Returns = "decimal";
                    _transform = row => Math.Floor(Convert.ToDecimal(row[_input]));
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