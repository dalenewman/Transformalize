using Transformalize.Core.Process_;
using Transformalize.Providers.AnalysisServices;
using Transformalize.Providers.MySql;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Providers
{
    public class ConnectionFactory
    {
        private readonly Process _process;

        public ConnectionFactory(Process process)
        {
            _process = process;
        }

        public IConnection Create(Configuration.ConnectionConfigurationElement element)
        {
            ProviderSetup provider;
            var type = element.Type.ToLower();

            switch (type)
            {
                case "sqlserver":
                    provider = new ProviderSetup {
                        ProviderType = "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                        L = "[",
                        R = "]"
                    };

                    return new DefaultConnection(element.Value, provider, new SqlServerCompatibilityReader())
                    {
                        ConnectionType = ConnectionType.SqlServer,
                        CompatibilityLevel = element.CompatabilityLevel,
                        BatchSize = element.BatchSize,
                        Process = _process.Name,
                        Name = element.Name
                    };

                case "mysql":
                    provider = new ProviderSetup { ProviderType = "MySql.Data.MySqlClient.MySqlConnection, MySql.Data", L = "`", R = "`" };

                    return new DefaultConnection(element.Value, provider, new MySqlCompatibilityReader())
                    {
                        ConnectionType = ConnectionType.MySql,
                        BatchSize = element.BatchSize,
                        Process = _process.Name,
                        Name = element.Name
                    };

                default:
                    return new AnalysisServicesConnection(element.Value)
                    {
                        BatchSize = element.BatchSize,
                        ConnectionType = ConnectionType.AnalysisServices,
                        CompatibilityLevel = element.CompatabilityLevel,
                        Process = _process.Name,
                        Name = element.Name
                    };
            }
        }
    }
}

