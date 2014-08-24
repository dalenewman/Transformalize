using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneEntityDelete : AbstractOperation {
        private readonly LuceneConnection _luceneConnection;
        private readonly Entity _entity;
        private readonly Field[] _primaryKey;
        private readonly bool _isMaster;

        public LuceneEntityDelete(LuceneConnection luceneConnection, Entity entity) {
            _luceneConnection = luceneConnection;
            _entity = entity;
            _primaryKey = entity.PrimaryKey.ToArray();
            _isMaster = entity.IsMaster();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var writer = LuceneWriter.Create(_luceneConnection, _entity)) {
                foreach (var row in rows) {
                    writer.DeleteDocuments(LuceneWriter.CreateQuery(_primaryKey, row));
                }
                writer.Commit();
                writer.Optimize();
            }
            yield break;
        }
    }
}