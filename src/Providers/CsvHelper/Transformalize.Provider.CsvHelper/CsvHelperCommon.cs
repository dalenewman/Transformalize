using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Text;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.CsvHelper {
   public class CsvHelperWriterBase {

      private readonly OutputContext _context;

      protected CsvConfiguration Config { get; set; }

      public CsvHelperWriterBase(OutputContext context) {
         _context = context;

         Config = new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture) {
            Delimiter = string.IsNullOrEmpty(_context.Connection.Delimiter) ? "," : _context.Connection.Delimiter,
            IgnoreBlankLines = true,
            NewLine = Environment.NewLine
         };

         if (!string.IsNullOrEmpty(_context.Connection.TextQualifier)) {
            Config.Quote = _context.Connection.TextQualifier[0];
         }
         Config.Encoding = Encoding.GetEncoding(_context.Connection.Encoding);
         Config.TrimOptions = TrimOptions.None;
      }
      public void WriteHeader(CsvWriter writer) {
         foreach (var field in _context.OutputFields) {
            writer.WriteField(field.Label == string.Empty ? field.Alias : field.Label);
         }
      }

      public void WriteRow(CsvWriter writer, IRow row) {
         foreach (var field in _context.OutputFields) {

            string strVal;

            switch (field.Type) {
               case "byte[]":
                  strVal = Convert.ToBase64String((byte[])row[field]);
                  break;
               case "string":
                  strVal = row[field] is string str ? str : row[field].ToString();
                  break;
               case "datetime":
                  var format = field.Format == string.Empty ? "o" : field.Format.Replace("AM/PM", "tt");
                  strVal = row[field] is DateTime dt ? dt.ToString(format) : Convert.ToDateTime(row[field]).ToString(format);
                  break;
               case "float":
               case "decimal":
               case "single":
               case "double":
                  if (field.Format == string.Empty) {
                     strVal = row[field].ToString();
                  } else {
                     switch (field.Type) {
                        case "single":
                        case "float":
                           strVal = row[field] is float flt ? flt.ToString(field.Format) : Convert.ToSingle(row[field]).ToString(field.Format);
                           break;
                        case "decimal":
                           strVal = row[field] is decimal dec ? dec.ToString(field.Format) : Convert.ToDecimal(row[field]).ToString(field.Format);
                           break;
                        case "double":
                           strVal = row[field] is double dbl ? dbl.ToString(field.Format) : Convert.ToDouble(row[field]).ToString(field.Format);
                           break;
                        default:
                           strVal = row[field].ToString();
                           break;
                     }
                  }
                  break;
               default:
                  strVal = row[field].ToString();
                  break;
            }
            writer.WriteField(strVal);

         }
      }
   }
}
