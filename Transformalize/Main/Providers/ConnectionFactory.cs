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
using Transformalize.Main.Providers.SqlCe4;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers {

    public class ConnectionFactory {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly Process _process;
        private readonly ConnectionElementCollection _elements;

        public ConnectionFactory(Process process, ConnectionElementCollection elements) {
            _process = process;
            _elements = elements;
        }

        public Dictionary<string, AbstractConnection> Create() {
            var connections = new Dictionary<string, AbstractConnection>();

            foreach (ConnectionConfigurationElement element in _elements) {

                Validate(element);

                var parameters = new Libs.Ninject.Parameters.IParameter[] {
                    new ConstructorArgument("process", _process),
                    new ConstructorArgument("element", element)
                };

                switch (element.Provider.ToLower()) {
                    case "sqlserver":
                        connections.Add(element.Name, _process.Kernal.Get<SqlServerConnection>(parameters));
                        break;
                    case "mysql":
                        connections.Add(element.Name, _process.Kernal.Get<MySqlConnection>(parameters));
                        break;
                    case "postgresql":
                        connections.Add(element.Name, _process.Kernal.Get<PostgreSqlConnection>(parameters));
                        break;
                    case "analysisservices":
                        connections.Add(element.Name, _process.Kernal.Get<AnalysisServicesConnection>(parameters));
                        break;
                    case "file":
                        connections.Add(element.Name, _process.Kernal.Get<FileConnection>(parameters));
                        break;
                    case "folder":
                        connections.Add(element.Name, _process.Kernal.Get<FolderConnection>(parameters));
                        break;
                    case "internal":
                        connections.Add(element.Name, _process.Kernal.Get<InternalConnection>(parameters));
                        break;
                    case "sqlce4":
                        connections.Add(element.Name, _process.Kernal.Get<SqlCe4Connection>(parameters));
                        break;
                    case "console":
                        connections.Add(element.Name, _process.Kernal.Get<ConsoleConnection>(parameters));
                        break;
                    case "log":
                        connections.Add(element.Name, _process.Kernal.Get<LogConnection>(parameters));
                        break;
                    case "mail":
                        connections.Add(element.Name, _process.Kernal.Get<MailConnection>(parameters));
                        break;
                    case "elasticsearch":
                        connections.Add(element.Name, _process.Kernal.Get<ElasticSearchConnection>(parameters));
                        break;
                    case "html":
                        connections.Add(element.Name, _process.Kernal.Get<HtmlConnection>(parameters));
                        break;
                    default:
                        _log.Warn("The provider '{0}' is not yet implemented.", element.Provider);
                        break;
                }
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
                LogManager.Flush();
                System.Environment.Exit(1);
            }
        }
    }
}
