using System;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchConnectionChecker : IConnectionChecker {

        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public bool Check(AbstractConnection connection) {
            var client = ElasticSearchClientFactory.Create(connection, null);
            try {
                var response = client.Client.Ping();
                if (response.HttpStatusCode != null && response.HttpStatusCode == 200) {
                    return true;
                }
                _log.Warn("Failed to connect to {0}, {1}. {2}", connection.Name, connection.Server, response.Error.ExceptionMessage);
            } catch (Exception e) {
                _log.Error("Failed to connect to {0}, {1}. {2}", connection.Name, connection.Server, e.Message);
                return false;
            }

            return false;
        }
    }
}