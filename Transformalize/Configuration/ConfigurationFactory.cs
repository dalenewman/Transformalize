using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class ConfigurationFactory {

        private readonly ILogger _logger;
        private readonly Dictionary<string, string> _parameters;
        private readonly string _resource;

        public ConfigurationFactory(string resource, ILogger logger, Dictionary<string, string> parameters = null) {
            _logger = logger;
            _parameters = parameters;
            _resource = resource.Trim();
        }

        public List<TflProcess> Create() {
            var shouldThrow = false;
            var cfg = new TflRoot(_resource, _parameters);

            if (cfg.Response.Any()) {
                foreach (var response in cfg.Response.Where(response => response.Status != (short)200)) {
                    _logger.Warn("API at {0} responded with {1} {2}.", _resource, response.Status, response.Message);
                }
            }

            foreach (var warning in cfg.Warnings()) {
                _logger.Warn(warning);
            }

            foreach (var error in cfg.Errors()) {
                shouldThrow = true;
                _logger.Error(error);
            }

            if (shouldThrow) {
                throw new TransformalizeException(_logger, string.Join(Environment.NewLine, cfg.Errors()));
            }

            return cfg.Processes;
        }

        public TflProcess CreateSingle() {
            return Create().First();
        }

    }
}