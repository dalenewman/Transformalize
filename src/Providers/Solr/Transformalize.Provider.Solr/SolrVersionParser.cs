using System;
using Transformalize.Contracts;

namespace Transformalize.Providers.Solr {
    public static class SolrVersionParser {

        public static Version ParseVersion(IConnectionContext context) {
            if (context.Connection.Version == Constants.DefaultSetting || context.Connection.Version == string.Empty) {
                context.Warn("Defaulting to SOLR version 7.7.3");
                context.Connection.Version = "7.7.3";
            }

            if (Version.TryParse(context.Connection.Version, out var parsed)) {
                return parsed;
            }

            context.Warn($"Unable to parse SOLR {context.Connection.Version}.");
            context.Connection.Version = "7.7.3";
            return new Version(7, 7, 3, 0);
        }

   }
}
