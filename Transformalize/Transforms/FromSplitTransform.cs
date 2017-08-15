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

using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class FromSplitTransform : BaseTransform {
        private readonly char[] _separator;
        private readonly Field _input;
        private readonly Field[] _output;

        public FromSplitTransform(IContext context) : base(context, null) {

            if (IsNotReceiving("string")) {
                return;
            }

            if (!context.Transform.Parameters.Any()) {
                Error($"The {context.Transform.Method} transform requires a collection of output fields.");
                Run = false;
                return;
            }

            if (context.Transform.Separator == Constants.DefaultSetting) {
                Error("The fromsplit method requires a separator.");
                Run = false;
                return;
            }
            
            _input = SingleInputForMultipleOutput();
            _output = MultipleOutput();
            _separator = context.Transform.Separator.ToCharArray();
        }

        public override IRow Transform(IRow row) {
            var values = row[_input].ToString().Split(_separator);
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