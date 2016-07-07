#region license
// Cfg.Net
// Copyright 2015 Dale Newman
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cfg.Net.Contracts;

namespace Cfg.Net.Loggers {
    public class MemoryLogger : ILogger {
        private readonly StringBuilder _errors;
        private readonly StringBuilder _warnings;
        private string[] _errorCache;
        private string[] _warningCache;
        private static Regex LeftBracket = new Regex("[^{][{]{1}[^{]");
        private static Regex RightBracket = new Regex("[^}][}]{1}[^}]");


        public MemoryLogger() {
            _errors = new StringBuilder();
            _warnings = new StringBuilder();
        }

        public void Warn(string message, params object[] args) {
            try {
                _warnings.AppendFormat(message, args);
            } catch (FormatException) {
                message = LeftBracket.Replace(message, "{{");
                message = RightBracket.Replace(message, "}}");
                _warnings.AppendFormat(message, args);
            }
            _warnings.AppendLine();
            _warningCache = null;
        }

        public void Error(string message, params object[] args) {
            try {
                _errors.AppendFormat(message, args);
            } catch (FormatException) {
                message = LeftBracket.Replace(message, "{{");
                message = RightBracket.Replace(message, "}}");
                _errors.AppendFormat(message, args);
            }
            _errors.AppendLine();
            _errorCache = null;
        }

        public string[] Errors() {
            return _errorCache ?? (_errorCache = _errors.ToString()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray());
        }

        public string[] Warnings() {
            return _warningCache ?? (_warningCache = _warnings.ToString()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray());
        }

    }
}