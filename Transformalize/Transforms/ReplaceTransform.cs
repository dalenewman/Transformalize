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
using System.Reflection;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {
    public class ReplaceTransform : StringTransform {
        private readonly Field _input;
        private readonly Func<IRow, string> _getOldValue;
        private readonly Func<IRow, string> _getNewValue;

        public ReplaceTransform(IContext context = null) : base(context, "string") {

            if(IsMissingContext()) {
                return;
            }

            if (IsMissing(Context.Operation.OldValue)) {
                return;
            }

            _input = SingleInput();

            Context.Operation.OldValue = Context.Operation.OldValue.Replace("\\r", "\r");
            Context.Operation.OldValue = Context.Operation.OldValue.Replace("\\n", "\n");

            var oldIsField = Context.Entity.FieldMatcher.IsMatch(Context.Operation.OldValue);
            if (oldIsField && Context.Entity.TryGetField(context.Operation.OldValue, out var oldField)) {
                _getOldValue = row => GetString(row, oldField);
                context.Debug(() => $"replace transform's old value comes from the field: {oldField.Alias}");
            } else {
                _getOldValue = row => Context.Operation.OldValue;
                Context.Debug(() => $"replace transform's old value is literal: {Context.Operation.OldValue}");
            }

            var newIsField = Context.Entity.FieldMatcher.IsMatch(Context.Operation.NewValue);
            if (newIsField && Context.Entity.TryGetField(Context.Operation.NewValue, out var newField)) {
                _getNewValue = row => GetString(row, newField);
                Context.Debug(() => $"replace transform's new value comes from the field: {newField.Alias}");
            } else {
                _getNewValue = row => Context.Operation.NewValue;
                Context.Debug(() => $"replace transform's new value is literal: {Context.Operation.NewValue}");
            }

        }

        public override IRow Operate(IRow row) {
            var oldValue = _getOldValue(row);
            if (oldValue != string.Empty) {
                row[Context.Field] = GetString(row, _input).Replace(oldValue, _getNewValue(row));
            }

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("replace") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("old-value"),
                    new OperationParameter("new-value","")
                }
            };
        }
    }
}