using System;

namespace Transformalize.Main.Providers.Solr {
    public class SolrEntityRecordsExist : IEntityRecordsExist {
        public IEntityExists EntityExists { get; set; }

        public bool RecordsExist(AbstractConnection connection, Entity entity) {

            var checker = new SolrConnectionChecker(connection.Logger);
            if (checker.Check(connection)) {
                throw new NotImplementedException();
            }
            return false;

        }
    }
}