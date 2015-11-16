using System;
using Transformalize.Configuration;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Transformalize.Libs.Rhino.Etl.Operations;
using Sort = Lucene.Net.Search.Sort;
using Version = Lucene.Net.Util.Version;

namespace Transformalize.Main.Providers.Lucene {
    public class LuceneConnection : AbstractConnection {

        public LuceneConnection(TflConnection element, AbstractConnectionDependencies dependencies)
            : base(element, dependencies) {
            Type = ProviderType.Lucene;
            IsDatabase = true;
            TextQualifier = string.Empty;
        }

        public override int NextBatchId(string processName) {
            if (!TflBatchRecordsExist(processName)) {
                return 1;
            }
            var searcher = LuceneSearcherFactory.Create(this, TflBatchEntity(processName));

            var query = new TermQuery(new Term("process", processName));
            var sort = new Sort(new SortField("id", SortField.INT, true));
            var hits = searcher.Search(query, null, 1, sort);

            if (hits.TotalHits <= 0)
                return 1;

            var doc = searcher.Doc(0);
            return Convert.ToInt32(doc.GetField("id").StringValue) + 1;
        }

        public override void WriteEndVersion(Process process, AbstractConnection input, Entity entity, bool force = false) {
            if (entity.Updates + entity.Inserts <= 0 && !force)
                return;

            var versionType = entity.Version == null ? "string" : entity.Version.SimpleType;
            var end = entity.End ?? new DefaultFactory(Logger).Convert(entity.End, versionType);

            using (var dir = LuceneDirectoryFactory.Create(this, TflBatchEntity(entity.ProcessName))) {
                using (var writer = new IndexWriter(dir, new KeywordAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED)) {
                    var doc = new Document();
                    doc.Add(new NumericField("id", global::Lucene.Net.Documents.Field.Store.YES, true).SetIntValue(entity.TflBatchId));
                    doc.Add(new global::Lucene.Net.Documents.Field("process", entity.ProcessName, global::Lucene.Net.Documents.Field.Store.YES, global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.Add(new global::Lucene.Net.Documents.Field("connection", input.Name, global::Lucene.Net.Documents.Field.Store.YES, global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.Add(new global::Lucene.Net.Documents.Field("entity", entity.Alias, global::Lucene.Net.Documents.Field.Store.YES, global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.Add(new NumericField("updates", global::Lucene.Net.Documents.Field.Store.YES, true).SetLongValue(entity.Updates));
                    doc.Add(new NumericField("inserts", global::Lucene.Net.Documents.Field.Store.YES, true).SetLongValue(entity.Inserts));
                    doc.Add(new NumericField("deletes", global::Lucene.Net.Documents.Field.Store.YES, true).SetLongValue(entity.Deletes));
                    doc.Add(LuceneWriter.CreateField("version", versionType, new SearchType { Analyzer = "keyword" }, end));
                    doc.Add(new global::Lucene.Net.Documents.Field("version_type", versionType, global::Lucene.Net.Documents.Field.Store.YES, global::Lucene.Net.Documents.Field.Index.NOT_ANALYZED_NO_NORMS));
                    doc.Add(new NumericField("tflupdate", global::Lucene.Net.Documents.Field.Store.YES, true).SetLongValue(DateTime.UtcNow.Ticks));
                    writer.AddDocument(doc);
                    writer.Commit();
                    writer.Optimize();
                }
            }
        }

        public override IOperation ExtractCorrespondingKeysFromOutput(Entity entity) {
            return new LuceneKeysExtractAll(this, entity, input: false);
        }

        public override IOperation ExtractAllKeysFromOutput(Entity entity) {
            return new LuceneKeysExtractAll(this, entity, input: false);
        }

        public override IOperation ExtractAllKeysFromInput(Process process, Entity entity) {
            return new LuceneKeysExtractAll(this, entity, input: true);
        }

        public override IOperation Insert(Process process, Entity entity) {
            return new LuceneLoadOperation(this, entity);
        }

        public override IOperation Update(Entity entity) {
            return new LuceneLoadOperation(this, entity, true);
        }

        public override void LoadBeginVersion(Entity entity) {
            using (var searcher = LuceneSearcherFactory.Create(this, TflBatchEntity(entity.ProcessName))) {

                var parser = new MultiFieldQueryParser(LuceneVersion(), new[] { "process", "entity" }, new KeywordAnalyzer());
                var query = parser.Parse($"process:\"{entity.ProcessName}\" AND entity:\"{entity.Alias}\"");
                var sort = new Sort(new SortField("id", SortField.INT, true));
                var hits = searcher.Search(query, null, 1, sort);

                entity.HasRange = hits.TotalHits > 0;

                if (!entity.HasRange)
                    return;

                var doc = searcher.Doc(0);
                var type = doc.GetField("version_type").StringValue;
                entity.Begin = Common.GetObjectConversionMap()[type](doc.GetField("version").StringValue);
            }
        }

        public override void LoadEndVersion(Entity entity) {
            using (var searcher = LuceneSearcherFactory.Create(this, entity)) {
                var query = new MatchAllDocsQuery();
                var sortType = LuceneWriter.SortMap.ContainsKey(entity.Version.SimpleType) ? LuceneWriter.SortMap[entity.Version.SimpleType] : LuceneWriter.SortMap["*"];
                var sort = new Sort(new SortField(entity.Version.Alias, sortType, true));
                var hits = searcher.Search(query, null, 1, sort);

                entity.HasRows = hits.TotalHits > 0;

                if (!entity.HasRows)
                    return;

                var doc = searcher.Doc(0);
                entity.End = Common.GetObjectConversionMap()[entity.Version.SimpleType](doc.Get(entity.Version.Alias));
            }
        }

        public override Fields GetEntitySchema(Process process, Entity entity, bool isMaster = false) {
            throw new NotImplementedException();
        }

        public override IOperation Delete(Entity entity) {
            return new LuceneEntityDelete(this, entity);
        }

        public override IOperation Extract(Process process, Entity entity, bool firstRun) {
            return new LuceneExtract(this, entity);
        }

        public static Version GetLuceneVersion(string version) {
            Version v;
            if (version == Common.DefaultValue || !Enum.TryParse(version, true, out v)) {
                v = global::Lucene.Net.Util.Version.LUCENE_30;
            }
            return v;
        }

        public Version LuceneVersion() {
            return GetLuceneVersion(this.Version);
        }

    }
}