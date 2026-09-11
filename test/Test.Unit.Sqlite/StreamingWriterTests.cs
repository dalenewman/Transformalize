using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Impl;
using Transformalize.Logging;
using Transformalize.Nulls;
using Transformalize.Providers.Ado;
using Transformalize.Providers.SQLite;

namespace IntegrationTests;

[TestClass]
[DoNotParallelize]
public class StreamingWriterTests {
   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task DirectInserterRollsBackAndDoesNotCountFailedWrites(bool cancellation) {
      using var fixture = new Fixture();
      using var token = new CancellationTokenSource();
      async IAsyncEnumerable<IRow> Source() {
         yield return fixture.Row(1);
         yield return fixture.Row(2); // one complete batch has been written inside the transaction
         await Task.Yield();
         if (cancellation) { token.Cancel(); token.Token.ThrowIfCancellationRequested(); }
         throw new InvalidOperationException("read failure after first batch");
      }
      var writer = new AdoEntityInserter(fixture.Context, fixture.Factory);
      if (cancellation) await Assert.ThrowsAsync<OperationCanceledException>(() => writer.WriteStreamAsync(Source(), token.Token));
      else await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteStreamAsync(Source()));
      Assert.AreEqual(0, fixture.Count());
      Assert.AreEqual(0u, fixture.Context.Entity.Inserts);
   }

   [TestMethod]
   public async Task EntityWriterCommitsOnlyCompletedBatchesBeforeReadFailure() {
      using var fixture = new Fixture();
      async IAsyncEnumerable<IRow> Source() {
         yield return fixture.Row(1);
         yield return fixture.Row(2);
         Assert.AreEqual(2, fixture.Count(), "Writer must commit before pulling more input");
         await Task.Yield();
         yield return fixture.Row(3);
         throw new InvalidOperationException("read failure in next batch");
      }
      var writer = new AdoEntityWriter(fixture.Context, new NullBatchReader(),
         new AdoEntityInserter(fixture.Context, fixture.Factory), new NullWriter());
      await Assert.ThrowsAsync<InvalidOperationException>(() => writer.WriteStreamAsync(Source()));
      Assert.AreEqual(2, fixture.Count());
      Assert.AreEqual(2u, fixture.Context.Entity.Inserts);
   }

   [TestMethod]
   public async Task ConstraintFailureRollsBackTheCallAndPropagates() {
      using var fixture = new Fixture();
      var rows = new[] { fixture.Row(1), fixture.Row(2), fixture.Row(1) };
      var writer = new AdoEntityInserter(fixture.Context, fixture.Factory);
      await Assert.ThrowsAsync<SqliteException>(() => writer.WriteStreamAsync(rows.AsAsyncStream()));
      Assert.AreEqual(0, fixture.Count());
      Assert.AreEqual(0u, fixture.Context.Entity.Inserts);
   }

   private sealed class Fixture : IDisposable {
      private readonly string _path = Path.Combine(Path.GetTempPath(), "tfl-stream-" + Guid.NewGuid() + ".db");
      public OutputContext Context { get; }
      public SqliteConnectionFactory Factory { get; }
      private readonly Field _field = new() { Name = "Value", Alias = "Value", Type = "int", Index = 0, MasterIndex = 0, Output = true };
      private readonly string _table;
      public Fixture() {
         var connection = new Connection { Name = "output", Provider = "sqlite", File = _path };
         var entity = new Entity { Name = "Rows", Alias = "Rows", Fields = new() { _field, new Field { Alias = Constants.TflHashCode, Output = false }, new Field { Alias = Constants.TflDeleted, Output = false } }, InsertSize = 2 };
         var process = new Process { Name = "Test", Mode = "init", ReadOnly = true, Connections = new() { connection }, Entities = new() { entity } };
         Context = new OutputContext(new PipelineContext(new NullLogger(), process, entity)) { OutputFields = new[] { _field } };
         Factory = new SqliteConnectionFactory(connection);
         _table = Factory.Enclose(entity.OutputTableName(process.Name));
         using var cn = Factory.GetConnection(); cn.Open();
         cn.Execute($"CREATE TABLE {_table} (Value INTEGER PRIMARY KEY)");
      }
      public IRow Row(int value) { var row = new RowFactory(1, false, false).Create(); row[_field] = value; return row; }
      public int Count() { using var cn = Factory.GetConnection(); cn.Open(); return cn.ExecuteScalar<int>($"SELECT COUNT(*) FROM {_table}"); }
      public void Dispose() { SqliteConnection.ClearAllPools(); File.Delete(_path); }
   }
}
