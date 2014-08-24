using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {

    public class LuceneLoadOperation : AbstractOperation {
        private readonly LuceneConnection _luceneConnection;
        private readonly Entity _entity;
        private readonly bool _deleteFirst;
        private readonly Dictionary<string, KeyValuePair<Field, SearchType>> _map;
        private readonly Field[] _primaryKey;

        public LuceneLoadOperation(LuceneConnection luceneConnection, Entity entity, bool deleteFirst = false) {
            _luceneConnection = luceneConnection;
            _entity = entity;
            _primaryKey = entity.PrimaryKey.ToArray();
            _deleteFirst = deleteFirst;
            _map = LuceneWriter.CreateFieldMap(entity);
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            using (var writer = LuceneWriter.Create(_luceneConnection, _entity)) {
                foreach (var row in rows) {
                    if (_deleteFirst) {
                        writer.DeleteDocuments(LuceneWriter.CreateQuery(_primaryKey, row));
                    }
                    writer.AddDocument(LuceneWriter.CreateDocument(_map, row));
                }
                writer.Commit();
                writer.Optimize();
            }
            yield break;
        }

    }
}