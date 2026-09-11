using Transformalize;
using System.Text;
using System.Text.Json;
using Elastic.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingTestSupport;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Elasticsearch;
using HttpMethod = Elastic.Transport.HttpMethod;

namespace Test.Integration.Core;

[TestClass]
public class StreamingReaderTests {
   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task EarlyExitAndCancellationClearScrollWithoutReadingNextPage(bool cancel) {
      var context = Setup.Context("read-size='2'");
      context.Connection.Version = "8.12.0";
      context.Connection.Index = "rows";
      var invoker = new Pages();
      var transport = new DistributedTransport(new TransportConfiguration(new SingleNodePool(new Uri("http://localhost:9200")), invoker));
      var factory = new CountingFactory(context.RowCapacity);
      IRead reader = new ElasticReader(context, context.InputFields.Where(f => !f.System).ToArray(), transport, factory, ReadFrom.Input);
      using var cancellation = new CancellationTokenSource();
      await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator(cancellation.Token)) {
         Assert.AreEqual(0, invoker.Searches);
         Assert.IsTrue(await iterator.MoveNextAsync());
         Assert.AreEqual(1, invoker.Searches);
         Assert.AreEqual(1, factory.Created);
         Assert.AreEqual(4, context.Entity.Hits);
         if (cancel) {
            cancellation.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await iterator.MoveNextAsync());
         }
      }
      Assert.AreEqual(1, invoker.Searches);
      Assert.AreEqual(1, invoker.Clears);
      Assert.AreEqual("scroll-1", invoker.ClearedId);
      Assert.IsFalse(invoker.CleanupCanceled);
   }

   [TestMethod]
   public async Task ReadsEachPageOnceAndClearsAfterCompletion() {
      var context = Setup.Context("read-size='2'");
      context.Connection.Version = "8.12.0";
      context.Connection.Index = "rows";
      var invoker = new Pages();
      var transport = new DistributedTransport(new TransportConfiguration(new SingleNodePool(new Uri("http://localhost:9200")), invoker));
      var factory = new CountingFactory(context.RowCapacity);
      var reader = new ElasticReader(context, context.InputFields.Where(f => !f.System).ToArray(), transport, factory, ReadFrom.Input);
      var rows = await reader.ReadStreamAsync().MaterializeAsync();
      var field = context.InputFields.Single(f => f.Name == "Value");
      CollectionAssert.AreEqual(new[] { "1", "2", "3", "4" }, rows.Select(r => (string)r[field]).ToArray());
      Assert.AreEqual(3, invoker.Searches);
      Assert.AreEqual(1, invoker.Clears);
      Assert.AreEqual("scroll-3", invoker.ClearedId);
   }

   [TestMethod]
   public async Task AggregationRowsMatchSyncWithoutMaterializingAllOutputRows() {
      var context = Setup.Context(fields: "<add name='Value'/><add name='Amount' type='int'/>");
      var buckets = Enumerable.Range(0, 1000).Select(i => new { key = i.ToString(), doc_count = 1, Amount = new { value = i } });
      var json = JsonSerializer.Serialize(new { aggregations = new { Value = new { buckets } } });
      var invoker = new InMemoryRequestInvoker(Encoding.UTF8.GetBytes(json), 200);
      var transport = new DistributedTransport(new TransportConfiguration(new SingleNodePool(new Uri("http://localhost:9200")), invoker));
      var factory = new CountingFactory(context.RowCapacity);
      var reader = new ElasticQueryReader(context, transport, factory);
      await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator()) {
         Assert.IsTrue(await iterator.MoveNextAsync());
         Assert.AreEqual(1, factory.Created);
      }
      var expected = reader.Read().ToArray();
      var actual = await reader.ReadStreamAsync().MaterializeAsync();
      Assert.AreEqual(1000, actual.Count);
      for (var i = 0; i < expected.Length; i++)
         foreach (var field in context.InputFields.Where(f => !f.System)) Assert.AreEqual(expected[i][field], actual[i][field]);
   }

   private sealed class Pages : InMemoryRequestInvoker, IRequestInvoker {
      public int Searches, Clears;
      public bool CleanupCanceled;
      public string? ClearedId;
      public new Task<TResponse> RequestAsync<TResponse>(Endpoint endpoint, BoundConfiguration configuration, PostData? postData, CancellationToken token) where TResponse : TransportResponse, new() {
         string json;
         if (endpoint.Method == HttpMethod.DELETE) {
            Clears++;
            using var body = new MemoryStream();
            postData!.Write(body, configuration.ConnectionSettings, false);
            using var document = JsonDocument.Parse(body.ToArray());
            ClearedId = document.RootElement.GetProperty("scroll_id").GetString();
            CleanupCanceled = token.IsCancellationRequested;
            json = "{\"succeeded\":true,\"num_freed\":1}";
         } else {
            Searches++;
            var docs = Searches <= 2
               ? $"{{\"_source\":{{\"Value\":\"{Searches * 2 - 1}\"}}}},{{\"_source\":{{\"Value\":\"{Searches * 2}\"}}}}"
               : "";
            json = $"{{\"_scroll_id\":\"scroll-{Searches}\",\"hits\":{{\"total\":{{\"value\":4}},\"hits\":[{docs}]}}}}";
         }
         return base.BuildResponseAsync<TResponse>(endpoint, configuration, postData, token, Encoding.UTF8.GetBytes(json), 200);
      }
   }
}
