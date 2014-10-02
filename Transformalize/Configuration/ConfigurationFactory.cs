using System.Collections.Specialized;
using System.IO;
using Transformalize.Main;
using Transformalize.Runner;

namespace Transformalize.Configuration {
    public class ConfigurationFactory {
        private readonly NameValueCollection _query;
        private readonly string _resource;

        /// <summary>
        /// Turns configuration into a collection of processes.
        /// </summary>
        /// <param name="resource">May be a named process in your application *.config, an xml file name, a url pointing to an xml file on a web server, or the xml configuration itself.  Way too many choices!</param>
        /// <param name="query">an optional collection of named values for replacing $(parameter) place-holders in the configuration.</param>
        public ConfigurationFactory(string resource, NameValueCollection query = null) {
            _resource = resource.Trim();
            _query = query;
        }

        public ProcessElementCollection Create() {
            return CreateReader().Read();
        }

        private IReader<ProcessElementCollection> CreateReader() {

            if (_resource.StartsWith("<")) {
                return new ProcessXmlConfigurationReader(_resource, new ContentsStringReader(_query));
            }

            if (_resource.StartsWith("http")) {
                return new ProcessXmlConfigurationReader(_resource, new ContentsWebReader());
            }

            var name = _resource.Contains("?") ? _resource.Substring(0, _resource.IndexOf('?')) : _resource;
            if (Path.HasExtension(name)) {
                return new ProcessXmlConfigurationReader(_resource, new ContentsFileReader());
            }

            return new ProcessConfigurationReader(_resource);
        }

    }
}