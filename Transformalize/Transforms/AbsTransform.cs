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
using Transformalize.Contracts;
using Transformalize.Impl;

namespace Transformalize.Transforms {

    public class AbsTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;

        public AbsTransform(IContext context) : base(context, "decimal") {
            if (IsNotReceivingNumber()) {
                return;
            }

            var input = SingleInput();

            var typeReceived = Received();

            switch (typeReceived) {
                case "double":
                    Returns = "double";
                    _transform = row => Math.Abs((double)row[input]);
                    break;
                case "decimal":
                    _transform = row => Math.Abs((decimal)row[input]);
                    break;
                case "int":
                case "int32":
                    Returns = "int";
                    _transform = row => Math.Abs((int)row[input]);
                    break;
                case "int64":
                case "long":
                    Returns = "long";
                    _transform = row => Math.Abs((long)row[input]);
                    break;
                case "float":
                    Returns = "float";
                    _transform = row => Math.Abs((float)row[input]);
                    break;
                default:
                    Returns = Context.Field.Type;
                    Context.Warn($"The Abs transform requires extra conversion to handle a {typeReceived} type.  It handles double, decimal, int, long, and float.");
                    _transform = row => Context.Field.Convert(Math.Abs(Convert.ToDecimal(row[input])));
                    break;
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            Increment();
            return row;
        }

        public static OperationSignature GetSignature() {
            return new OperationSignature("abs");
        }
    }
}