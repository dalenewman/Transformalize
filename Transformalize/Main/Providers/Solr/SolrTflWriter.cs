namespace Transformalize.Main.Providers.Solr {

    public class SolrTflWriter : ITflWriter {

        public void Initialize(Process process) {
            var connection = process.OutputConnection;
            var entity = process.OutputConnection.TflBatchEntity(process.Name);
            new SolrEntityDropper().Drop(connection, entity);

            process.Logger.Info("Initialized TrAnSfOrMaLiZeR {0} connection.", process.OutputConnection.Name);
        }
    }
}