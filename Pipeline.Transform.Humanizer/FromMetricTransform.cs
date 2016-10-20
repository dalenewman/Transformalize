using System;
using Humanizer;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Transforms;

namespace Pipeline.Transform.Humanizer {
    public class FromMetricTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;
        private readonly Field _input;

        public FromMetricTransform(IContext context) : base(context) {
            _input = SingleInput();
            switch (_input.Type.Left(3)) {
                case "string":
                    _transform = (row) => {
                        var input = (string) row[_input];
                        return Context.Field.Convert(input.FromMetric());
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