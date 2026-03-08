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

   public class CsvHelperStreamWriter : CsvHelperWriterBase, IWrite, IDisposable {

      private readonly OutputContext _context;
      private readonly CsvWriter _csv;
      private readonly StreamWriter _streamWriter;

      public CsvHelperStreamWriter(OutputContext context, StreamWriter streamWriter) : base(context) {
         _context = context;
         _streamWriter = streamWriter;
         _csv = new CsvWriter(_streamWriter, Config);
      }

      public void Write(IEnumerable<IRow> rows) {

         if (_context.Connection.Header == Constants.DefaultSetting) {
            WriteHeader(_csv);
            _csv.NextRecord();
         }

         foreach (var row in rows) {
            WriteRow(_csv, row);
            _context.Entity.Inserts++;
            _csv.NextRecord();
            _csv.Flush();
         }

         _csv.Flush();

      }

      public void Dispose() {
         _csv?.Dispose();
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
   }
}
