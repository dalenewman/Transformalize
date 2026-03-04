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
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Transformalize.Logging.MsLog {
    internal sealed class TflJsonFormatter : ConsoleFormatter {

        public const string FormatterName = "tfl-json";

        public TflJsonFormatter(IOptionsMonitor<ConsoleFormatterOptions> options) : base(FormatterName) { }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter) {
            var buffer = new MemoryStream();
            using (var writer = new Utf8JsonWriter(buffer)) {
                writer.WriteStartObject();
                writer.WriteString("Timestamp", DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                writer.WriteString("Level", logEntry.LogLevel.ToString());
                writer.WriteString("Category", logEntry.Category);

                if (logEntry.State is IEnumerable<KeyValuePair<string, object?>> pairs) {
                    foreach (var pair in pairs) {
                        if (pair.Key == "{OriginalFormat}") continue;
                        writer.WriteString(pair.Key, pair.Value?.ToString() ?? string.Empty);
                    }
                }

                if (logEntry.Exception != null) {
                    writer.WriteString("Exception", logEntry.Exception.ToString());
                }

                writer.WriteEndObject();
            }

            textWriter.WriteLine(Encoding.UTF8.GetString(buffer.ToArray()));
        }
    }
}
