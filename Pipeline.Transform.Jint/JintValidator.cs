#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.Linq;
using Cfg.Net.Contracts;
using Jint.Parser;
using Transformalize.Extensions;

namespace Transformalize.Transform.Jint {

    public class JintValidator : ICustomizer {

        readonly JavaScriptParser _jint = new JavaScriptParser();
        readonly ParserOptions _options;
        public string Name { get; set; }
        public void Customize(string parent, INode node, IDictionary<string, string> parameters, ILogger logger)
        {

            if (parent != "transform")
                return;

            IAttribute scriptAttr;
            if (!node.TryAttribute("script", out scriptAttr))
                return;

            var value = scriptAttr.Value.ToString();

            if (string.IsNullOrEmpty(value)) {
                logger.Error("Script is null or empty");
                return;
            }

            try {
                var program = _jint.Parse(value, _options);
                if (program?.Errors == null || !program.Errors.Any())
                    return;

                foreach (var e in program.Errors) {
                    logger.Error("{0}, script: {1}...", e.Message, value.Left(30).Replace("{", "{{").Replace("}", "}}"));
                }
            } catch (ParserException ex) {
                logger.Error("{0}, script: {1}...", ex.Message, value.Left(30).Replace("{", "{{").Replace("}", "}}"));
            }
        }

        public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger){}

        public JintValidator() {
            _options = new ParserOptions { Tolerant = true };
        }


    }
}
