#region license
// Transformalize
// Copyright 2013 Dale Newman
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
using System.IO;
using System.Linq;
using NLog;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Pipeline.Context;
using Pipeline.Contracts;
using LogLevel = Pipeline.Contracts.LogLevel;

namespace Pipeline.Logging.NLog {
    public class NLogPipelineLogger : IPipelineLogger {

        const string Context = "{0} | {1} | {2} | {3}";
        readonly Logger _log;

        public LogLevel LogLevel { get; }

        public NLogPipelineLogger(string name, LogLevel logLevel = LogLevel.None) {
            LogLevel = logLevel;
            _log = LogManager.GetLogger("Pipeline.NET");
            ReConfiguredLogLevel(name, logLevel);
        }

        static void ReConfiguredLogLevel(string name, LogLevel logLevel) {
            if (LogManager.Configuration == null)
                return;

            if (logLevel != LogLevel.None) {
                global::NLog.LogLevel level;
                switch (logLevel) {
                    case LogLevel.Debug:
                        level = global::NLog.LogLevel.Debug;
                        break;
                    case LogLevel.Error:
                        level = global::NLog.LogLevel.Error;
                        break;
                    case LogLevel.Warn:
                        level = global::NLog.LogLevel.Warn;
                        break;
                    case LogLevel.None:
                        level = global::NLog.LogLevel.Off;
                        break;
                    case LogLevel.Info:
                        level = global::NLog.LogLevel.Info;
                        break;
                    default:
                        level = global::NLog.LogLevel.Info;
                        break;
                }
                foreach (var rule in LogManager.Configuration.LoggingRules) {
                    if (rule.Targets.Any(t => t.Name == "console")) {
                        rule.EnableLoggingForLevel(level);
                    }
                }
            }

            var target = LogManager.Configuration.FindTargetByName("file");
            if (target != null) {
                FileTarget file;
                if (target is AsyncTargetWrapper) {
                    file = (FileTarget)((AsyncTargetWrapper)target).WrappedTarget;
                } else {
                    file = (FileTarget)target;
                }
                try {
                    var info = new FileInfo(file.FileName.Render(new LogEventInfo { TimeStamp = DateTime.Now }));
                    file.FileName = Path.Combine(info.DirectoryName ?? string.Empty, name + "-" + info.Name);
                } catch (Exception) {
                    // eat it
                }
            }

            target = LogManager.Configuration.FindTargetByName("mail");
            if (target != null) {
                MailTarget mail;
                if (target is AsyncTargetWrapper) {
                    mail = (MailTarget)((AsyncTargetWrapper)target).WrappedTarget;
                } else {
                    mail = (MailTarget)target;
                }
                mail.Subject = name + " " + mail.Subject.Render(new LogEventInfo { TimeStamp = DateTime.Now });
            }

            LogManager.ReconfigExistingLoggers();
        }

        static string ForLog(PipelineContext context) {
            return string.Format(Context, context.ForLog);
        }

        public void Debug(PipelineContext context, Func<string> lamda) {
            if (!_log.IsDebugEnabled)
                return;
            _log.Debug("debug | " + ForLog(context) + " | " + lamda());
        }

        public void Info(PipelineContext context, string message, params object[] args) {
            if (!_log.IsInfoEnabled)
                return;
            var custom = string.Format(message, args);
            _log.Info("info  | " + ForLog(context) + " | " + custom);
        }

        public void Warn(PipelineContext context, string message, params object[] args) {
            if (!_log.IsWarnEnabled)
                return;
            var custom = string.Format(message, args);
            _log.Warn("warn  | " + ForLog(context) + " | " + custom);
        }

        public void Error(PipelineContext context, string message, params object[] args) {
            if (!_log.IsErrorEnabled)
                return;
            var custom = string.Format(message, args);
            _log.Error("error | " + ForLog(context) + " | " + custom);
        }

        public void Error(PipelineContext context, Exception exception, string message, params object[] args) {
            if (!_log.IsErrorEnabled)
                return;
            var custom = string.Format(message, args);
            _log.Error(exception, "error | " + ForLog(context) + " | " + custom);
            if (exception.StackTrace != null) {
                _log.Error("error | " + ForLog(context) + " | " + exception.StackTrace.Replace(Environment.NewLine, "   "));
            }

        }

        public void Clear() {
            LogManager.Flush();
        }
    }
}
