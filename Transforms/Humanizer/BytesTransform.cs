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
using Humanizer;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Humanizer {
    public class BytesTransform : BaseTransform {
        private readonly Field _input;

        public BytesTransform(IContext context) : base(context, "bytesize") {
            _input = SingleInput();
        }

        public override IRow Transform(IRow row) {
            switch (_input.Type) {
                case "byte":
                    row[Context.Field] = ((byte)row[_input]).Bytes();
                    break;
                case "short":
                case "int16":
                    row[Context.Field] = ((short)row[_input]).Bytes();
                    break;
                case "int":
                case "int32":
                    row[Context.Field] = ((int)row[_input]).Bytes();
                    break;
                case "double":
                    row[Context.Field] = ((double)row[_input]).Bytes();
                    break;
                case "long":
                case "int64":
                    row[Context.Field] = ((long)row[_input]).Bytes();
                    break;
                default:
                    row[Context.Field] = Convert.ToDouble(row[_input]).Bytes();
                    break;
            }
            Increment();
            return row;
        }
    }
}