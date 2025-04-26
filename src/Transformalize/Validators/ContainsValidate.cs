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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
   public class ContainsValidator : StringValidate {

      private readonly Field[] _input;
      private readonly Field _valueField;
      private readonly bool _valueIsField;
      private readonly BetterFormat _betterFormat;
      private Func<IRow, string, bool> _isValid;
      private readonly Field _field;

      public ContainsValidator(IContext context = null) : base(context) {

         if (IsMissingContext()) {
            return;
         }

         if (!Run)
            return;

         if (IsMissing(Context.Operation.Value)) {
            return;
         }

         _input = MultipleInput();
         _field = _input[0];
         _valueIsField = Context.Entity.TryGetField(Context.Operation.Value, out _valueField);

         var nextOperation = NextOperation();
         var inverted = nextOperation != null && nextOperation.Method == "invert";

         var help = Context.Field.Help;
         if (help == string.Empty) {
            if (inverted) {
               if (_valueIsField) {
                  help = $"{Context.Field.Label} must not contain {{{_valueField.Alias}}}.";
               } else {
                  help = $"{Context.Field.Label} must not contain {Context.Operation.Value}.";
               }
            } else {
               if (_valueIsField) {
                  help = $"{Context.Field.Label} must contain {{{_valueField.Alias}}}.";
               } else {
                  help = $"{Context.Field.Label} must contain {Context.Operation.Value}.";
               }
            }
         }
         _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);

         if(_input.Length > 1) {
            if (inverted) {
               _isValid = (row, text) => {
                  return !_input.Any(f => GetString(row, f).Contains(text));
               };
            } else {
               _isValid = (row, text) => {
                  return _input.Any(f => GetString(row, f).Contains(text));
               };
            }
         } else {
            if (inverted) {
               _isValid = (row, text) => !GetString(row, _field).Contains(text);
            } else {
               _isValid = (row, text) => GetString(row, _field).Contains(text);
            }
         }

      }

      public override IRow Operate(IRow row) {
         var text = _valueIsField ? GetString(row, _valueField) : Context.Operation.Value;
         var valid = _isValid(row, text);
         if (IsInvalid(row, valid)) {
            AppendMessage(row, _betterFormat.Format(row));
         }

         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("contains") { Parameters = new List<OperationParameter>(1) { new OperationParameter("value") } };
      }
   }
}