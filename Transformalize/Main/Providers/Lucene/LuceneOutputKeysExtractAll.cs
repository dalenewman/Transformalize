using System.Collections.Generic;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneOutputKeysExtractAll : AbstractOperation {
        private readonly LuceneConnection _luceneConnection;
        private readonly Entity _entity;
        private readonly Fields _fields;

        public LuceneOutputKeysExtractAll(LuceneConnection luceneConnection, Entity entity) {
            _luceneConnection = luceneConnection;
            _entity = entity;
            _fields = entity.PrimaryKey;
            if (!_fields.HaveField(entity.Version.Alias)) {
                _fields.Add(entity.Version);
            }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var reader = LuceneIndexReaderFactory.Create(_luceneConnection, _entity, true)) {
                var docCount = reader.NumDocs();

                for (var i = 0; i < docCount; i++) {

                    if (reader.IsDeleted(i))
                        continue;

                    var doc = reader.Document(i);
                    var row = new Row();

                    foreach (var field in _fields) {
                        row[field.Alias] = Common.ConversionMap[field.SimpleType](doc.GetField(field.Alias).StringValue);
                    }

                    yield return row;
                }
            }
        }
    }
}