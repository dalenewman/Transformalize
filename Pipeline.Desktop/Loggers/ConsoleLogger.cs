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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Logging;

namespace Transformalize.Desktop.Loggers {
    public class ConsoleLogger : BaseLogger, IPipelineLogger {

        const string Format = "{0:u} | {1} | {2} | {3}";
        const string Context = "{0} | {1} | {2} | {3}";

        public ConsoleLogger(LogLevel level = LogLevel.Info)
            : base(level) {
        }

        static string ForLog(PipelineContext context) {
            return string.Format(Context, context.ForLog);
        }

        public void Debug(PipelineContext context, Func<string> lamda) {
            if (DebugEnabled) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(Format, DateTime.UtcNow, ForLog(context), "debug", lamda());
            }
        }

        public void Info(PipelineContext context, string message, params object[] args) {
            if (InfoEnabled) {
                var custom = string.Format(message, args);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(Format, DateTime.UtcNow, ForLog(context), "info ", custom);
            }
        }

        public void Warn(PipelineContext context, string message, params object[] args) {
            if (WarnEnabled) {
                var custom = string.Format(message, args);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(Format, DateTime.UtcNow, ForLog(context), "warn ", custom);
            }
        }

        public void Error(PipelineContext context, string message, params object[] args) {
            if (ErrorEnabled) {
                var custom = string.Format(message, args);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(Format, DateTime.UtcNow, ForLog(context), "error", custom);
            }
        }

        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {
            if (ErrorEnabled) {
                var custom = string.Format(message, args);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(Format, DateTime.UtcNow, ForLog(context), "error", custom);
                Console.Error.WriteLine(exception.Message);
                Console.Error.WriteLine(exception.StackTrace);
            }
        }

        public void Clear() {
            Console.Clear();
        }
    }
}