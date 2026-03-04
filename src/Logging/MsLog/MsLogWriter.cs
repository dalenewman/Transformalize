#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using Microsoft.Extensions.Logging;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Logging.MsLog {
    public class MsLogWriter : IWrite, IDisposable {

        private readonly ILogger _logger;
        private readonly IField _level;
        private readonly Field _message;
        private readonly ILoggerFactory _factory;
        private readonly bool _ownsFactory;

        /// <summary>
        /// Use this constructor when you already have an ILoggerFactory (e.g. from the pipeline logger or DI).
        /// The factory will NOT be disposed when the writer is disposed.
        /// </summary>
        public MsLogWriter(OutputContext context, ILoggerFactory factory) {
            _factory = factory;
            _ownsFactory = false;
            _logger = factory.CreateLogger(context.Process.Name);
            _level = context.OutputFields.First(f => f.Alias.Equals("level", StringComparison.OrdinalIgnoreCase));
            _message = context.OutputFields.First(f => f.Alias.Equals("message", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Use this constructor when no factory is available. A simple console factory is created
        /// and disposed when the writer is disposed.
        /// </summary>
        public MsLogWriter(OutputContext context, bool jsonFormat = false) {
            _factory = LoggerFactory.Create(builder => {
                if (jsonFormat) {
                    builder.AddJsonConsole(o => {
                        o.IncludeScopes = false;
                        o.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
                    });
                } else {
                    builder.AddSimpleConsole(o => {
                        o.SingleLine = true;
                        o.TimestampFormat = "u ";
                    });
                }
            });
            _ownsFactory = true;
            _logger = _factory.CreateLogger(context.Process.Name);
            _level = context.OutputFields.First(f => f.Alias.Equals("level", StringComparison.OrdinalIgnoreCase));
            _message = context.OutputFields.First(f => f.Alias.Equals("message", StringComparison.OrdinalIgnoreCase));
        }

        public void Write(IEnumerable<IRow> rows) {
            foreach (var row in rows) {
                var message = row[_message]?.ToString() ?? string.Empty;
                switch (row[_level]?.ToString()?.ToLower()) {
                    case "warn":
                    case "warning":
                        _logger.LogWarning(message);
                        break;
                    case "error":
                        _logger.LogError(message);
                        break;
                    case "debug":
                    case "trace":
                        _logger.LogDebug(message);
                        break;
                    case "critical":
                    case "fatal":
                        _logger.LogCritical(message);
                        break;
                    default:
                        _logger.LogInformation(message);
                        break;
                }
            }
        }

        public void Dispose() {
            if (_ownsFactory) {
                _factory.Dispose();
            }
        }
    }
}
