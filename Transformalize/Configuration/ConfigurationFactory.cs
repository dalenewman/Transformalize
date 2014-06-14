using System;
using System.IO;
using Transformalize.Main;
using Transformalize.Runner;

namespace Transformalize.Configuration
{
    public class ConfigurationFactory {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly string _name;

        public ConfigurationFactory(string name) {
            _name = name;
        }

        public ProcessElementCollection Create() {
            return CreateReader().Read();
        }

        private IReader<ProcessElementCollection> CreateReader()
        {
            var name = _name.Contains("?") ? _name.Substring(0, _name.IndexOf('?')) : _name;
            if (Path.HasExtension(name)) {
                return _name.StartsWith("http", IC) ?
                    new ProcessXmlConfigurationReader(_name, new ContentsWebReader()) :
                    new ProcessXmlConfigurationReader(_name, new ContentsFileReader());
            }
            return new ProcessConfigurationReader(_name);
        }
    }
}