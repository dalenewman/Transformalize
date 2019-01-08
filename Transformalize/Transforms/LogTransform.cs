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

namespace Transformalize.Transforms {

    public class LogTransform : BaseTransform {

        private readonly Action<object> _logger;

        public LogTransform(IContext context = null) : base(context, null) {
            if (IsMissingContext()) {
                return;
            }

            switch (Context.Operation.Level) {
                case "debug":
                    _logger = o => Context.Debug(() => $"{LastMethod()} => {o}");
                    break;
                case "error":
                    _logger = o => Context.Error($"{LastMethod()} => {o}");
                    break;
                case "warn":
                    _logger = o => Context.Warn($"{LastMethod()} => {o}");
                    break;
                default:
                    _logger = o => Context.Info($"{LastMethod()} => {o}");
                    break;
            }

        }

        public override IRow Operate(IRow row) {
            _logger(row[Context.Field]);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("log") { Parameters = new List<OperationParameter>(1) { new OperationParameter("level", "info") } };
        }
    }
}