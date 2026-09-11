using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingTestSupport;
using Transformalize.Extensions;
using Transformalize.Providers.Console;
using Transformalize.Providers.File;

namespace Tests;

[TestClass]
public class SourceStreamingTests {
   [TestMethod]
   [DataRow("")]
   [DataRow("^record")]
   public async Task FileRecordsMatchSyncAndCreateOnlyDemandedRows(string pattern) {
      var path = Path.Combine(Path.GetTempPath(), "stream-file-" + Guid.NewGuid().ToString("N") + ".txt");
      try {
         await File.WriteAllTextAsync(path, "record one\ncontinuation\nrecord two\nrecord three\n");
         var context = Setup.Context();
         context.Connection.File = path;
         context.Connection.LinePattern = pattern;
         var factory = new CountingFactory(context.RowCapacity);
         var reader = new FileReader(context, factory);
         await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator()) {
            Assert.IsTrue(await iterator.MoveNextAsync());
            Assert.AreEqual(1, factory.Created);
         }
         var field = context.InputFields.Single(f => f.Name == "Value");
         var expected = reader.Read().Select(r => (string)r[field]).ToArray();
         var actual = (await reader.ReadStreamAsync().MaterializeAsync()).Select(r => (string)r[field]).ToArray();
         CollectionAssert.AreEqual(expected, actual);
      } finally { File.Delete(path); }
   }

   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task CommandYieldsBeforeExitAndStopsOnDisposal(bool cancel) {
      if (OperatingSystem.IsWindows()) Assert.Inconclusive("Unix shell test.");
      var context = Setup.Context();
      context.Connection.Command = "/bin/sh";
      // Keep the owned shell alive without spawning another process.
      context.Connection.Arguments = "-c \"echo $$; while :; do :; done\"";
      var factory = new CountingFactory(context.RowCapacity);
      var reader = new ConsoleCommandReader(context, factory);
      using var token = new CancellationTokenSource(TimeSpan.FromSeconds(10));
      System.Diagnostics.Process? child = null;
      await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator(token.Token)) {
         Assert.IsTrue(await iterator.MoveNextAsync());
         child = System.Diagnostics.Process.GetProcessById(int.Parse((string)iterator.Current[context.InputFields.Single(f => f.Name == "Value")]));
         Assert.IsFalse(child.HasExited);
         Assert.AreEqual(1, factory.Created);
         if (cancel) {
            token.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await iterator.MoveNextAsync());
         }
      }
      using (child) {
         Assert.IsTrue(child.HasExited, "Disposal must stop the owned process.");
      }
   }
}
