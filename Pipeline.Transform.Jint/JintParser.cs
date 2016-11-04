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

using System;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Loggers;
using IParser = Pipeline.Contracts.IParser;

namespace Pipeline.Transform.Jint {
    public class JintParser : IParser {
        private readonly IValidator _jintValidator;
        private readonly MemoryLogger _logger;

        public JintParser() {
            _logger = new MemoryLogger();
            _jintValidator = new JintValidator();
        }

        public string[] Errors() {
            return _logger.Errors();
        }

        public bool Parse(string script, Action<string, object[]> error) {
            _jintValidator.Validate("js", script, null, _logger);
            if (!_logger.Errors().Any())
                return true;

            foreach (var e in _logger.Errors()) {
                error(e, new object[0]);
            }
            return false;
        }
    }
}