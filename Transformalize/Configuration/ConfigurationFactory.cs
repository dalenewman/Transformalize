using System.Collections.Generic;
using System.IO;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Runner;

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
            return CreateReader().Read(_parameters);
        }

        public TflProcess CreateSingle() {
            return CreateReader().Read(_parameters)[0];
        }

        private IReader<List<TflProcess>> CreateReader() {

            var source = DetermineConfigurationSource(_resource);

            switch (source) {
                case ConfigurationSource.Xml:
                    return new ProcessXmlConfigurationReader(_resource, new ContentsStringReader(), _logger);
                case ConfigurationSource.WebFile:
                    return new ProcessXmlConfigurationReader(_resource, new ContentsWebReader(_logger), _logger);
                default:
                    var name = _resource.Contains("?") ? _resource.Substring(0, _resource.IndexOf('?')) : _resource;
                    if (Path.HasExtension(name)) {
                        return new ProcessXmlConfigurationReader(_resource, new ContentsFileReader(_logger), _logger);
                    }

                    throw new TransformalizeException(_logger, "{0} is invalid configuration.", _resource);
            }

        }

        public static ConfigurationSource DetermineConfigurationSource(string resource) {
            if (resource.StartsWith("<")) {
                return ConfigurationSource.Xml;
            }

            return resource.StartsWith("http") ?
                ConfigurationSource.WebFile :
                ConfigurationSource.LocalFile;
        }

    }
}