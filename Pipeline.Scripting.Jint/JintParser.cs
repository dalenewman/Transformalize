using System;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Loggers;
using Pipeline.Contracts;
using IParser = Pipeline.Contracts.IParser;

namespace Pipeline.Scripting.Jint {
    public class JintParser : IParser {
        private readonly IValidator _jintValidator;
        private readonly MemoryLogger _logger;

        public JintParser() {
            _logger = new MemoryLogger();
            _jintValidator = new JintValidator();
        }

        public string[] Errors() {
            return _logger.Errors();
        }

        public bool Parse(string script, Action<string, object[]> error) {
            _jintValidator.Validate("js", script, null, _logger);
            if (!_logger.Errors().Any())
                return true;

            foreach (var e in _logger.Errors()) {
                error(e, new object[0]);
            }
            return false;
        }
    }
}