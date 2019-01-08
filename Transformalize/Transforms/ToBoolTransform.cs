#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class ToBoolTransform : BaseTransform {

        private readonly Field _input;
        private readonly Func<object, bool> _transform;

        public ToBoolTransform(IContext context = null) : base(context, "bool") {

            if (IsMissingContext()) {
                return;
            }

            switch (Received()) {

                case "string":
                    _transform = delegate (object o) {
                        var input = ((string)o).ToLower();
                        switch (input) {
                            case "false":
                            case "no":
                            case "n":
                            case "":
                            case "0":
                                return false;
                            default:
                                return true;
                        }
                    };
                    break;
                case "char":
                    _transform = o => (char)o != '0' && (char)o != default(char);
                    break;
                case "int32":
                case "int":
                    _transform = o => (int)o != 0;
                    break;
                case "int16":
                case "short":
                    _transform = o => (short)o != 0;
                    break;
                case "int64":
                case "long":
                    _transform = o => (long)o != 0;
                    break;
                case "byte":
                    _transform = o => (byte)o != 0;
                    break;
                case "byte[]":
                    _transform = o => ((byte[])o).Length != 0;
                    break;
                case "real":
                case "double":
                    _transform = o => (double)o > 0.0 || (double)o < 0.0;
                    break;
                case "decimal":
                    _transform = o => (decimal)o != 0.0M;
                    break;
                case "float":
                case "single":
                    _transform = o => (float)o > 0.0F || (float)o < 0.0F;
                    break;
                case "guid":
                    _transform = o => (Guid)o != Guid.Empty;
                    break;
                case "uint16":
                case "ushort":
                    _transform = o => (ushort)o != 0;
                    break;
                case "uint32":
                case "uint":
                    _transform = o => (uint)o != 0;
                    break;
                case "uint64":
                case "ulong":
                    _transform = o => (ulong)o != 0;
                    break;
                default:
                    Run = false;
                    Context.Error($"The toBool method in the {Context.Field.Alias} can not receive a {Received()} type as input.");
                    return;
            }
            _input = SingleInput();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row[_input]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] { new OperationSignature("tobool") };
        }

    }

}
