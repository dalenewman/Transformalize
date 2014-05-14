using System;
using Transformalize.Libs.NLog;

namespace Transformalize.Main.Providers.Solr {

    public class SolrEntityDropper : IEntityDropper {
        private readonly Logger _log = LogManager.GetLogger("tfl");
        public IEntityExists EntityExists { get; set; }

        public SolrEntityDropper() {
            EntityExists = new SolrEntityExists();
        }

        public void Drop(AbstractConnection connection, Entity entity) {
            if (!EntityExists.Exists(connection, entity))
                return;
            throw new NotImplementedException();
        }
    }
}