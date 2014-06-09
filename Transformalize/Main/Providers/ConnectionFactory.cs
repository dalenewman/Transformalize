using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.NLog;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.Console;
using Transformalize.Main.Providers.ElasticSearch;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Providers.Folder;
using Transformalize.Main.Providers.Html;
using Transformalize.Main.Providers.Internal;
using Transformalize.Main.Providers.Log;
using Transformalize.Main.Providers.Mail;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.PostgreSql;
using Transformalize.Main.Providers.Solr;
using Transformalize.Main.Providers.SqlCe4;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers {

    public class ConnectionFactory {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private Dictionary<string, string> _providers = new Dictionary<string, string>();
        private readonly IKernel _kernal = new StandardKernel(new NinjectBindings());

        public ConnectionFactory() {
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

        public InternalConnection Internal(string name) {
            return GetConnection(new ConnectionConfigurationElement() { Name = name, Provider = "internal" }) as InternalConnection;
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

            AbstractConnection connection;
            Validate(element);

            var parameters = new Libs.Ninject.Parameters.IParameter[] {
                new ConstructorArgument("element", element)
            };

            var provider = element.Provider.ToLower();

            switch (provider) {
                case "sqlserver":
                    connection = _kernal.Get<SqlServerConnection>(parameters);
                    break;
                case "mysql":
                    connection = _kernal.Get<MySqlConnection>(parameters);
                    break;
                case "postgresql":
                    connection = _kernal.Get<PostgreSqlConnection>(parameters);
                    break;
                case "analysisservices":
                    connection = _kernal.Get<AnalysisServicesConnection>(parameters);
                    break;
                case "file":
                    connection = _kernal.Get<FileConnection>(parameters);
                    break;
                case "folder":
                    connection = _kernal.Get<FolderConnection>(parameters);
                    break;
                case "internal":
                    connection = _kernal.Get<InternalConnection>(parameters);
                    break;
                case "sqlce":
                    connection = _kernal.Get<SqlCe4Connection>(parameters);
                    break;
                case "console":
                    connection = _kernal.Get<ConsoleConnection>(parameters);
                    break;
                case "log":
                    connection = _kernal.Get<LogConnection>(parameters);
                    break;
                case "mail":
                    connection = _kernal.Get<MailConnection>(parameters);
                    break;
                case "elasticsearch":
                    connection = _kernal.Get<ElasticSearchConnection>(parameters);
                    break;
                case "html":
                    connection = _kernal.Get<HtmlConnection>(parameters);
                    break;
                case "solr":
                    connection = _kernal.Get<SolrConnection>(parameters);
                    break;
                default:
                    throw new TransformalizeException("The provider '{0}' is not yet implemented.", element.Provider);
            }
            connection.TypeAndAssemblyName = Providers[provider];
            return connection;
        }

        public SqlServerConnection SqlServer(string name, string database, string server = "localhost") {
            return GetConnection(new ConnectionConfigurationElement() { Name = name, Server = server, Database = database }) as SqlServerConnection;
        }
    }
}
