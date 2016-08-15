using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Logging.NLog {
    public class NLogWriter : IWrite {

        private readonly Logger _logger;
        private readonly IField _level;
        private readonly Field _message;

        public NLogWriter(OutputContext context) {
            _logger = LogManager.GetLogger(context.Process.Name);
            _level = context.OutputFields.First(f => f.Name.Equals("level", StringComparison.OrdinalIgnoreCase));
            _message = context.OutputFields.First(f => f.Name.Equals("message", StringComparison.OrdinalIgnoreCase));
        }

        public void Write(IEnumerable<IRow> rows) {

            foreach (var row in rows) {

                var message = row[_message] ?? string.Empty;
                switch (row[_level].ToString().ToLower()) {
                    case "warn":
                    case "warning":
                        _logger.Warn(message);
                        break;
                    case "error":
                        _logger.Error(message);
                        break;
                    case "debug":
                        _logger.Debug(message);
                        break;
                    case "trace":
                        _logger.Trace(message);
                        break;
                    case "fatal":
                        _logger.Fatal(message);
                        break;
                    default:
                        _logger.Info(message);
                        break;
                }
            }
        }
    }
}