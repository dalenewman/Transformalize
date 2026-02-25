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
using Humanizer;
using Humanizer.Bytes;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Humanizer {

    public class HumanizeTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;
        private readonly Field _input;

        public HumanizeTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            var type = Received() ?? _input.Type;

            if (type != "string" && !type.StartsWith("date", StringComparison.OrdinalIgnoreCase) && type != "bytesize") {
                Error($"The {Context.Operation.Method} expects a string or date, but received a {type} in field {Context.Field.Alias}.");
                Run = false;
                return;
            }

            _input = SingleInput();

            switch (type) {
                case "bytesize":
                    _transform = (row) => {
                        var input = (ByteSize)row[_input];
                        return Context.Operation.Format == Constants.DefaultSetting ? input.Humanize() : input.Humanize(context.Operation.Format);
                    };
                    break;
                case "date":
                case "datetime":
                    _transform = (row) => {
                        var input = (DateTime)row[_input];
                        return input.Humanize(false);
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

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("humanize"){
                    Parameters = new List<OperationParameter>{
                        new OperationParameter("format", Constants.DefaultSetting)
                    }
                }
            };
        }
    }
}
