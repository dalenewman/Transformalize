using Transformalize;
using System.Diagnostics.CodeAnalysis;
using System.Data;
using System.Data.Common;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Impl;
using Transformalize.Logging;
using Transformalize.Providers.Ado;

namespace Test.Unit.Ado;

[TestClass]
public class StreamingReaderTests {
   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task ReaderUsesAsyncIoAndDisposesOnEarlyExit(bool keys) {
      var fixture = Create(keys);
      await using (var enumerator = fixture.Reader.ReadStreamAsync().GetAsyncEnumerator()) {
         Assert.IsFalse(fixture.Connection.Opened);
         Assert.IsTrue(await enumerator.MoveNextAsync());
         Assert.IsTrue(fixture.Connection.Opened);
         Assert.IsFalse(fixture.Connection.WasDisposed);
         Assert.IsFalse(fixture.Connection.Command.WasDisposed);
         Assert.IsFalse(fixture.Connection.Command.Reader.IsClosed);
      }
      Assert.IsTrue(fixture.Connection.WasDisposed);
      Assert.IsTrue(fixture.Connection.Command.WasDisposed);
      Assert.IsTrue(fixture.Connection.Command.Reader.IsClosed);
   }

   [TestMethod]
   public async Task ExplicitBufferClosesConnectionBeforeFirstRow() {
      var fixture = Create(false, true);
      await using var enumerator = fixture.Reader.ReadStreamAsync().GetAsyncEnumerator();
      Assert.IsTrue(await enumerator.MoveNextAsync());
      Assert.IsTrue(fixture.Connection.WasDisposed);
      Assert.IsTrue(fixture.Connection.Command.Reader.IsClosed);
   }

   [TestMethod]
   public async Task CancellationReleasesAllResources() {
      var fixture = Create(false);
      using var token = new CancellationTokenSource();
      await using (var enumerator = fixture.Reader.ReadStreamAsync().GetAsyncEnumerator(token.Token)) {
         Assert.IsTrue(await enumerator.MoveNextAsync());
         token.Cancel();
         await Assert.ThrowsAsync<OperationCanceledException>(async () => await enumerator.MoveNextAsync());
      }
      Assert.IsTrue(fixture.Connection.WasDisposed);
      Assert.IsTrue(fixture.Connection.Command.WasDisposed);
      Assert.IsTrue(fixture.Connection.Command.Reader.IsClosed);
   }

   [TestMethod]
   public async Task QueryFailurePropagatesAndDisposesCommandAndConnection() {
      var fixture = Create(false);
      fixture.Connection.Command.Fail = true;
      await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Reader.ReadStreamAsync().MaterializeAsync());
      Assert.IsTrue(fixture.Connection.WasDisposed);
      Assert.IsTrue(fixture.Connection.Command.WasDisposed);
   }

   private static (IReadStream Reader, FakeConnection Connection) Create(bool keys, bool buffer = false) {
      var connection = new Connection { Name = "input", Provider = "sqlserver", Buffer = buffer };
      var field = new Field { Name = "Value", Alias = "Value", Type = "int", Index = 0, MasterIndex = 0 };
      var entity = new Entity { Name = "Rows", Alias = "Rows", Input = "input", Query = "select Value from Rows", Fields = new() { field } };
      var process = new Process { Name = "Test", ReadOnly = true, Connections = new() { connection }, Entities = new() { entity } };
      var context = new InputContext(new PipelineContext(new NullLogger(), process, entity));
      var cn = new FakeConnection();
      var factory = new FakeFactory(cn);
      var rows = new RowFactory(1, false, false);
      IReadStream reader = keys ? new AdoReader(context, new[] { field }, factory, rows, ReadFrom.Input)
         : new AdoInputReader(context, new[] { field }, factory, rows);
      return (reader, cn);
   }

   private sealed class FakeFactory(FakeConnection connection) : IConnectionFactory {
      public IDbConnection GetConnection(string? appName = null) => connection;
      public string GetConnectionString(string? appName = null) => "fake";
      public string Enclose(string name) => name;
      public string SqlDataType(Field field) => "int";
      public AdoProvider AdoProvider => AdoProvider.SqlServer;
      public string Terminator => ";";
      public bool SupportsLimit => false;
   }

   private sealed class FakeConnection : DbConnection {
      public readonly FakeCommand Command = new();
      public bool Opened, WasDisposed;
      [AllowNull]
      public override string ConnectionString { get; set; } = "";
      public override string Database => "test";
      public override string DataSource => "test";
      public override string ServerVersion => "1";
      public override ConnectionState State => Opened ? ConnectionState.Open : ConnectionState.Closed;
      public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();
      public override void Open() => throw new AssertFailedException("Synchronous Open selected");
      public override Task OpenAsync(CancellationToken cancellationToken) { cancellationToken.ThrowIfCancellationRequested(); Opened = true; return Task.CompletedTask; }
      public override void Close() => Opened = false;
      protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();
      protected override DbCommand CreateDbCommand() => Command;
      protected override void Dispose(bool disposing) { WasDisposed = true; Close(); base.Dispose(disposing); }
   }

   private sealed class FakeCommand : DbCommand {
      public readonly DataTableReader Reader;
      public bool WasDisposed, Fail;
      public FakeCommand() {
         var table = new DataTable(); table.Columns.Add("Value", typeof(int));
         table.Rows.Add(1); table.Rows.Add(2); table.Rows.Add(3);
         Reader = table.CreateDataReader();
      }
      [AllowNull]
      public override string CommandText { get; set; } = "";
      public override int CommandTimeout { get; set; }
      public override CommandType CommandType { get; set; }
      public override bool DesignTimeVisible { get; set; }
      public override UpdateRowSource UpdatedRowSource { get; set; }
      protected override DbConnection? DbConnection { get; set; } = null!;
      protected override DbTransaction? DbTransaction { get; set; } = null!;
      protected override DbParameterCollection DbParameterCollection => throw new NotSupportedException();
      public override void Cancel() { }
      public override int ExecuteNonQuery() => throw new NotSupportedException();
      public override object ExecuteScalar() => throw new NotSupportedException();
      public override void Prepare() => throw new NotSupportedException();
      protected override DbParameter CreateDbParameter() => throw new NotSupportedException();
      protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new AssertFailedException("Synchronous ExecuteReader selected");
      protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) {
         cancellationToken.ThrowIfCancellationRequested();
         if (Fail) throw new InvalidOperationException("query failed");
         return Task.FromResult<DbDataReader>(Reader);
      }
      protected override void Dispose(bool disposing) { WasDisposed = true; base.Dispose(disposing); }
   }
}
