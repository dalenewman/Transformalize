namespace Transformalize.Main.Providers.ElasticSearch {
    public class ElasticSearchTflWriter : ITflWriter {
        public void Initialize(Process process) {
            var connection = process.OutputConnection;
            var entity = process.OutputConnection.TflBatchEntity(process.Name);
            new ElasticSearchEntityDropper().Drop(connection, entity);

            process.Logger.Info( "Initialized TrAnSfOrMaLiZeR {0} connection.", process.OutputConnection.Name);
        }
    }
}