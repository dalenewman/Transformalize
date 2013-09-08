using Transformalize.Configuration;
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

        public IConnection Create(ConnectionConfigurationElement element)
        {
            var provider = element.Provider.ToLower();

            switch (provider)
            {
                case "sqlserver":
                    return GetDefaultConnection(element,
                        new ProviderSetup(new SqlServerCompatibilityReader()) { ProviderType = _process.Providers[provider], L = "[", R = "]" }
                    );

                case "mysql":
                    return GetDefaultConnection(element,
                        new ProviderSetup(new MySqlCompatibilityReader()) { ProviderType = _process.Providers[provider], L = "`", R = "`" }
                    );

                default:
                    return new AnalysisServicesConnection(element.Value)
                    {
                        BatchSize = element.BatchSize,
                        CompatibilityLevel = element.CompatabilityLevel,
                        Process = _process.Name,
                        Name = element.Name
                    };
            }
        }

        private IConnection GetDefaultConnection(ConnectionConfigurationElement element, ProviderSetup providerSetup)
        {
            return new DefaultConnection(element.Value, providerSetup)
                       {
                           CompatibilityLevel = element.CompatabilityLevel,
                           BatchSize = element.BatchSize,
                           Process = _process.Name,
                           Name = element.Name
                       };
        }
    }
}

