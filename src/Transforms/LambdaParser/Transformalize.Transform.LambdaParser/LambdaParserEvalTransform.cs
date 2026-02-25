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
using Transformalize.Contracts;

namespace Transformalize.Transforms.LambdaParser {
    public class LambdaParserEvalTransform : BaseTransform {

        private readonly Func<IRow, object> _transform;
        private readonly Dictionary<string, object> _typeDefaults = Constants.TypeDefaults();

        public LambdaParserEvalTransform(IContext context = null) : base(context, null) {

            if (IsMissingContext()) {
                return;
            }

            Returns = Context.Field.Type;

            if (IsMissing(Context.Operation.Expression)) {
                return;
            }

            try {

                var lambdaParser = new NReco.Linq.LambdaParser { UseCache = true };
                var exp = lambdaParser.Parse(Context.Operation.Expression);
                var input = Context.Entity.GetFieldMatches(exp.ToString()).ToArray();

                while (exp.CanReduce) {
                    exp = exp.Reduce();
                    Context.Debug(() => $"The expression {Context.Operation.Expression} can be reduced to {exp}");
                }
                _transform = row => Context.Field.Convert(lambdaParser.Eval(Context.Operation.Expression, input.ToDictionary(k => k.Alias, v => row[v])));
            } catch (NReco.Linq.LambdaParserException ex) {
                Context.Error($"The expression {Context.Operation.Expression} in field {Context.Field.Alias} can not be parsed. {ex.Message}");
                _transform = row => _typeDefaults[Context.Field.Type];
            }

        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _transform(row);

            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[]{
                new OperationSignature("eval") {
                    Parameters = new List<OperationParameter> {new OperationParameter("expression")}
                }
            };
        }
    }
}
