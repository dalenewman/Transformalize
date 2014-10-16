using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;

namespace Transformalize.Main.Providers {

    public class ConnectionFactory {
        private readonly Process _process;
        private Dictionary<string, string> _providers = new Dictionary<string, string>();

        public ConnectionFactory(Process process) {
            _process = process;
            _providers.Add("sqlserver", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            _providers.Add("sqlce", "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe");
            _providers.Add("mysql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
            _providers.Add("postgresql", "Npgsql.NpgsqlConnection, Npgsql");
            var empties = new[] {
                "analysisservices",
                "file",
                "folder",
                "internal",
                "console",
                "log",
                "html",
                "elasticsearch",
                "solr",
                "lucene",
                "mail",
                "web"
            };
            foreach (var empty in empties) {
                _providers.Add(empty, empty);
            }
        }

        public Dictionary<string, string> Providers {
            get { return _providers; }
            set { _providers = value; }
        }

        public AbstractConnection Create(ConnectionConfigurationElement element) {
            return GetConnection(element);
        }

        public Dictionary<string, AbstractConnection> Create(ConnectionElementCollection elements) {
            var connections = new Dictionary<string, AbstractConnection>();
            foreach (ConnectionConfigurationElement element in elements) {
                Validate(element);
                connections.Add(element.Name, GetConnection(element));
            }
            return connections;
        }

        private void Validate(ConnectionConfigurationElement element) {
            var validator = ValidationFactory.CreateValidator<ConnectionConfigurationElement>();
            var results = validator.Validate(element);
            if (!results.IsValid) {
                foreach (var result in results) {
                    TflLogger.Error(_process.Name, string.Empty, result.Message);
                }
                throw new TransformalizeException("Connection validation failed. See error log.");
            }
        }

        private AbstractConnection GetConnection(ConnectionConfigurationElement element) {
            Validate(element);
            var parameters = new IParameter[] {
                new ConstructorArgument("element", element)
            };
            var provider = element.Provider.ToLower();
            var connection = _process.Kernal.Get<AbstractConnection>(provider, parameters);
            connection.TypeAndAssemblyName = Providers[provider];
            return connection;
        }

    }
}
