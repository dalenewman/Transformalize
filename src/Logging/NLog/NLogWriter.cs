#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using NLog;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Logging.NLog {
    public class NLogWriter : IWrite {

        private readonly Logger _logger;
        private readonly IField _level;
        private readonly Field _message;

        public NLogWriter(OutputContext context) {
            _logger = LogManager.GetLogger(context.Process.Name);
            _level = context.OutputFields.First(f => f.Alias.Equals("level", StringComparison.OrdinalIgnoreCase));
            _message = context.OutputFields.First(f => f.Alias.Equals("message", StringComparison.OrdinalIgnoreCase));
        }

        public void Write(IEnumerable<IRow> rows) {

            foreach (var row in rows) {

                var message = row[_message] ?? string.Empty;
                switch (row[_level].ToString().ToLower()) {
                    case "warn":
                    case "warning":
                        _logger.Warn(message);
                        break;
                    case "error":
                        _logger.Error(message);
                        break;
                    case "debug":
                        _logger.Debug(message);
                        break;
                    case "trace":
                        _logger.Trace(message);
                        break;
                    case "fatal":
                        _logger.Fatal(message);
                        break;
                    default:
                        _logger.Info(message);
                        break;
                }
            }

            _logger.Info("flushing log writer");
            LogManager.Flush();
        }
    }
}