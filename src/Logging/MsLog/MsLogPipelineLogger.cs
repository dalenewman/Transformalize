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
using Microsoft.Extensions.Logging;
using Transformalize.Contracts;
using Transformalize.Logging;
using MelLogLevel = Microsoft.Extensions.Logging.LogLevel;
using TflLogLevel = Transformalize.Contracts.LogLevel;

namespace Transformalize.Logging.MsLog {
    public class MsLogPipelineLogger : BaseLogger, IPipelineLogger, IDisposable {

        private readonly ILoggerFactory _factory;
        private readonly ILogger _logger;

        public MsLogPipelineLogger(TflLogLevel level, bool jsonFormat = false) : base(level) {
            _factory = LoggerFactory.Create(builder => {
                builder.SetMinimumLevel(ToMelLevel(level));
                if (jsonFormat) {
                    builder.AddJsonConsole(o => {
                        o.IncludeScopes = true;
                        o.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
                    });
                } else {
                    builder.AddSimpleConsole(o => {
                        o.IncludeScopes = false;
                        o.SingleLine = true;
                        o.TimestampFormat = "u ";
                    });
                }
            });
            _logger = _factory.CreateLogger("TFL");
        }

        public void Debug(IContext context, Func<string> lambda) {
            if (!DebugEnabled) return;
            using (_logger.BeginScope(Scope(context)))
                _logger.LogDebug("{Context} | {Message}", ForLog(context), lambda());
        }

        public void Info(IContext context, string message, params object[] args) {
            if (!InfoEnabled) return;
            using (_logger.BeginScope(Scope(context)))
                _logger.LogInformation("{Context} | {Message}", ForLog(context), string.Format(message, args));
        }

        public void Warn(IContext context, string message, params object[] args) {
            if (!WarnEnabled) return;
            using (_logger.BeginScope(Scope(context)))
                _logger.LogWarning("{Context} | {Message}", ForLog(context), string.Format(message, args));
        }

        public void Error(IContext context, string message, params object[] args) {
            if (!ErrorEnabled) return;
            using (_logger.BeginScope(Scope(context)))
                _logger.LogError("{Context} | {Message}", ForLog(context), string.Format(message, args));
        }

        public void Error(IContext context, Exception exception, string message, params object[] args) {
            if (!ErrorEnabled) return;
            using (_logger.BeginScope(Scope(context)))
                _logger.LogError(exception, "{Context} | {Message}", ForLog(context), string.Format(message, args));
        }

        public void SuppressConsole() {
            DebugEnabled = false;
            InfoEnabled = false;
            WarnEnabled = false;
            // ErrorEnabled stays true — mirrors ConsoleLogger behavior
        }

        public void Clear() { }

        public void Dispose() => _factory.Dispose();

        private static string ForLog(IContext context) {
            return string.Format("{0} | {1} | {2}", context.ForLog);
        }

        private static Dictionary<string, object?> Scope(IContext context) => new() {
            ["Process"]   = context.Process?.Name,
            ["Entity"]    = context.Entity?.Alias,
            ["Field"]     = context.Field?.Alias,
            ["Operation"] = context.Operation?.Method
        };

        private static MelLogLevel ToMelLevel(TflLogLevel level) => level switch {
            TflLogLevel.Debug => MelLogLevel.Debug,
            TflLogLevel.Warn  => MelLogLevel.Warning,
            TflLogLevel.Error => MelLogLevel.Error,
            TflLogLevel.None  => MelLogLevel.None,
            _                 => MelLogLevel.Information,
        };
    }
}
