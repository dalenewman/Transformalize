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
using System.Diagnostics;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Logging;

namespace Pipeline.Desktop.Loggers {

    public class TraceLogger : BaseLogger, IPipelineLogger {

        const string Format = "{0:u} | {1} | {2} | {3}";
        const string Context = "{0} | {1} | {2} | {3}";
        public TraceLogger(LogLevel level = LogLevel.Info)
            : base(level) {
        }

        static string ForLog(PipelineContext context) {
            return string.Format(Context, context.ForLog);
        }

        public void Debug(PipelineContext context, Func<string> lamda) {
            if (DebugEnabled) {
                Trace.WriteLine(string.Format(Format, DateTime.UtcNow, ForLog(context), "debug", lamda()), "debug");
            }
        }

        public void Info(PipelineContext context, string message, params object[] args) {
            if (InfoEnabled) {
                var custom = string.Format(message, args);
                Trace.WriteLine(string.Format(Format, DateTime.UtcNow, ForLog(context), "info ", custom), "info ");
            }
        }

        public void Warn(PipelineContext context, string message, params object[] args) {
            if (WarnEnabled) {
                var custom = string.Format(message, args);
                Trace.WriteLine(string.Format(Format, DateTime.UtcNow, ForLog(context), "warn ", custom), "warn ");
            }
        }

        public void Error(PipelineContext context, string message, params object[] args) {
            if (ErrorEnabled) {
                var custom = string.Format(message, args);
                Trace.WriteLine(string.Format(Format, DateTime.UtcNow, ForLog(context), "error", custom), "error");
            }
        }

        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {
            if (ErrorEnabled) {
                var custom = string.Format(message, args);
                Trace.WriteLine(string.Format(Format, DateTime.UtcNow, ForLog(context), "error", custom));
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(exception.StackTrace);
            }
        }

        public void Clear() {
            Trace.Flush();
        }
    }
}