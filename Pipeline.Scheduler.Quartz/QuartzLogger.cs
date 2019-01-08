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
using Common.Logging;
using Common.Logging.Simple;

namespace Transformalize.Scheduler.Quartz {
    public class QuartzLogger : AbstractSimpleLogger {
        private readonly Contracts.IContext _context;

        public QuartzLogger(Contracts.IContext context, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat) : base("Transformalize", level, showLevel, showDateTime, showLogName, "o") {
            _context = context;
        }
        protected override void WriteInternal(LogLevel level, object message, Exception exception) {
            switch (level) {
                case LogLevel.All:
                    _context.Debug(message.ToString);
                    break;
                case LogLevel.Debug:
                    _context.Debug(message.ToString);
                    break;
                case LogLevel.Error:
                    if (exception == null) {
                        _context.Error(message.ToString());
                    } else {
                        _context.Error(exception, message.ToString());
                    }
                    break;
                case LogLevel.Fatal:
                    if (exception == null) {
                        _context.Error(message.ToString());
                    } else {
                        _context.Error(exception, message.ToString());
                    }
                    break;
                case LogLevel.Info:
                    _context.Info(message.ToString());
                    break;
                case LogLevel.Off:
                    break;
                case LogLevel.Trace:
                    _context.Debug(message.ToString);
                    break;
                case LogLevel.Warn:
                    _context.Warn(message.ToString());
                    break;
            }
        }
    }

}
