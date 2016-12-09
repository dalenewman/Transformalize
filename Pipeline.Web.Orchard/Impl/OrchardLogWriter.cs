using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Localization;
using Orchard.Logging;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Pipeline.Web.Orchard.Impl {
    public class OrchardLogWriter : IWrite {
        private readonly ILogger _logger;
        private readonly IField _level;
        private readonly Field _message;
        public readonly Localizer _t;

        public OrchardLogWriter(OutputContext context) {
            _logger = NullLogger.Instance;
            _t = NullLocalizer.Instance;
            _level = context.OutputFields.First(f => f.Name.Equals("level", StringComparison.OrdinalIgnoreCase));
            _message = context.OutputFields.First(f => f.Name.Equals("message", StringComparison.OrdinalIgnoreCase));
        }

        public void Write(IEnumerable<IRow> rows) {
            foreach (var row in rows) {

                var message = row[_message] ?? string.Empty;
                switch (row[_level].ToString().ToLower()) {
                    case "warn":
                    case "warning":
                        _logger.Warning(message as string);
                        break;
                    case "error":
                        _logger.Error(message as string);
                        break;
                    case "debug":
                        _logger.Debug(message as string);
                        break;
                    case "fatal":
                        _logger.Fatal(message as string);
                        break;
                    default:
                        _logger.Information(message as string);
                        break;
                }
            }

        }
    }
}