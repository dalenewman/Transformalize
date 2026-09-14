using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Logging;

namespace StreamingTestSupport;

internal static class Setup {
   public static InputContext Context(string entityAttributes = "", string fields = "<add name='Value'/>") {
      using var scope = new ConfigurationContainer().CreateScope($"<add name='Stream' read-only='true'><connections><add name='input' provider='internal' start='0'/></connections><entities><add name='Rows' {entityAttributes}><fields>{fields}</fields></add></entities></add>", new NullLogger());
      var process = scope.Resolve<Process>();
      return new InputContext(new PipelineContext(new NullLogger(), process, process.Entities[0]));
   }
}
internal sealed class CountingFactory(int capacity) : IRowFactory {
   private readonly RowFactory _inner = new(capacity, false, false);
   public int Created { get; private set; }
   public IRow Create() { Created++; return _inner.Create(); }
   public IRow Clone(IRow row, IEnumerable<IField> fields) => _inner.Clone(row, fields);
}
internal sealed class ObservedStream(byte[] bytes) : Stream {
   private readonly MemoryStream _inner = new(bytes);
   public int BytesRead { get; private set; }
   public bool Disposed { get; private set; }
   public override bool CanSeek => false;
   public override bool CanRead => !Disposed;
   public override bool CanWrite => false;
   public override long Length => bytes.Length;
   public override long Position { get => BytesRead; set => throw new NotSupportedException(); }
   public override int Read(byte[] buffer, int offset, int count) => throw new AssertFailedException("Synchronous I/O selected");
   public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token) {
      token.ThrowIfCancellationRequested();
      var read = _inner.Read(buffer, offset, count);
      BytesRead += read;
      return Task.FromResult(read);
   }
   protected override void Dispose(bool disposing) { Disposed = true; if (disposing) _inner.Dispose(); base.Dispose(disposing); }
   public override void Flush() => throw new NotSupportedException();
   public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
   public override void SetLength(long value) => throw new NotSupportedException();
   public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}

/// <summary>
/// Linked into every streaming test project, so each one enforces the contract for the provider
/// assemblies it references. The compatibility adapters in StreamingExtensions exist for
/// third-party components; nothing shipped in this repository should depend on them, because they
/// materialize the whole input and defeat the point of streaming.
/// </summary>
[TestClass]
public class StreamingContractTests {

   [TestMethod]
   public void ShippedComponentsImplementTheStreamingContracts() {
      foreach (var assembly in ReferencedTransformalizeAssemblies()) {
         foreach (var type in SafeTypes(assembly)) {
            if (type.IsAbstract || type.IsInterface || !type.IsPublic) continue;

            // A reader that can actually produce rows must stream. The metadata-only input
            // providers throw from Read(), so they can never reach the buffering adapter.
            if ((typeof(IRead).IsAssignableFrom(type) || typeof(IInputProvider).IsAssignableFrom(type))
                && !typeof(IReadStream).IsAssignableFrom(type) && !ThrowsFrom(type, "Read")) {
               Assert.Fail($"{type.FullName} reads rows but does not implement IReadStream, so ReadStreamAsync buffers its whole result.");
            }

            if ((typeof(IWrite).IsAssignableFrom(type) || typeof(IOutputProvider).IsAssignableFrom(type))
                && !typeof(IWriteStream).IsAssignableFrom(type) && !ThrowsFrom(type, "Write")) {
               Assert.Fail($"{type.FullName} consumes rows but does not implement IWriteStream, so WriteStreamAsync buffers the whole input before calling it.");
            }
         }
      }
   }

   private static IEnumerable<System.Reflection.Assembly> ReferencedTransformalizeAssemblies() {
      var entry = typeof(StreamingContractTests).Assembly;
      foreach (var name in entry.GetReferencedAssemblies()) {
         if (name.Name == null || !name.Name.StartsWith("Transformalize")) continue;
         System.Reflection.Assembly loaded;
         try { loaded = System.Reflection.Assembly.Load(name); } catch { continue; }
         yield return loaded;
      }
   }

   private static IEnumerable<Type> SafeTypes(System.Reflection.Assembly assembly) {
      try { return assembly.GetTypes(); } catch (System.Reflection.ReflectionTypeLoadException ex) { return ex.Types.Where(t => t != null)!; }
   }

   /// <summary>True when the synchronous row method is a NotImplementedException stub (metadata-only provider).</summary>
   private static bool ThrowsFrom(Type type, string method) {
      var mi = method == "Read"
         ? type.GetMethod("Read", Type.EmptyTypes)
         : type.GetMethod("Write", new[] { typeof(IEnumerable<IRow>) });
      if (mi == null || mi.DeclaringType != type) return false;
      var body = mi.GetMethodBody();
      if (body == null) return false;
      // A stub that only throws has a tiny IL body; anything that enumerates rows is far larger.
      return body.GetILAsByteArray()?.Length <= 12;
   }
}
