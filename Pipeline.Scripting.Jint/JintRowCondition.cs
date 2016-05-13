#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using System.Linq;
using Jint;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Scripting.Jint {

    public class JintRowCondition : IRowCondition {
        private readonly string _expression;
        private readonly Field[] _input;
        readonly Engine _jint = new Engine();
        public JintRowCondition(IContext context, string expression) {
            _expression = expression;
            _input = new global::Jint.Parser.JavaScriptParser().Parse(expression, new global::Jint.Parser.ParserOptions { Tokens = true }).Tokens
                .Where(o => o.Type == global::Jint.Parser.Tokens.Identifier)
                .Select(o => o.Value.ToString())
                .Intersect(context.GetAllEntityFields().Select(f => f.Alias))
                .Distinct()
                .Select(a => context.Entity.GetField(a))
                .ToArray();
        }

        public bool Eval(IRow row) {
            foreach (var field in _input) {
                _jint.SetValue(field.Alias, row[field]);
            }
            return _jint.Execute(_expression).GetCompletionValue().AsBoolean();
        }
    }
}