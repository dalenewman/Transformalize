using Transformalize;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingTestSupport;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Lucene;
using Field = Lucene.Net.Documents.Field;

namespace IntegrationTests;

[TestClass]
public class StreamingReaderTests {
   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task BoundedPagesUseActualHitIdsAndSkipDeletedDocuments(bool filtered) {
      var folder = Path.Combine(Path.GetTempPath(), "stream-lucene-" + Guid.NewGuid().ToString("N"));
      try {
         var context = Setup.Context("read-size='2'");
         using var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
         var directoryFactory = new DirectoryFactory(folder);
         var writerFactory = new IndexWriterFactory(directoryFactory, analyzer);
         using (var writer = writerFactory.Create()) {
            for (var i = 0; i < 20; i++) writer.AddDocument(new Document { new StringField("Value", i.ToString(), Field.Store.YES) });
            writer.DeleteDocuments(new Term("Value", "5"));
            writer.Commit();
         }
         if (filtered) context.Entity.Filter.Add(new Transformalize.Configuration.Filter { Field = "Value", Value = "17" });
         var factory = new CountingFactory(context.RowCapacity);
         var readerFactory = new IndexReaderFactory(directoryFactory, writerFactory);
         using var reader = new LuceneReader(context, context.InputFields.Where(f => !f.System), new Transformalize.Providers.Lucene.SearcherFactory(readerFactory), analyzer, readerFactory, factory, ReadFrom.Input);
         await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator()) {
            Assert.AreEqual(0, factory.Created);
            Assert.IsTrue(await iterator.MoveNextAsync());
            Assert.AreEqual(1, factory.Created);
         }
         var rows = await reader.ReadStreamAsync().MaterializeAsync();
         var field = context.InputFields.Single(f => f.Name == "Value");
         var expected = filtered ? new[] { "17" } : Enumerable.Range(0, 20).Where(i => i != 5).Select(i => i.ToString()).ToArray();
         CollectionAssert.AreEqual(expected, rows.Select(r => (string)r[field]).ToArray());
      } finally {
         if (Directory.Exists(folder)) Directory.Delete(folder, true);
      }
   }
}
