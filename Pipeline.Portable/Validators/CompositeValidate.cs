#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
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
using Pipeline.Contracts;
using Pipeline.Transforms;

namespace Pipeline.Validators {
    public class CompositeValidator : BaseTransform, ITransform {
        readonly IEnumerable<ITransform> _transforms;
        readonly Func<IRow, object> _validate;

        public CompositeValidator(IContext context, IEnumerable<ITransform> transforms)
            : base(context) {
            _transforms = transforms.ToArray();

            if (context.Field.Type.StartsWith("bool", StringComparison.Ordinal)) {
                _validate = r => _transforms.All(t => (bool)t.Transform(r)[context.Field]);
            } else {
                _validate = r => string.Concat(_transforms.Select(t => t.Transform(r)[context.Field] + " ")).Trim();
            }
        }

        public IRow Transform(IRow row) {
            row[Context.Field] = _validate(row);
            Increment();
            return row;
        }

    }
}