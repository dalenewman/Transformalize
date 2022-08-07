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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class AnyTransform : BaseTransform {

        private class FieldWithValue {
            public Field Field { get; set; }
            public object Value { get; set; }
        }

        private readonly List<FieldWithValue> _input = new List<FieldWithValue>();
        private readonly Func<IRow, bool> _func;

        public AnyTransform(IContext context = null) : base(context, "bool") {

            if (IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.Operator)) {
                return;
            }

            if (IsMissing(Context.Operation.Value)) {
                return;
            }

            foreach (var field in MultipleInput()) {
                if (Constants.CanConvert()[field.Type](Context.Operation.Value)) {
                    _input.Add(new FieldWithValue { Field = field, Value = field.Convert(Context.Operation.Value) });
                }
            }

            _func = GetFunc(Context.Operation.Operator);
        }

        /// <summary>
        /// TODO: Implement lessthan,lessthanequal,greaterthan,greaterthanequal
        /// WARNING: Currently only support equal and notequal.
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        private Func<IRow, bool> GetFunc(string @operator) {
            // equal,notequal,lessthan,greaterthan,lessthanequal,greaterthanequal,=,==,!=,<,<=,>,>=
            switch (@operator) {
                case "notequal":
                case "notequals":
                case "!=":
                    return row => _input.Any(f => !row[f.Field].Equals(f.Value));
                case "lessthan":
                case "<":
                case "lessthanequal":
                case "<=":
                case "greaterthan":
                case ">":
                case "greaterthanequal":
                case ">=":
                default:  // equals
                    return row => _input.Any(f => row[f.Field].Equals(f.Value));
            }
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _func(row);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("any") {
                Parameters = new List<OperationParameter>(2){
                    new OperationParameter("value"),
                    new OperationParameter("operator","equals")
                }
            };
        }
    }
}