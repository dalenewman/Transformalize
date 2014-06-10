using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers {

    public class ConnectionFactory {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private Dictionary<string, string> _providers = new Dictionary<string, string>();
        private readonly IKernel _kernal;

        public ConnectionFactory(IKernel kernal = null) {
            _kernal = kernal ?? new StandardKernel(new NinjectBindings());
            _providers.Add("sqlserver", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            _providers.Add("sqlce", "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe");
            _providers.Add("mysql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
            _providers.Add("postgresql", "Npgsql.NpgsqlConnection, Npgsql");
            var empties = new[] { "analysisservices", "file", "folder", "internal", "console", "log", "mail", "html", "elasticsearch", "solr" };
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
                    _log.Error(result.Message);
                }
                throw new TransformalizeException("Connection validation failed. See error log.");
            }
        }

        private AbstractConnection GetConnection(ConnectionConfigurationElement element) {
            Validate(element);
            var parameters = new Libs.Ninject.Parameters.IParameter[] {
                new ConstructorArgument("element", element)
            };
            var provider = element.Provider.ToLower();
            var connection = _kernal.Get<AbstractConnection>(provider, parameters);
            connection.TypeAndAssemblyName = Providers[provider];
            return connection;
        }

    }
}
