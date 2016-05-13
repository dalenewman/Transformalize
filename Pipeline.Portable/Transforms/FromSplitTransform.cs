#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Transforms {
    public class FromSplitTransform : BaseTransform, ITransform {

        readonly char[] _separator;
        readonly Field _input;
        readonly Field[] _output;

        public FromSplitTransform(PipelineContext context)
            : base(context) {
            _input = SingleInputForMultipleOutput();
            _output = MultipleOutput();
            _separator = context.Transform.Separator.ToCharArray();
        }

        public IRow Transform(IRow row) {
            var values = row.GetString(_input).Split(_separator);
            if (values.Length > 0) {
                for (var i = 0; i < values.Length && i < _output.Length; i++) {
                    var output = _output[i];
                    row[output] = output.Convert(values[i]);
                }
            }
            Increment();
            return row;
        }

    }
}