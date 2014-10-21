using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;

namespace Transformalize.Main.Providers.Solr
{
    public class SolrTflWriter : WithLoggingMixin, ITflWriter {
        public void Initialize(Process process) {
            var connection = process.OutputConnection;
            var entity = process.OutputConnection.TflBatchEntity(process.Name);
            new SolrEntityDropper().Drop(connection, entity);

            TflLogger.Info(process.Name, string.Empty, "Initialized TrAnSfOrMaLiZeR {0} connection.", process.OutputConnection.Name);
        }
    }
}