using System;
using Humanizer;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Transform.Humanizer {
    public class HumanizeTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;
        private readonly Field _input;

        public HumanizeTransform(IContext context) : base(context) {
            _input = SingleInput();
            switch (_input.Type) {
                case "date":
                case "datetime":
                    _transform = (row) => {
                        var input = (DateTime)row[_input];
                        return input.Humanize();
                    };
                    break;
                case "string":
                    _transform = (row) => {
                        var input = (string)row[_input];
                        return input.Humanize();
                    };
                    break;
                default:
                    _transform = (row) => row[_input];
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
