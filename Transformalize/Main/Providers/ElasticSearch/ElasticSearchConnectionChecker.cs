using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchConnectionChecker : IConnectionChecker {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private static readonly Dictionary<int, bool> Checks = new Dictionary<int, bool>();

        public bool Check(AbstractConnection connection) {

            var hashCode = connection.Uri().GetHashCode();
            if (Checks.ContainsKey(hashCode)) {
                return Checks[hashCode];
            }

            var client = ElasticSearchClientFactory.Create(connection, null);
            try {
                var response = client.Client.Ping();
                if (response.HttpStatusCode != null && response.HttpStatusCode == 200) {
                    _log.Debug("Successful ping of {0}.", connection.Name);
                    return true;
                }
                _log.Warn("Failed to connect to {0}, {1}:{2}. {3}", connection.Name, connection.Server, connection.Port, response.Error.ExceptionMessage);
            } catch (Exception e) {
                _log.Warn("Failed to connect to {0}, {1}:{2}. {3}", connection.Name, connection.Server, connection.Port, e.Message);
                return false;
            }

            return false;
        }
    }
}