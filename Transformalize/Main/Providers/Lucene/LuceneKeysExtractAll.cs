using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Main.Providers.Lucene {

    public class LuceneKeysExtractAll : AbstractOperation {

        private readonly LuceneConnection _luceneConnection;
        private readonly Entity _entity;
        private readonly bool _input;
        private readonly Fields _fields;
        private readonly string[] _selected;

        public LuceneKeysExtractAll(LuceneConnection luceneConnection, Entity entity, bool input = false) {
            _luceneConnection = luceneConnection;
            _entity = entity;
            _input = input;
            _fields = entity.PrimaryKey;
            if (entity.Version != null && !_fields.HaveField(entity.Version.Alias)) {
                _fields.Add(entity.Version);
            }
            _selected = _fields.Select(f => input ? f.Name : f.Alias).ToArray();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            using (var reader = LuceneReaderFactory.Create(_luceneConnection, _entity, true)) {
                var docCount = reader.NumDocs();

                for (var i = 0; i < docCount; i++) {

                    if (reader.IsDeleted(i))
                        continue;

                    var doc = reader.Document(i, new MapFieldSelector(_selected));

                    var row = new Row();

                    foreach (var field in _fields) {
                        row[field.Alias] = Common.ConversionMap[field.SimpleType](doc.Get(_input ? field.Name : field.Alias));
                    }

                    yield return row;
                }
            }
        }
    }
}