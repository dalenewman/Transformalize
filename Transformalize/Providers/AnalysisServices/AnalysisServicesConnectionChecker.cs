using System;
using Microsoft.AnalysisServices;

namespace Transformalize.Providers.AnalysisServices
{
    public class AnalysisServicesConnectionChecker : IConnectionChecker
    {
        public bool Check(IConnection connection)
        {
            bool isReady;
            var server = new Server();
            try
            {
                server.Connect(connection.ConnectionString);
                isReady = server.Connected;
                server.Disconnect();
            }
            catch (Exception)
            {
                return false;
            }
            return isReady;
        }
    }
}