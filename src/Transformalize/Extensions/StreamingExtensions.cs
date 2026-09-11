using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Contracts;

namespace Transformalize.Extensions {
   /// <summary>Opt-in streaming with compatibility adapters for existing components.</summary>
   public static class StreamingExtensions {
      public static IAsyncEnumerable<IRow> ReadStreamAsync(this IRead reader, CancellationToken token = default) {
         return reader is IReadStream stream ? stream.ReadStreamAsync(token) : ReadLegacy(reader.ReadAsync, token);
      }

      public static IAsyncEnumerable<IRow> ReadStreamAsync(this IInputProvider reader, CancellationToken token = default) {
         return reader is IReadStream stream ? stream.ReadStreamAsync(token) : ReadLegacy(reader.ReadAsync, token);
      }

      public static async IAsyncEnumerable<IReadOnlyList<IRow>> PartitionStreamAsync(this IAsyncEnumerable<IRow> rows,
         int size, [EnumeratorCancellation] CancellationToken token = default) {
         if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
         token.ThrowIfCancellationRequested();
         var batch = new List<IRow>(size);
         await foreach (var row in rows.WithCancellation(token).ConfigureAwait(false)) {
            token.ThrowIfCancellationRequested();
            batch.Add(row);
            if (batch.Count == size) {
               yield return batch;
               batch = new List<IRow>(size);
            }
         }
         token.ThrowIfCancellationRequested();
         if (batch.Count != 0) yield return batch;
      }

      public static Task WriteStreamAsync(this IWrite writer, IAsyncEnumerable<IRow> rows, CancellationToken token = default) {
         return writer is IWriteStream stream ? stream.WriteStreamAsync(rows, token) : WriteLegacy(writer.WriteAsync, rows, token);
      }

      public static Task WriteStreamAsync(this IOutputProvider writer, IAsyncEnumerable<IRow> rows, CancellationToken token = default) {
         return writer is IWriteStream stream ? stream.WriteStreamAsync(rows, token) : WriteLegacy(writer.WriteAsync, rows, token);
      }

      public static Task ExecuteStreamAsync(this IProcessController controller, CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         return controller is IExecuteStream stream ? stream.ExecuteStreamAsync(token) : controller.ExecuteAsync(token);
      }

      public static Task ExecuteStreamAsync(this IPipeline pipeline, CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         return pipeline is IExecuteStream stream ? stream.ExecuteStreamAsync(token) : pipeline.ExecuteAsync(token);
      }

      // The legacy read is deliberately invoked once. It may buffer or perform synchronous I/O.
      private static async IAsyncEnumerable<IRow> ReadLegacy(Func<CancellationToken, Task<IEnumerable<IRow>>> read,
         [EnumeratorCancellation] CancellationToken token) {
         token.ThrowIfCancellationRequested();
         var rows = await read(token).ConfigureAwait(false);
         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            yield return row;
         }
      }

      private static async Task WriteLegacy(Func<IEnumerable<IRow>, CancellationToken, Task> write,
         IAsyncEnumerable<IRow> rows, CancellationToken token) {
         // Never restart an unknown writer per batch: it may truncate output or finalize a document.
         var buffered = await MaterializeAsync(rows, token).ConfigureAwait(false);
         token.ThrowIfCancellationRequested();
         await write(buffered, token).ConfigureAwait(false);
      }

      public static async Task<List<IRow>> MaterializeAsync(this IAsyncEnumerable<IRow> rows, CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         var result = new List<IRow>();
         await foreach (var row in rows.WithCancellation(token).ConfigureAwait(false)) {
            token.ThrowIfCancellationRequested();
            result.Add(row);
         }
         return result;
      }

      public static async IAsyncEnumerable<IRow> AsAsyncStream(this IEnumerable<IRow> rows,
         [EnumeratorCancellation] CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            yield return row;
         }
         await Task.CompletedTask.ConfigureAwait(false);
      }

      public static async IAsyncEnumerable<IRow> OperateStreamAsync(this IOperation operation,
         IAsyncEnumerable<IRow> rows, [EnumeratorCancellation] CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         if (operation is IOperateStream stream) {
            await foreach (var row in stream.OperateStreamAsync(rows, token).WithCancellation(token).ConfigureAwait(false)) {
               token.ThrowIfCancellationRequested();
               yield return row;
            }
         } else {
            // Preserve the complete sequence contract, including initialization and finalization.
            var buffered = await rows.MaterializeAsync(token).ConfigureAwait(false);
            foreach (var row in operation.Operate(buffered)) {
               token.ThrowIfCancellationRequested();
               yield return row;
            }
         }
      }
   }
}
