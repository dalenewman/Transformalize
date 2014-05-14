using System.Collections.Generic;
using System.Linq;
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

            if (!providers.ContainsKey("sqlserver")) {
                providers.Add("sqlserver", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            }

            if (!providers.ContainsKey("sqlce4")) {
                providers.Add("sqlce4", "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe");
            }

            if (!providers.ContainsKey("mysql")) {
                providers.Add("mysql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
            }

            if (!providers.ContainsKey("postgresql")) {
                providers.Add("postgresql", "Npgsql.NpgsqlConnection, Npgsql");
            }

            var empties = new[] {"analysisservices", "file", "folder", "internal", "console", "log", "mail", "html", "elasticsearch","solr"};
            foreach (var empty in empties.Where(empty => !providers.ContainsKey(empty))) {
                providers.Add(empty, empty);
            }

            //remember to update ConnectionFactory too.

            return providers;
        }
    }
}
