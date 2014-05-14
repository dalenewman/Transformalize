using System;
using System.Collections.Generic;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.Solr {

    public class SolrConnectionChecker : IConnectionChecker {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private static readonly Dictionary<int, bool> Checks = new Dictionary<int, bool>();

        public bool Check(AbstractConnection connection) {

            var hashCode = connection.Uri().GetHashCode();
            if (Checks.ContainsKey(hashCode)) {
                return Checks[hashCode];
            }

            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                _log.Warn("Failed to connect to {0}, {1}:{2}. {3}", connection.Name, connection.Server, connection.Port, e.Message);
                return false;
            }

            return false;
        }
    }
}