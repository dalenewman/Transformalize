using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet;
using SolrNet.Commands.Parameters;
using StreamingTestSupport;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Solr;

namespace Test.Integration.Core;

[TestClass]
public class StreamingReaderTests {
   [TestMethod]
   [DataRow("4.6")]
   [DataRow("8.11")]
   public async Task FetchesOnlyDemandedPagesAndPreservesRows(string version) {
      var context = Setup.Context("read-size='2'", "<add name='Value' primary-key='true'/>");
      context.Connection.Version = version;
      var solr = DispatchProxy.Create<ISolrReadOnlyOperations<Dictionary<string, object>>, Pages>();
      var calls = (Pages)solr;
      var factory = new CountingFactory(context.RowCapacity);
      IRead reader = new SolrInputReader(solr, context, context.InputFields.Where(f => !f.System).ToArray(), factory);
      await using (var iterator = reader.ReadStreamAsync().GetAsyncEnumerator()) {
         Assert.AreEqual(0, calls.Queries);
         Assert.IsTrue(await iterator.MoveNextAsync());
         Assert.AreEqual(1, calls.Queries);
         Assert.AreEqual(1, factory.Created);
      }
      calls.Queries = 0;
      var rows = await reader.ReadStreamAsync().MaterializeAsync();
      var field = context.InputFields.Single(f => f.Name == "Value");
      CollectionAssert.AreEqual(new[] { "1", "2", "3" }, rows.Select(r => (string)r[field]).ToArray());
      Assert.AreEqual(2, calls.Queries);
   }

   public class Pages : DispatchProxy {
      public int Queries;
      protected override object? Invoke(MethodInfo? method, object?[]? args) {
         Assert.AreEqual("QueryAsync", method!.Name);
         ((CancellationToken)args![2]!).ThrowIfCancellationRequested();
         var options = (QueryOptions)args[1]!;
         Assert.AreEqual(2, options.Rows);
         Queries++;
         var page = new SolrQueryResults<Dictionary<string, object>> { NumFound = 3, NextCursorMark = new StartOrCursor.Cursor("page-" + Queries) };
         if (Queries == 1) {
            page.Add(new Dictionary<string, object> { ["Value"] = "1" });
            page.Add(new Dictionary<string, object> { ["Value"] = "2" });
         } else if (Queries == 2) page.Add(new Dictionary<string, object> { ["Value"] = "3" });
         return Task.FromResult(page);
      }
   }
}
