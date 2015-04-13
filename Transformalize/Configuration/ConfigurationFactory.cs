using System.Collections.Generic;
using System.IO;
using Transformalize.Main;
using Transformalize.Runner;

namespace Transformalize.Configuration {

    public class ConfigurationFactory {

        private readonly Dictionary<string, string> _parameters;
        private readonly string _resource;

        public ConfigurationFactory(string resource, Dictionary<string, string> parameters = null) {
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
                    return new ProcessXmlConfigurationReader(_resource, new ContentsStringReader());
                case ConfigurationSource.WebFile:
                    return new ProcessXmlConfigurationReader(_resource, new ContentsWebReader());
                default:
                    var name = _resource.Contains("?") ? _resource.Substring(0, _resource.IndexOf('?')) : _resource;
                    if (Path.HasExtension(name)) {
                        return new ProcessXmlConfigurationReader(_resource, new ContentsFileReader());
                    }

                    throw new TransformalizeException(string.Empty, string.Empty, "{0} is invalid configuration.", _resource);
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