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
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Transforms {

    public class RandomTransform : BaseTransform {

        private readonly Func<object> _transform;
        public RandomTransform(IContext context = null) : base(context, null) {
            if (IsMissingContext()) {
                return;
            }

            Returns = Context.Field.Type;

            if (!Context.Field.Type.In("double", "int")) {
                Run = false;
                Context.Error($"The random operation in {Context.Field.Alias} must return an int or double.  It can not return a {Context.Operation.Type}.");
            }

            var random = Context.Operation.Seed > 0 ? new Random(Context.Operation.Seed) : new Random();

            if (Context.Field.Type == "int" && int.TryParse(Context.Field.Max.ToString(), out var max) && int.TryParse(Context.Field.Min.ToString(), out var min) && max > 0)
            {
                _transform = () => random.Next(min, max);

            }
            else if (Context.Field.Type == "double")
            {
                _transform = () => random.NextDouble();
            }
            else
            {
                Run = false;
                Context.Error("If the random operation returns an int, you must set the min and max attributes on the field.");
            }

        }

        public override IRow Operate(IRow row)
        {
            row[Context.Field] = _transform();
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures()
        {
            yield return new OperationSignature("random")
            {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("seed","0")
                }
            };
        }
    }
}