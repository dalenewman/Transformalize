using System.Runtime.CompilerServices;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Impl;
using Transformalize.Logging;
using Transformalize.Nulls;
using Transformalize.Transforms;
using Transformalize.Validators;

namespace Tests;

[TestClass]
public class StreamingTests {
   private static PipelineContext Context() => new(new NullLogger(), new Process { Name = "Test", ReadOnly = true }, new Entity { Name = "Rows", Alias = "Rows" });

   [TestMethod]
   [DataRow(false)]
   [DataRow(true)]
   public async Task AutofacExecutesTransformsAndValidators(bool streaming) {
      const string xml = """
         <add name='Streaming' read-only='true'>
           <entities><add name='Rows'><rows><add Value=' a '/><add Value='b'/><add Value=' '/></rows>
             <fields><add name='Value' t='trim().toupper()' v='required'/></fields>
             <calculated-fields><add name='Number' type='int' t='rownumber()'/></calculated-fields>
           </add></entities>
         </add>
         """;
      using var config = new ConfigurationContainer().CreateScope(xml, new NullLogger());
      var process = config.Resolve<Process>();
      using var scope = new Container().CreateScope(process, new NullLogger());
      var controller = scope.Resolve<IStreamingProcessController>();
      Assert.IsInstanceOfType<IStreamingPipeline>(scope.ResolveNamed<IPipeline>(process.Entities[0].Key));
      if (streaming) await controller.ExecuteStreamAsync(); else controller.Execute();
      Assert.AreEqual(3, process.Entities[0].Rows.Count);
      Assert.AreEqual(true, process.Entities[0].Rows[0]["ValueValid"]);
      Assert.AreEqual(false, process.Entities[0].Rows[2]["ValueValid"]);
      Assert.AreEqual("A", process.Entities[0].Rows[0]["Value"]);
      Assert.AreEqual("B", process.Entities[0].Rows[1]["Value"]);
      Assert.AreEqual("1", process.Entities[0].Rows[0]["Number"].ToString());
      Assert.AreEqual("2", process.Entities[0].Rows[1]["Number"].ToString());
   }

   [TestMethod]
   public async Task PullBasedReadDoesNotReadAheadAndEarlyExitDisposes() {
      var source = new Source();
      using var pipeline = new DefaultPipeline(new NullOutputController(), Context());
      pipeline.Register((IRead)source);
      pipeline.Register(new CountingTransform(Context()));
      await using (var iterator = pipeline.ReadStreamAsync().GetAsyncEnumerator()) {
         Assert.AreEqual(0, source.ReadCount);
         Assert.IsTrue(await iterator.MoveNextAsync());
         Assert.AreEqual(1, source.ReadCount);
         // Waiting consumers must not cause a background producer to run ahead.
         await Task.Delay(20);
         Assert.AreEqual(1, source.ReadCount);
      }
      Assert.IsTrue(source.Disposed);
   }

   [TestMethod]
   public async Task SequenceOverrideExpandsAndFinalizesOnce() {
      var source = new Source();
      var operation = new SequenceTransform(Context());
      var rows = await operation.OperateStreamAsync(source.ReadStreamAsync()).MaterializeAsync();
      Assert.AreEqual(1, operation.Starts);
      Assert.AreEqual(1, operation.Ends);
      Assert.AreEqual(7, rows.Count); // double three rows, then one final row
      Assert.IsTrue(source.Disposed);
   }

   [TestMethod]
   public async Task LegacyWriterIsCalledOnceWithTheWholeSequence() {
      var source = new Source();
      var writer = new LegacyWriter();
      await ((IWrite)writer).WriteStreamAsync(source.ReadStreamAsync());
      Assert.AreEqual(1, writer.Calls);
      Assert.AreEqual(3, writer.Count);
      Assert.IsTrue(source.Disposed);
   }

   [TestMethod]
   public async Task EnumerationTokenCancelsAndDisposes() {
      var source = new Source();
      using var token = new CancellationTokenSource();
      await using (var iterator = ((IRead)source).ReadStreamAsync().GetAsyncEnumerator(token.Token)) {
         Assert.IsTrue(await iterator.MoveNextAsync());
         token.Cancel();
         await Assert.ThrowsAsync<OperationCanceledException>(async () => await iterator.MoveNextAsync());
      }
      Assert.IsTrue(source.Disposed);
   }

   [TestMethod]
   [DataRow(true)]
   [DataRow(false)]
   public async Task FailureDisposesSourceAndSkipsCompletion(bool inputFailure) {
      var source = new Source { Fail = inputFailure };
      var lifecycle = new Lifecycle();
      using var pipeline = new DefaultPipeline(lifecycle, Context());
      pipeline.Register((IRead)source);
      pipeline.Register((IWrite)new Sink { Fail = !inputFailure });
      pipeline.Register((IUpdate)lifecycle);
      using var controller = new ProcessController(new[] { pipeline }, Context());
      var post = new PostAction();
      controller.PostActions.Add(post);
      await Assert.ThrowsAsync<InvalidOperationException>(() => controller.ExecuteStreamAsync());
      Assert.IsTrue(source.Disposed);
      Assert.IsFalse(lifecycle.Ended);
      Assert.IsFalse(lifecycle.Updated);
      Assert.IsFalse(post.Ran);
   }

   [TestMethod]
   public async Task BatchAdapterBoundsDemandAndKeepsOrder() {
      var source = new Source();
      await using var batches = source.ReadStreamAsync().PartitionStreamAsync(2).GetAsyncEnumerator();
      Assert.IsTrue(await batches.MoveNextAsync());
      Assert.AreEqual(2, source.ReadCount);
      Assert.AreEqual(2, batches.Current.Count);
      Assert.IsTrue(await batches.MoveNextAsync());
      Assert.AreEqual(1, batches.Current.Count);
      Assert.IsFalse(await batches.MoveNextAsync());
   }

   [TestMethod]
   public void BuiltInOperationsDoNotFallBackToMaterializing() {
      // BaseTransform/BaseValidate buffer the whole input for any subclass that overrides the
      // sequence overload without providing a native stream. That is the right default for
      // third-party subclasses, but no built-in operation should rely on it.
      var offenders = new List<string>();
      foreach (var type in typeof(BaseTransform).Assembly.GetTypes()) {
         if (type.IsAbstract || !typeof(IOperateStream).IsAssignableFrom(type)) continue;
         var sequence = type.GetMethod("Operate", new[] { typeof(IEnumerable<IRow>) });
         if (sequence == null) continue;
         var declaring = sequence.DeclaringType;
         if (declaring == typeof(BaseTransform) || declaring == typeof(BaseValidate)) continue;
         var stream = type.GetMethod(nameof(IOperateStream.OperateStreamAsync), new[] { typeof(IAsyncEnumerable<IRow>), typeof(CancellationToken) });
         if (stream?.DeclaringType == typeof(BaseTransform) || stream?.DeclaringType == typeof(BaseValidate)) {
            offenders.Add(type.FullName ?? type.Name);
         }
      }
      Assert.AreEqual(0, offenders.Count,
         "These operations override Operate(IEnumerable<IRow>) without a native OperateStreamAsync, so streaming buffers the whole input for them: "
         + string.Join(", ", offenders));
   }

   [TestMethod]
   public async Task PreCanceledExecutionDoesNoWork() {
      var lifecycle = new Lifecycle();
      using var pipeline = new DefaultPipeline(lifecycle, Context());
      await Assert.ThrowsAsync<OperationCanceledException>(() => pipeline.ExecuteStreamAsync(new CancellationToken(true)));
      Assert.IsFalse(lifecycle.Started);
   }

   private sealed class Source : IRead, IReadStream {
      public int ReadCount;
      public bool Disposed;
      public bool Fail;
      public IEnumerable<IRow> Read() => throw new AssertFailedException("Synchronous read selected");
      public Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) => throw new AssertFailedException("Obsolete read selected");
      public async IAsyncEnumerable<IRow> ReadStreamAsync([EnumeratorCancellation] CancellationToken token = default) {
         try {
            for (var i = 0; i < 3; i++) {
               token.ThrowIfCancellationRequested();
               if (Fail && i == 1) throw new InvalidOperationException("read failure");
               await Task.Yield();
               ReadCount++;
               yield return new RowFactory(1, false, false).Create();
            }
         } finally { Disposed = true; }
      }
   }
   private sealed class CountingTransform(IContext context) : BaseTransform(context, "object") {
      public override IRow Operate(IRow row) => row;
   }
   private sealed class SequenceTransform(IContext context) : BaseTransform(context, "object") {
      public int Starts, Ends;
      public override IRow Operate(IRow row) => throw new AssertFailedException("Single row overload selected");
      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
         Starts++;
         foreach (var row in rows) { yield return row; yield return row; }
         Ends++;
         yield return new RowFactory(1, false, false).Create();
      }
   }
   private sealed class LegacyWriter : IWrite {
      public int Calls, Count;
      public void Write(IEnumerable<IRow> rows) { Calls++; Count = rows.Count(); }
      public Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) { Write(rows); return Task.CompletedTask; }
   }
   private sealed class Sink : IWrite, IWriteStream {
      public bool Fail;
      public void Write(IEnumerable<IRow> rows) => throw new AssertFailedException();
      public Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) => throw new AssertFailedException();
      public async Task WriteStreamAsync(IAsyncEnumerable<IRow> rows, CancellationToken token = default) {
         await foreach (var row in rows.WithCancellation(token)) if (Fail) throw new InvalidOperationException("write failure");
      }
   }
   private sealed class Lifecycle : IOutputController, IUpdate {
      public bool Started, Ended, Updated;
      public ActionResponse Initialize() => new NullOutputController().Initialize();
      public Task<ActionResponse> InitializeAsync(CancellationToken token = default) => Task.FromResult(Initialize());
      public void Start() => Started = true;
      public void End() => Ended = true;
      public void Update() => Updated = true;
      public Task StartAsync(CancellationToken token = default) { Start(); return Task.CompletedTask; }
      public Task EndAsync(CancellationToken token = default) { End(); return Task.CompletedTask; }
      public Task UpdateAsync(CancellationToken token = default) { Update(); return Task.CompletedTask; }
   }
   private sealed class PostAction : IAction {
      public bool Ran;
      public ActionResponse Execute() { Ran = true; return new NullOutputController().Initialize(); }
      public Task<ActionResponse> ExecuteAsync(CancellationToken token = default) => Task.FromResult(Execute());
   }
}
