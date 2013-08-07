using System;
using Microsoft.AnalysisServices;
using Transformalize.Core;
using ConnectionType = Transformalize.Providers.ConnectionType;

namespace Transformalize.Providers.AnalysisServices
{
    public class AnalysisServicesConnection : IConnection
    {
        public AnalysisServicesConnection(string connectionString)
        {
            ConnectionString = connectionString;
            Database = string.Empty; //
            CompatibilityLevel = 100;
            Server = string.Empty; //
        }

        public int BatchSize { get; set; }
        public string ConnectionString { get; private set; }

        public bool IsReady()
        {
            bool isReady;
            var server = new Server();
            try
            {
                server.Connect(ConnectionString);
                isReady = server.Connected;
                server.Disconnect();

            }
            catch (Exception)
            {
                return false;
            }
            return isReady;
        }

        public string Database { get; private set; }
        public string Server { get; private set; }
        public int CompatibilityLevel { get; set; }
        public ConnectionType ConnectionType { get; set; }
    }
}