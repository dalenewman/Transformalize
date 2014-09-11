using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Targets;
using Transformalize.Libs.NLog.Targets.Wrappers;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Runner {
    public abstract class AbstractProcessRunner : IProcessRunner {
        public abstract IEnumerable<Row> Run(Process process);
        public void SetLog(Process process) {

            var config = new LoggingConfiguration();

            //todo: control from process <logging> collection

            //console
            var consoleTarget = new ColoredConsoleTarget {
                Layout = @"${date:format=HH\:mm\:ss} | ${level} | " + process.Name + " | ${gdc:item=entity} | ${message}"
            };
            var consoleRule = new LoggingRule("tfl", process.LogLevel, consoleTarget);

            //file
            var fileTarget = new AsyncTargetWrapper(new FileTarget {
                FileName = "${basedir}/logs/tfl-" + process.Name + "-${date:format=yyyy-MM-dd}.log",
                Layout = @"${date:format=HH\:mm\:ss} | ${level} | " + process.Name + " | ${gdc:item=entity} | ${message}"
            });
            var fileRule = new LoggingRule("tfl", process.LogLevel, fileTarget);

            //mail
            var mailTarget = new MailTarget {
                SmtpServer = "",
                Subject = "Tfl Error (" + process.Name + ")",
                From = "",
                To = "",
                Layout = @"${date:format=HH\:mm\:ss} | ${level} | ${gdc:item=entity} | ${message}"
            };
            var mailRule = new LoggingRule("tfl", LogLevel.Error, mailTarget);

            config.AddTarget("console", consoleTarget);
            config.AddTarget("file", fileTarget);
            config.AddTarget("mail", mailTarget);
            config.LoggingRules.Add(consoleRule);
            config.LoggingRules.Add(fileRule);
            config.LoggingRules.Add(mailRule);

            LogManager.Configuration = config;

            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}