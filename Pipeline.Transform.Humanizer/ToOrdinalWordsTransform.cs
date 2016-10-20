using System;
using Humanizer;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Transforms;

namespace Pipeline.Transform.Humanizer {
    public class ToOrdinalWordsTransform : BaseTransform {
        private readonly Func<IRow, object> _transform;
        private readonly Field _input;

        public ToOrdinalWordsTransform(IContext context) : base(context) {
            _input = SingleInput();
            switch (_input.Type.Left(3)) {
                case "int":
                case "sho":
                    _transform = (row) => {
                        var input = _input.Type.In("int","int32") ? (int)row[_input] : Convert.ToInt32(row[_input]);
                        return input.ToOrdinalWords();
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