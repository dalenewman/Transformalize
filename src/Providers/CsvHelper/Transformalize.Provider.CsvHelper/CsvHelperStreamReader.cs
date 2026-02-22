using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Transforms;

namespace Transformalize.Providers.CsvHelper {

   public class CsvHelperStreamReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;
      private readonly List<ITransform> _transforms = new List<ITransform>();
      private readonly StreamReader _streamReader;

      public CsvHelperStreamReader(InputContext context, StreamReader streamReader, IRowFactory rowFactory) {
         _context = context;
         _streamReader = streamReader;
         _rowFactory = rowFactory;

         foreach (var field in context.Entity.Fields.Where(f => f.Input && f.Type != "string" && (!f.Transforms.Any() || f.Transforms.First().Method != "convert"))) {
            _transforms.Add(new ConvertTransform(new PipelineContext(context.Logger, context.Process, context.Entity, field, new Operation { Method = "convert" })));
         }
      }

      public IEnumerable<IRow> Read() {
         return _transforms.Aggregate(PreRead(), (rows, transform) => transform.Operate(rows));
      }

      private IEnumerable<IRow> PreRead() {

         _context.Debug(() => "Reading file stream.");

         var ignoreFirstLines = _context.Connection.Start > 1 ? _context.Connection.Start - 1 : _context.Connection.Start;

         var start = _context.Connection.Start;
         var end = 0;
         if (_context.Entity.IsPageRequest()) {
            start += (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
            end = start + _context.Entity.Size;
         }

         var current = _context.Connection.Start;

         var configuration = new CsvConfiguration(CultureInfo.InvariantCulture) {
            IgnoreBlankLines = true,
            Delimiter = string.IsNullOrEmpty(_context.Connection.Delimiter) ? "," : _context.Connection.Delimiter,
            Encoding = Encoding.GetEncoding(_context.Connection.Encoding)
         };

         if (_context.Connection.ErrorMode.Equals("IgnoreAndContinue", System.StringComparison.OrdinalIgnoreCase)) {
            configuration.BadDataFound = null;  // skip the record
         }

         if (_context.Connection.TextQualifier != string.Empty) {
            configuration.Escape = _context.Connection.TextQualifier[0];
            configuration.Quote = _context.Connection.TextQualifier[0];
         }

         using (var csv = new CsvReader(_streamReader, configuration)) {

            while (csv.Read()) {

               if (csv.Parser.RawRow <= ignoreFirstLines) {
                  continue;
               }

               if (end == 0 || current.Between(start, end)) {
                  var row = _rowFactory.Create();
                  for (int i = 0; i < _context.InputFields.Length; i++) {
                     var data = csv.GetField(i);
                     var field = _context.InputFields[i];
                     row[field] = data;
                  }
                  yield return row;
                  ++_context.Entity.Hits;
               }
               ++current;
               if (current == end) {
                  break;
               }
            }
         }

         _streamReader.Close();

      }

   }
}
