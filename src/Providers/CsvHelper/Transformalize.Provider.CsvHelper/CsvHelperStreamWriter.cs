using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.CsvHelper {

   public class CsvHelperStreamWriter : CsvHelperWriterBase, IWriteStream, IWrite, IDisposable {

      private readonly OutputContext _context;
      private readonly StreamWriter _streamWriter;
      private readonly bool _ownsStreamWriter;
      private CsvWriter _csv;
      private CsvWriter Csv => _csv ?? (_csv = new CsvWriter(_streamWriter, Config, true));

      public CsvHelperStreamWriter(OutputContext context, StreamWriter streamWriter) : this(context, streamWriter, true) {
      }

      public CsvHelperStreamWriter(OutputContext context, StreamWriter streamWriter, bool ownsStreamWriter) : base(context) {
         _context = context;
         _streamWriter = streamWriter;
         _ownsStreamWriter = ownsStreamWriter;
      }

      public void Write(IEnumerable<IRow> rows) {

         // The async methods stage rows in memory and never need a CsvWriter over the destination
         // stream. Create it only for the synchronous path and leave the underlying StreamWriter
         // open so its ownership can be handled explicitly.
         var csv = Csv;

         if (_context.Connection.Header == Constants.DefaultSetting) {
            WriteHeader(csv);
            csv.NextRecord();
         }

         foreach (var row in rows) {
            WriteRow(csv, row);
            _context.Entity.Inserts++;
            csv.NextRecord();
            csv.Flush();
         }

         csv.Flush();

      }

      public void Dispose() {
         try {
            _csv?.Dispose();
         } finally {
            if (_ownsStreamWriter) {
               _streamWriter.Dispose();
            }
         }
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {

         // CsvWriter.WriteField() calls TextWriter.Write() synchronously. Writing directly to
         // _streamWriter (which wraps Response.Body) would overflow its buffer and trigger a
         // synchronous flush to the stream, which ASP.NET Core disallows. Instead, we buffer
         // each row in a StringBuilder via a StringWriter — pure memory, no stream IO — then
         // async-write the completed row string to _streamWriter.
         var sb = new StringBuilder();
         using (var sw = new StringWriter(sb))
         using (var csv = new CsvWriter(sw, Config)) {

            if (_context.Connection.Header == Constants.DefaultSetting) {
               WriteHeader(csv);
               csv.NextRecord(); // sync is safe: StringWriter writes to StringBuilder, not a stream
               await _streamWriter.WriteAsync(sb.ToString()).ConfigureAwait(false);
               sb.Clear();
            }

            foreach (var row in rows) {
               token.ThrowIfCancellationRequested();
               WriteRow(csv, row);
               _context.Entity.Inserts++;
               csv.NextRecord(); // sync is safe: StringWriter writes to StringBuilder, not a stream
               await _streamWriter.WriteAsync(sb.ToString()).ConfigureAwait(false);
               sb.Clear();
            }

         }

         await _streamWriter.FlushAsync().ConfigureAwait(false);

      }

      public async Task WriteStreamAsync(IAsyncEnumerable<IRow> rows, CancellationToken token = default) {

         // Same buffering note as WriteAsync: each row is staged in a StringBuilder, never the set.
         var sb = new StringBuilder();
         using (var sw = new StringWriter(sb))
         using (var csv = new CsvWriter(sw, Config)) {

            if (_context.Connection.Header == Constants.DefaultSetting) {
               WriteHeader(csv);
               csv.NextRecord(); // sync is safe: StringWriter writes to StringBuilder, not a stream
               await _streamWriter.WriteAsync(sb.ToString()).ConfigureAwait(false);
               sb.Clear();
            }

            await foreach (var row in rows.WithCancellation(token).ConfigureAwait(false)) {
               token.ThrowIfCancellationRequested();
               WriteRow(csv, row);
               _context.Entity.Inserts++;
               csv.NextRecord(); // sync is safe: StringWriter writes to StringBuilder, not a stream
               await _streamWriter.WriteAsync(sb.ToString()).ConfigureAwait(false);
               sb.Clear();
            }

         }

         await _streamWriter.FlushAsync().ConfigureAwait(false);

      }
   }
}
