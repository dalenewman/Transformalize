#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class FromLengthsTranform : BaseTransform {
        private readonly Field _input;
        private readonly Field[] _output;
        private readonly int[] _lengths;

        public FromLengthsTranform(IContext context) : base(context, null) {

            _input = SingleInputForMultipleOutput();
            Run = _input.Type == "string";
            if (!Run)
                return;

            if (!context.Operation.Parameters.Any()) {
                Error($"The {context.Operation.Method} transform requires a collection of output fields.");
                Run = false;
                return;
            }

            _output = MultipleOutput();

            if (_output.Any(f => f.Length == "max")) {
                Error("The fields in a fromlengths transform may not have a max length. Set it to a numeric length.");
                Run = false;
                return;
            }

            _lengths = _output.Select(f => Convert.ToInt32(f.Length)).ToArray();
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                var line = row[_input] as string;
                if (line == null) {
                    Increment();
                } else {
                    line = line.TrimEnd();
                    if (line.Length == 0) {
                        Increment();
                    } else {
                        yield return Operate(row);
                    }
                }
            }
        }

        public override IRow Operate(IRow row) {

            var line = row[_input] as string ?? string.Empty;
            var values = new string[_lengths.Length];

            var index = 0;
            for (var i = 0; i < _lengths.Length; i++) {
                var tooShort = line.Length < index + _lengths[i];
                values[i] = tooShort ? line.Substring(index) : line.Substring(index, _lengths[i]);
                index += _lengths[i];
            }

            for (var i = 0; i < values.Length && i < _output.Length; i++) {
                var field = _output[i];
                row[field] = field.Transforms.Any() ? values[i] : field.Convert(values[i]);
            }

            Increment();
            return row;
        }
    }
}