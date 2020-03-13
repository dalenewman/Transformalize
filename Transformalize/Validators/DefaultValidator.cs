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
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {
   public class DefaultValidator : BaseValidate {
      private readonly Field _input;
      private readonly object _default;
      private readonly BetterFormat _betterFormat;
      private readonly bool _isDefault;

      public DefaultValidator(IContext context = null) : base(context) {
         if (IsMissingContext()) {
            return;
         }

         if (!Run)
            return;

         var nextOperation = NextOperation();
         _isDefault = nextOperation == null || nextOperation.Method != "invert";

         _input = SingleInput();
         var types = Constants.TypeDefaults();
         _default = _input.Default == Constants.DefaultSetting ? types[_input.Type] : _input.Convert(_input.Default);

         var help = Context.Field.Help;
         if (help == string.Empty) {
            if (_isDefault) {
               help = $"{Context.Field.Label} must be the default value of {_default}.";
            } else {
               help = $"{Context.Field.Label} must not be the default value of {_default}.";
            }
            
         }
         _betterFormat = new BetterFormat(context, help, Context.Entity.GetAllFields);
      }

      public override IRow Operate(IRow row) {
         if (IsInvalid(row, _isDefault ? row[_input].Equals(_default) : !row[_input].Equals(_default))) {
            AppendMessage(row, _betterFormat.Format(row));
         }

         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("default");
      }
   }
}