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
using Humanizer.Bytes;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Humanizer {
    public class ByteSizeTransform : BaseTransform {
        private readonly Field _input;
        public ByteSizeTransform(IContext context) : base(context, "double") {
            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            var input = ByteSize.Parse(row[_input].ToString());
            switch (Context.Operation.Units) {
                case "bits":
                    row[Context.Field] = input.Bits;
                    break;
                case "b":
                case "bytes":
                    row[Context.Field] = input.Bytes;
                    break;
                case "kb":
                case "kilobytes":
                    row[Context.Field] = input.Kilobytes;
                    break;
                case "mb":
                case "megabytes":
                    row[Context.Field] = input.Megabytes;
                    break;
                case "gb":
                case "gigabytes":
                    row[Context.Field] = input.Gigabytes;
                    break;
                case "tb":
                case "terabytes":
                    row[Context.Field] = input.Terabytes;
                    break;
                default:
                    row[Context.Field] = 0.0d;
                    break;
            }
            Increment();
            return row;
        }
    }
}