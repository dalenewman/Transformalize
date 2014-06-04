using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;
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
        private readonly Process _process;
        private readonly ConnectionElementCollection _elements;

        public ConnectionFactory(Process process, ConnectionElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public AbstractConnection Create(ConnectionConfigurationElement element) {
            return GetConnection(element);
        }

        public Dictionary<string, AbstractConnection> Create() {
            var connections = new Dictionary<string, AbstractConnection>();
            foreach (ConnectionConfigurationElement element in _elements) {
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
                    _process.ValidationResults.AddResult(result);
                    _log.Error(result.Message);
                }
                throw new TransformalizeException("Connection validation failed. See error log.");
            }
        }

        private AbstractConnection GetConnection(ConnectionConfigurationElement element) {

            Validate(element);

            var parameters = new Libs.Ninject.Parameters.IParameter[] {
                    new ConstructorArgument("process", _process),
                    new ConstructorArgument("element", element)
                };

            switch (element.Provider.ToLower()) {
                case "sqlserver":
                    return _process.Kernal.Get<SqlServerConnection>(parameters);
                case "mysql":
                    return _process.Kernal.Get<MySqlConnection>(parameters);
                case "postgresql":
                    return _process.Kernal.Get<PostgreSqlConnection>(parameters);
                case "analysisservices":
                    return _process.Kernal.Get<AnalysisServicesConnection>(parameters);
                case "file":
                    return _process.Kernal.Get<FileConnection>(parameters);
                case "folder":
                    return _process.Kernal.Get<FolderConnection>(parameters);
                case "internal":
                    return _process.Kernal.Get<InternalConnection>(parameters);
                case "sqlce":
                    return _process.Kernal.Get<SqlCe4Connection>(parameters);
                case "console":
                    return _process.Kernal.Get<ConsoleConnection>(parameters);
                case "log":
                    return _process.Kernal.Get<LogConnection>(parameters);
                case "mail":
                    return _process.Kernal.Get<MailConnection>(parameters);
                case "elasticsearch":
                    return _process.Kernal.Get<ElasticSearchConnection>(parameters);
                case "html":
                    return _process.Kernal.Get<HtmlConnection>(parameters);
                case "solr":
                    return _process.Kernal.Get<SolrConnection>(parameters);
                default:
                    throw new TransformalizeException("The provider '{0}' is not yet implemented.", element.Provider);
            }
        }
    }
}
