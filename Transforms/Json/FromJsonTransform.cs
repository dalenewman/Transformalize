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
using Cfg.Net.Parsers.fastJSON;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Json {

    public class FromJsonTransform : BaseTransform {
        private readonly Func<object, string> _serializer;
        private readonly Field _input;
        private readonly Field[] _output;
        private readonly HashSet<string> _errors = new HashSet<string>();

        public FromJsonTransform(IContext context, Func<object, string> serializer) : base(context, "object") {
            if (!context.Operation.Parameters.Any()) {
                Error($"The {context.Operation.Method} transform requires a collection of output fields.");
                Run = false;
                return;
            }

            _serializer = serializer;
            _input = SingleInput();
            _output = MultipleOutput();
        }

        public override IRow Operate(IRow row) {
            var json = (string)row[_input];

            try {
                var dict = JSON.Parse(json) as Dictionary<string, object>;
                if (dict != null) {
                    if (_output.Any()  && !_output[0].Equals(_input)) {
                        foreach (var field in _output) {
                            if (dict.ContainsKey(field.Name)) {
                                var value = dict[field.Name];
                                if (value is string || value is int || value is long || value is double) {
                                    row[field] = value;
                                } else {
                                    row[field] = _serializer(value);
                                }

                            }
                        }
                    } else {
                        if (dict.ContainsKey(Context.Field.Name)) {
                            var value = dict[Context.Field.Name];
                            if (value is string || value is int || value is long || value is double) {
                                row[Context.Field] = value;
                            } else {
                                row[Context.Field] = _serializer(value);
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                if (_errors.Add(ex.Message)) {
                    Context.Error(ex, ex.Message);
                }
            }

            
            return row;

        }
    }

}
