using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;

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
            _csv.NextRecordAsync().ConfigureAwait(false);
         }

         foreach (var row in rows) {
            WriteRow(_csv, row);
            _context.Entity.Inserts++;
            _csv.NextRecordAsync().ConfigureAwait(false);
            _csv.FlushAsync().ConfigureAwait(false);
         }

         _csv.FlushAsync().ConfigureAwait(false);
         
      }

      public void Dispose() {
         _csv?.Dispose();
      }
   }
}
