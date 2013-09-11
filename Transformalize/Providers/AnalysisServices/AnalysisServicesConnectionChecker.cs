using System;
using Microsoft.AnalysisServices;

namespace Transformalize.Providers.AnalysisServices
{
    public class AnalysisServicesConnectionChecker : AbstractConnectionChecker
    {
        public bool Check(AbstractConnection connection)
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