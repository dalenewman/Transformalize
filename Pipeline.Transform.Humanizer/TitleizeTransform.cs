using System;
using Humanizer;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Transform.Humanizer {
    public class TitleizeTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;
        private readonly Field _input;

        public TitleizeTransform(IContext context) : base(context, "string") {
            _input = SingleInput();
            switch (_input.Type) {
                case "string":
                    _transform = (row) => {
                        var input = (string)row[_input];
                        return input.Titleize();
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