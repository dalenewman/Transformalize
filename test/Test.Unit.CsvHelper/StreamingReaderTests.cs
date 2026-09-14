using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingTestSupport;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.CsvHelper;

namespace Test.Unit;

[TestClass]
public class StreamingReaderTests {
   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task FirstRecordArrivesBeforeEofAndOwnedReaderCloses(bool cancel) {
      var context = Setup.Context(fields: "<add name='Value' type='int'/>");
      var factory = new CountingFactory(context.RowCapacity);
      using var source = new ObservedStream(Encoding.UTF8.GetBytes(string.Join("\n", Enumerable.Range(1, 10000))));
      IRead reader = new CsvHelperStreamReader(context, new StreamReader(source), factory);
      using var cancellation = new CancellationTokenSource();
      await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator(cancellation.Token)) {
         Assert.AreEqual(0, source.BytesRead);
         Assert.IsTrue(await iterator.MoveNextAsync());
         Assert.AreEqual(1, iterator.Current[context.InputFields.Single(f => f.Name == "Value")]);
         Assert.AreEqual(1, factory.Created);
         Assert.IsTrue(source.BytesRead < source.Length / 2);
         if (cancel) {
            cancellation.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await iterator.MoveNextAsync());
         }
      }
      Assert.IsTrue(source.Disposed);
   }

   [TestMethod]
   public async Task QuotedMultilineRecordsAndPagingMatchSync() {
      var context = Setup.Context("page='2' size='1'");
      var factory = new CountingFactory(context.RowCapacity);
      const string csv = "first\n\"second, with\nnewline\"\nthird";
      var expected = new CsvHelperStreamReader(context, new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv))), factory).Read().Single();
      var actual = (await new CsvHelperStreamReader(context, new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csv))), factory).ReadStreamAsync().MaterializeAsync()).Single();
      var field = context.InputFields.Single(f => f.Name == "Value");
      Assert.AreEqual(expected[field], actual[field]);
      Assert.AreEqual("second, with\nnewline", actual[field]);
   }
}
