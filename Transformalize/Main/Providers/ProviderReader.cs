using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.Ninject.Activation;

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

            if (!providers.ContainsKey("sqlserver")) {
                providers.Add("sqlserver", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            }

            if (!providers.ContainsKey("mysql")) {
                providers.Add("mysql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
            }

            if (!providers.ContainsKey("postgresql")) {
                providers.Add("postgresql", "Transformalize.Libs.Npgsql.NpgsqlConnection, Transformalize");
            }

            if (!providers.ContainsKey("analysisservices")) {
                providers.Add("analysisservices", string.Empty);
            }
            
            if (!providers.ContainsKey("file")) {
                providers.Add("file", string.Empty);
            }

            if (!providers.ContainsKey("folder")) {
                providers.Add("folder", string.Empty);
            }

            if (!providers.ContainsKey("internal")) {
                providers.Add("internal", string.Empty);
            }

            return providers;
        }
    }
}
