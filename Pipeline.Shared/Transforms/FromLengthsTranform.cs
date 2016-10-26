using System;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FromLengthsTranform : BaseTransform {
        readonly Field _input;
        readonly Field[] _output;
        private readonly int[] _lengths;

        public FromLengthsTranform(IContext context) : base(context, null) {
            _input = SingleInputForMultipleOutput();
            _output = MultipleOutput();
            _lengths = _output.Select(f=>Convert.ToInt32(f.Length)).ToArray();
        }

        public override IRow Transform(IRow row) {

            var line = row[_input] as string;
            if (line == null) {
                Increment();
                return row;
            }

            var values = new string[_lengths.Length];

            var index = 0;
            for (var i = 0; i < _lengths.Length; i++) {
                values[i] = line.Substring(index, _lengths[i]);
                index += _lengths[i];
            }

            for (var i = 0; i < values.Length && i < _output.Length; i++) {
                var field = _output[i];
                row[field] = field.Convert(values[i]);
            }

            Increment();
            return row;
        }
    }
}