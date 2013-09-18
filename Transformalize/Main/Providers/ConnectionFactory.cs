using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main.Providers {

    public class ConnectionFactory {
        private readonly Process _process;
        private readonly ConnectionElementCollection _elements;

        public ConnectionFactory(Process process, ConnectionElementCollection elements)
        {
            _process = process;
            _elements = elements;
        }

        public Dictionary<string, AbstractConnection> Create()
        {
            var connections = new Dictionary<string, AbstractConnection>();
            var processArgument = new ConstructorArgument("process", _process);

            foreach (ConnectionConfigurationElement element in _elements) {

                var elementArgument = new ConstructorArgument("element", element);
                var parameters = new Libs.Ninject.Parameters.IParameter[] { processArgument, elementArgument };

                switch (element.Provider.ToLower()) {
                    case "mysql":
                        connections.Add(element.Name, _process.Kernal.Get<MySqlConnection>(parameters));
                        break;
                    case "analysisservices":
                        connections.Add(element.Name, _process.Kernal.Get<AnalysisServicesConnection>(parameters));
                        break;
                    default:
                        connections.Add(element.Name, _process.Kernal.Get<SqlServerConnection>(parameters));
                        break;
                }
            }
            return connections;
        } 
    }
}
