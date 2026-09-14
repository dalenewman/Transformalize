using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingTestSupport;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Json;

namespace Test.Unit;

[TestClass]
public class StreamingReaderTests {
   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task FirstRowDoesNotConsumeOrMaterializeWholeSource(bool lines) {
      var context = Setup.Context();
      var factory = new CountingFactory(context.RowCapacity);
      var records = Enumerable.Range(0, 10000).Select(i => $"{{\"Value\":\"row {i}\"}}");
      var text = lines ? string.Join("\n", records) : "[" + string.Join(",", records) + "]";
      using var source = new ObservedStream(Encoding.UTF8.GetBytes(text));
      IRead reader = lines ? new JsonLinesStreamReader(context, source, factory) : new JsonStreamReader(context, source, factory);
      using var cancellation = new CancellationTokenSource();
      await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator(cancellation.Token)) {
         Assert.AreEqual(0, source.BytesRead);
         Assert.IsTrue(await iterator.MoveNextAsync());
         Assert.AreEqual("row 0", iterator.Current[context.InputFields.Single(f => f.Name == "Value")]);
         Assert.AreEqual(1, factory.Created);
         Assert.IsTrue(source.BytesRead < source.Length / 2, "First row must arrive before the whole source is read.");
         cancellation.Cancel();
         await Assert.ThrowsAsync<OperationCanceledException>(async () => await iterator.MoveNextAsync());
      }
      Assert.IsFalse(source.Disposed, "Stream readers must leave the caller's stream open.");
   }

   [TestMethod]
   [DataRow(false, "utf-8")]
   [DataRow(false, "utf-16")]
   [DataRow(false, "utf-32")]
   [DataRow(true, "utf-8")]
   [DataRow(true, "utf-16")]
   public async Task PagingAndUnicodeMatchSynchronousRead(bool lines, string encodingName) {
      var context = Setup.Context("page='2' size='1'", "<add name='Value'/><add name='Number' type='int'/><add name='Nested'/>");
      var factory = new CountingFactory(context.RowCapacity);
      string[] records = ["{\"Value\":\"first\"}", "{\"Value\":\"café 😀\",\"Number\":42,\"Nested\":[1,2]}", "{\"Value\":null}"];
      var text = lines ? string.Join("\n", records) : "[" + string.Join(",", records) + "]";
      var encoding = Encoding.GetEncoding(encodingName);
      var bytes = encoding.GetPreamble().Concat(encoding.GetBytes(text)).ToArray();
      using var source = new MemoryStream(bytes);
      IRead reader = lines ? new JsonLinesStreamReader(context, source, factory) : new JsonStreamReader(context, source, factory);
      var expected = reader.Read().Single();
      var actual = (await reader.ReadStreamAsync().MaterializeAsync()).Single();
      foreach (var field in context.InputFields.Where(f => !f.System)) Assert.AreEqual(expected[field], actual[field]);
      Assert.AreEqual("café 😀", actual[context.InputFields.Single(f => f.Name == "Value")]);
   }

   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task MalformedTailFailsWhenConsumed(bool lines) {
      var context = Setup.Context();
      var factory = new CountingFactory(context.RowCapacity);
      var records = Enumerable.Repeat("{\"Value\":\"valid\"}", 4000);
      var text = lines ? string.Join("\n", records) + "\n{broken" : "[" + string.Join(",", records) + ",{broken]";
      using var source = new ObservedStream(Encoding.UTF8.GetBytes(text));
      IRead reader = lines ? new JsonLinesStreamReader(context, source, factory) : new JsonStreamReader(context, source, factory);
      await using var iterator = reader.ReadStreamAsync().GetAsyncEnumerator();
      Assert.IsTrue(await iterator.MoveNextAsync());
      await Assert.ThrowsAsync<JsonException>(async () => { while (await iterator.MoveNextAsync()) { } });
      Assert.IsFalse(source.Disposed);
   }
}
