using System;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elasticsearch {
    public static class ElasticVersionParser {

        public static Version ParseVersion(IConnectionContext context) {

            if (context.Connection.Version == Constants.DefaultSetting || context.Connection.Version == string.Empty) {
                context.Warn("Defaulting to Elasticsearch version 5.0.0");
                context.Connection.Version = "5.0.0";
            }

            if (Version.TryParse(context.Connection.Version, out var parsed)) {
                return parsed;
            }

            context.Warn($"Unable to parse Elasticsearch version {context.Connection.Version}.");
            context.Connection.Version = "5.0.0";
            return new Version(5, 0, 0, 0);
        }
    }
}
