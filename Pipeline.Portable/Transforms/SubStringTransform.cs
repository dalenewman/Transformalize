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
using System;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Transforms {

    public class SubStringTransform : BaseTransform, ITransform {

        private readonly Field _input;
        private readonly Action<IRow> _transform;
        private readonly Func<string, string> _substring;

        public SubStringTransform(PipelineContext context) : base(context) {
            _input = SingleInput();

            if (context.Transform.Length == 0) {
                _substring = s => s.Length < context.Transform.StartIndex ? s : s.Substring(context.Transform.StartIndex);
            } else {
                _substring = s => s.Length < context.Transform.StartIndex ? s : s.Substring(context.Transform.StartIndex, context.Transform.Length);
            }

            if (_input.Type == "string" && context.Field.Type == "string") {
                _transform = row => {
                    row.SetString(Context.Field, _substring(row.GetString(_input)));
                };
            } else {
                _transform = row => {
                    var value = _substring(row[_input].ToString());
                    if (Context.Field.Type == "string") {
                        row[context.Field] = value;
                    } else {
                        row[context.Field] = Context.Field.Convert(value);
                    }
                };
            }
        }

        public IRow Transform(IRow row) {
            _transform(row);
            Increment();
            return row;
        }
    }
}