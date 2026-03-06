using System.Collections.Generic;
using System.IO;
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.CsvHelper {
   public class CsvHelperReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;

      public CsvHelperReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
      }

      public IEnumerable<IRow> Read() {
         var fileInfo = File.FileUtility.Find(_context.Connection.File);
         var encoding = Encoding.GetEncoding(_context.Connection.Encoding);
         return new CsvHelperStreamReader(_context, new StreamReader(new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), encoding), _rowFactory).Read();
      }

      public Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) {
         var fileInfo = File.FileUtility.Find(_context.Connection.File);
         var encoding = Encoding.GetEncoding(_context.Connection.Encoding);
         var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
         var reader = new StreamReader(stream, encoding);
         return new CsvHelperStreamReader(_context, reader, _rowFactory).ReadAsync(token);
      }
   }
}
