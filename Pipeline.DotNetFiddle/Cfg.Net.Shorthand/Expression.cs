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
using System.Collections.Generic;
using System.Linq;

namespace Cfg.Net.Shorthand {
    public class Expression {
        private const char ParameterSplitter = ',';
        private const char Open = '(';
        private const string Close = ")";

        public Expression(string expression) {
            OriginalExpression = expression;
            int openIndex = expression.IndexOf(Open);

            if (openIndex > 0) {
                Method = expression.Substring(0, openIndex).ToLower();
                string parameters = expression.Remove(0, openIndex + 1);
                if (parameters.EndsWith(Close, StringComparison.Ordinal)) {
                    parameters = parameters.Substring(0, parameters.Length - 1);
                }
                SingleParameter = parameters;
                Parameters = Utility.Split(parameters, ParameterSplitter).ToList();
            } else {
                Method = expression;
                SingleParameter = string.Empty;
                Parameters = new List<string>();
            }
        }

        public string Method { get; private set; }
        public List<string> Parameters { get; private set; }
        public string OriginalExpression { get; private set; }
        public string SingleParameter { get; set; }
    }
}