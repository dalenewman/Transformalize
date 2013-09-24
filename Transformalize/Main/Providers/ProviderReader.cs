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

            if (providers.Count == 0) {
                providers.Add("sqlserver", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            }

            return providers;
        }
    }
}
