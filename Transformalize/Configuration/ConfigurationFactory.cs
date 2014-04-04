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
            return Reader().Read();
        }

        private IReader<ProcessElementCollection> Reader() {
            if (Path.HasExtension(_name)) {
                return _name.StartsWith("http", IC) ?
                    new ProcessXmlConfigurationReader(_name, new ContentsWebReader()) :
                    new ProcessXmlConfigurationReader(_name, new ContentsFileReader());
            }
            return new ProcessConfigurationReader(_name);
        }
    }
}