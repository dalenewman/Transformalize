using System;

namespace Transformalize.Main.Providers.Solr {
    public class SolrEntityExists : IEntityExists {
        public bool Exists(AbstractConnection connection, Entity entity) {
            throw new NotImplementedException();
        }
    }
}