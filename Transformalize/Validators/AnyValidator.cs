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
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class AnyValidator : StringValidate {
        private class FieldWithValue {
            public Field Field { get; set; }
            public object Value { get; set; }
        }

        private readonly List<FieldWithValue> _input = new List<FieldWithValue>();
        private readonly Func<IRow, bool> _func;
        private readonly BetterFormat _betterFormat;

        public AnyValidator(IContext context) : base(context) {

            if (!Run)
                return;

            if (IsMissing(context.Operation.Operator)) {
                return;
            }

            if (IsMissing(context.Operation.Value)) {
                return;
            }

            foreach (var field in MultipleInput()) {
                if (Constants.CanConvert()[field.Type](Context.Operation.Value)) {
                    _input.Add(new FieldWithValue { Field = field, Value = field.Convert(Context.Operation.Value) });
                }
            }

            _func = GetFunc(Context.Operation.Operator);
            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"At least one of the field(s): {Utility.ReadableDomain(_input.Select(f => f.Field.Alias))} must be {context.Operation.Operator.TrimEnd('s')} to {context.Operation.Value}.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
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
                default:
                    return row => _input.Any(f => row[f.Field].Equals(f.Value));
            }
        }

        public override IRow Operate(IRow row) {
            var valid = _func(row);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            Increment();
            return row;
        }
    }
}