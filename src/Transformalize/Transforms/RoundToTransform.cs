#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms {

    public enum RoundTo {
        Down,
        Nearest,
        Up
    }

    public class RoundUpToTransform : RoundToTransform {
        public RoundUpToTransform(IContext context = null) : base(context, RoundTo.Up) { }
    }

    public class RoundDownToTransform : RoundToTransform {
        public RoundDownToTransform(IContext context = null) : base(context, RoundTo.Down) { }
    }

    public class RoundToTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;
        private readonly RoundTo _roundTo;

        public RoundToTransform(IContext context = null, RoundTo roundTo = RoundTo.Nearest) : base(context, "object") {

            _roundTo = roundTo;
            // return type is adjusted below

            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceivingNumber()) {
                return;
            }

            if (!context.Operation.Value.IsNumeric()) {
                Error($"The {context.Operation.Method} transform requires a numeric value.");
                Run = false;
                return;
            }

            var input = SingleInput();

            if (Received() == "double") {

                var by = Convert.ToDouble(context.Operation.Value);
                switch (roundTo) {

                    case RoundTo.Up:
                        _transform = (r) => Math.Ceiling((double)r[input] / by) * by;
                        break;
                    case RoundTo.Down:
                        _transform = (r) => Math.Floor((double)r[input] / by) * by;
                        break;
                    default:
                        _transform = (r) => Math.Round((double)r[input] / by) * by;
                        break;
                }
                Returns = "double";
            } else {

                var by = Convert.ToDecimal(context.Operation.Value);
                if (Received() == "decimal"){
                    switch (roundTo){
                        case RoundTo.Up:
                            _transform = (r) => Math.Ceiling((decimal)r[input] / by) * by;
                            break;
                        case RoundTo.Down:
                            _transform = (r) => Math.Floor((decimal)r[input] / by) * by;
                            break;
                        default:
                            _transform = (r) => Math.Round((decimal)r[input] / by) * by;
                            break;
                    }
                    Returns = "decimal";
                } else {
                    switch (roundTo) {
                        case RoundTo.Up:
                            _transform = (r) => context.Field.Convert(Math.Ceiling(Convert.ToDecimal(r[input]) / by) * by);
                            break;
                        case RoundTo.Down:
                            _transform = (r) => context.Field.Convert(Math.Floor(Convert.ToDecimal(r[input]) / by) * by);
                            break;
                        default:
                            _transform = (r) => context.Field.Convert(Math.Round(Convert.ToDecimal(r[input]) / by) * by);
                            Returns = context.Field.Type;
                            break;
                    }
                }
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);
            
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature(GetMethodName()){
                Parameters = new List<OperationParameter> {
                    new OperationParameter("value", Constants.DefaultSetting)
                }
            };
        }

        private string GetMethodName() {
            switch (_roundTo) {
                case RoundTo.Down:
                    return "rounddownto";
                case RoundTo.Up:
                    return "roundupto";
                default:
                    return "roundto";
            }
        }
    }
}