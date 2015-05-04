namespace Transformalize.Main.Providers.Lucene {

    public class LuceneTflWriter : ITflWriter {
        public void Initialize(Process process) {
            var connection = process.OutputConnection;
            var entity = process.OutputConnection.TflBatchEntity(process.Name);
            new LuceneEntityDropper().Drop(connection, entity);

            process.Logger.Info( "Initialized TrAnSfOrMaLiZeR {0} connection.", process.OutputConnection.Name);
        }
    }
}