using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchTflWriter : WithLoggingMixin, ITflWriter {
        public void Initialize(Process process) {
            var connection = process.OutputConnection;
            var entity = process.OutputConnection.TflBatchEntity(process.Name);
            new ElasticSearchEntityDropper().Drop(connection, entity);

            TflLogger.Info(process.Name, string.Empty, "Initialized TrAnSfOrMaLiZeR {0} connection.", process.OutputConnection.Name);
        }
    }
}