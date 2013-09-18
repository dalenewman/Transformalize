using System.Collections.Generic;
using Transformalize.Configuration;

namespace Transformalize.Main.Providers {
    public class ProviderReader {
        private readonly ProviderElementCollection _elements;

        public ProviderReader(ProviderElementCollection elements) {
            _elements = elements;
        }

        public Dictionary<string, string> Read() {

            var providers = new Dictionary<string, string>();

            foreach (ProviderConfigurationElement element in _elements) {
                providers[element.Name.ToLower()] = element.Type;
            }

            return providers;
        }
    }
}
