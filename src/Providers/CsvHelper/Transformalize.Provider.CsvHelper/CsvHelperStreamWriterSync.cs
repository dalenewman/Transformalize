using CsvHelper;
using System.Collections.Generic;
using System.IO;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.CsvHelper {

   public class CsvHelperStreamWriterSync : CsvHelperWriterBase, IWrite {

      private readonly OutputContext _context;
      private readonly StreamWriter _streamWriter;

      public CsvHelperStreamWriterSync(OutputContext context, StreamWriter streamWriter) : base(context) {
         _context = context;
         _streamWriter = streamWriter;
      }

      public void Write(IEnumerable<IRow> rows) {

         var csv = new CsvWriter(_streamWriter, Config);

         try {
            if (_context.Connection.Header == Constants.DefaultSetting) {
               WriteHeader(csv);
               csv.NextRecord();
            }

            foreach (var row in rows) {
               WriteRow(csv, row);
               _context.Entity.Inserts++;
               csv.NextRecord();
            }

         } finally {
            csv?.Flush();
            csv?.Dispose();
         }
      }
   }
}
