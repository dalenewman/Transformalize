using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
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

         if (_context.Connection.Header == Constants.DefaultSetting) {
            WriteHeader(_csv);
            await _csv.NextRecordAsync().ConfigureAwait(false);
         }

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            WriteRow(_csv, row);
            _context.Entity.Inserts++;
            await _csv.NextRecordAsync().ConfigureAwait(false);
            await _csv.FlushAsync().ConfigureAwait(false);
         }

         await _csv.FlushAsync().ConfigureAwait(false);

      }
   }
}
