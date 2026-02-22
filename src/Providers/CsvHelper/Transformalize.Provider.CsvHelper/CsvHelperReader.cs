using System.Collections.Generic;
using System.IO;
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;

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
   }
}
