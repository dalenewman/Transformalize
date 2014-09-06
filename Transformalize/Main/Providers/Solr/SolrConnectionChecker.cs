using System;
using System.Collections.Generic;
using System.Net;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.Solr {

    public class SolrConnectionChecker : IConnectionChecker {

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private static readonly Dictionary<int, bool> Checks = new Dictionary<int, bool>();

        public bool Check(AbstractConnection connection) {

            var solrConnection = (SolrConnection)connection;

            var hashCode = connection.Uri().GetHashCode();
            if (Checks.ContainsKey(hashCode)) {
                return Checks[hashCode];
            }

            try {
                new WebClient().DownloadString(solrConnection.GetPingUrl());
                Checks[hashCode] = true;
                return true;
            } catch (Exception e) {
                _log.Warn("Failed to connect to {0}. Pinging {1} resulted in: {3}", connection.Name, solrConnection.GetPingUrl(), e.Message);
                return false;
            }
        }
    }
}