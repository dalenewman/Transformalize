#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
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
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Shorthand {
    public class ShorthandValidator : INodeValidator {
        private readonly ShorthandRoot _root;

        public ShorthandValidator(ShorthandRoot root, string name) {
            _root = root;
            Name = name;
        }

        public string Name { get; set; }
        public void Validate(INode node, string value, IDictionary<string, string> parameters, ILogger logger) {
            if (string.IsNullOrEmpty(value))
                return;
            var expressions = new Expressions(value);
            foreach (var expression in expressions) {
                MethodData methodData;
                if (!_root.MethodDataLookup.TryGetValue(expression.Method, out methodData)) {
                    logger.Warn($"The short-hand expression method {expression.Method} is undefined.");
                }
            }
        }
    }
}