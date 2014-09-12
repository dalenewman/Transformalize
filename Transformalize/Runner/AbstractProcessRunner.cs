using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Targets;
using Transformalize.Libs.NLog.Targets.Wrappers;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Runner {
    public abstract class AbstractProcessRunner : IProcessRunner {
        public abstract IEnumerable<Row> Run(ref Process process);
        public void SetLog(ref Process process) {

            if (process.Log.Count == 1) {
                var log = process.Log[0];
                if (log.Provider == ProviderType.Internal)
                {
                    SimpleConfigurator.ConfigureForTargetLogging(log.MemoryTarget, log.Level);
                    return;
                }
            }

            if (process.Log.Count > 0) {
                var config = new LoggingConfiguration();

                foreach (var log in process.Log) {
                    switch (log.Provider) {
                        case ProviderType.Console:
                            //console
                            var consoleTarget = new ColoredConsoleTarget {
                                Name = log.Name,
                                Layout = log.Layout.Equals(Common.DefaultValue) ? @"${date:format=HH\:mm\:ss} | ${level} | " + process.Name + " | ${gdc:item=entity} | ${message}" : log.Layout
                            };
                            var consoleRule = new LoggingRule("tfl", log.Level, consoleTarget);
                            config.AddTarget(log.Name, consoleTarget);
                            config.LoggingRules.Add(consoleRule);
                            break;
                        case ProviderType.File:
                            var fileTarget = new AsyncTargetWrapper(new FileTarget {
                                Name = log.Name,
                                FileName = log.File.Equals(Common.DefaultValue) ? "${basedir}/logs/tfl-" + process.Name + "-${date:format=yyyy-MM-dd}.log" : log.File,
                                Layout = log.Layout.Equals(Common.DefaultValue) ? @"${date:format=HH\:mm\:ss} | ${level} | " + process.Name + " | ${gdc:item=entity} | ${message}" : log.Layout
                            });
                            var fileRule = new LoggingRule("tfl", log.Level, fileTarget);
                            config.AddTarget(log.Name, fileTarget);
                            config.LoggingRules.Add(fileRule);
                            break;
                        case ProviderType.Internal:
                            var memoryRule = new LoggingRule("tfl", log.Level, log.MemoryTarget);
                            config.LoggingRules.Add(memoryRule);
                            break;
                        case ProviderType.Mail:
                            if (log.Connection == null) {
                                throw new TransformalizeException("The mail logger needs to reference a mail connection in <connections/> collection.");
                            }
                            var mailTarget = new MailTarget {
                                Name = log.Name,
                                SmtpPort = log.Connection.Port.Equals(0) ? 25 : log.Connection.Port,
                                SmtpUserName = log.Connection.User,
                                SmtpPassword = log.Connection.Password,
                                SmtpServer = log.Connection.Server,
                                EnableSsl = log.Connection.EnableSsl,
                                Subject = log.Subject.Equals(Common.DefaultValue) ? "Tfl Error (" + process.Name + ")" : log.Subject,
                                From = log.From,
                                To = log.To,
                                Layout = log.Layout.Equals(Common.DefaultValue) ? @"${date:format=HH\:mm\:ss} | ${level} | ${gdc:item=entity} | ${message}" : log.Layout
                            };
                            var mailRule = new LoggingRule("tfl", LogLevel.Error, mailTarget);
                            config.AddTarget(log.Name, mailTarget);
                            config.LoggingRules.Add(mailRule);
                            break;
                        default:
                            throw new TransformalizeException("Log does not support {0} provider.", log.Provider);
                    }
                }

                LogManager.Configuration = config;
            }

            GlobalDiagnosticsContext.Set("process", Common.LogLength(process.Name));
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}