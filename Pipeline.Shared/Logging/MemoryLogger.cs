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

using System;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Logging {
    public class MemoryLogger : BaseLogger, IPipelineLogger {
        public List<LogEntry> Log { get; }

        public MemoryLogger(LogLevel level)
            : base(level) {
            Log = new List<LogEntry>();
        }

        public void Debug(PipelineContext context, Func<string> lamda) {
            if (DebugEnabled) {
                Log.Add(new LogEntry(LogLevel.Debug, context, lamda()));
            }
        }

        public void Info(PipelineContext context, string message, params object[] args) {
            if (InfoEnabled) {
                Log.Add(new LogEntry(LogLevel.Info, context, message, args));
            }
        }

        public void Warn(PipelineContext context, string message, params object[] args) {
            if (WarnEnabled) {
                Log.Add(new LogEntry(LogLevel.Warn, context, message, args));
            }
        }

        public void Error(PipelineContext context, string message, params object[] args) {
            if (ErrorEnabled) {
                Log.Add(new LogEntry(LogLevel.Error, context, message, args));
            }
        }

        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {
            if (ErrorEnabled) {
                Log.Add(new LogEntry(LogLevel.Error, context, message, args) { Exception = exception });
            }
        }

        public void Clear() {
            Log.Clear();
        }
    }
}