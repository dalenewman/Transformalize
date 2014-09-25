using System;
using System.Collections.Generic;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchConnectionChecker : IConnectionChecker {

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
                    TflLogger.Debug(string.Empty, string.Empty, "Successful ping of {0}.", connection.Name);
                    return true;
                }
                TflLogger.Warn(string.Empty, string.Empty, "Failed to connect to {0}, {1}:{2}. {3}", connection.Name, connection.Server, connection.Port, response.ServerError.Error);
            } catch (Exception e) {
                TflLogger.Warn(string.Empty, string.Empty, "Failed to connect to {0}, {1}:{2}. {3}", connection.Name, connection.Server, connection.Port, e.Message);
                return false;
            }

            return false;
        }
    }
}